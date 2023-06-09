﻿using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;

namespace The_BFME_API_by_MarcellVokk.Network
{
    public class NetworkClient
    {
        public string CurrentRoomId = "";

        public NetworkClient()
        {
            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)))
            {
                throw new AccessViolationException("EasyVLanClient needs administrator privelidges to work!");
            }

            try
            {
                if(!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier"));
                }

                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One"));
                }

                foreach (string resource in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.Contains(".RuntimeResources")))
                {
                    string path = Path.GetFullPath(@$"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\");
                    List<string> chunks = resource.Split('.').ToList();

                    foreach (string s in chunks.GetRange(2, chunks.Count - 4))
                    {
                        path += s.Replace("_", "-") + @"\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }

                    path += chunks[chunks.Count - 2] + "." + chunks[chunks.Count - 1];

                    if (!File.Exists(path))
                    {
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                        {
                            byte[] data = new byte[stream.Length];
                            stream.Read(data, 0, data.Length);

                            File.WriteAllBytes(path, data);
                        }
                    }
                }
            }
            catch
            {

            }

            Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -I") { CreateNoWindow = true })?.WaitForExit();

            ServiceController service = new ServiceController("ZeroTierOneService");

            if (service != null && service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
            }

            RegistryHelper.DisableNewNetworkPopup();
        }

        public void Dispose()
        {
            CurrentRoomId = "";

            foreach (string room in GetAllCurrentRooms())
            {
                Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q leave {room}") { CreateNoWindow = true })?.WaitForExit();
            }

            ServiceController service = new ServiceController("ZeroTierOneService");

            if (service != null && service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
            }
        }

        public static void Remove()
        {
            Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -R") { CreateNoWindow = true })?.WaitForExit();
        }

        public async Task<string> JoinRoom(string roomId, Action cancellationAssertion = null)
        {
            await LeaveRoom();

            CurrentRoomId = roomId;

            ServiceController service = new ServiceController("ZeroTierOneService");
            if (service != null && service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
            }
            RegistryHelper.DisableNewNetworkPopup();

            await Task.Run(() => Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q join {roomId}") { CreateNoWindow = true })?.WaitForExit());

            string ip = await GetLocalClientIp(cancellationAssertion);

            return ip;
        }

        public async Task LeaveRoom()
        {
            CurrentRoomId = "";

            await Task.Run(() =>
            {
                bool wasInAnyRoom = false;

                foreach (string room in GetAllCurrentRooms())
                {
                    wasInAnyRoom = true;
                    Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q leave {room}") { CreateNoWindow = true })?.WaitForExit();
                }

                if (wasInAnyRoom)
                {
                    Thread.Sleep(6000);
                }
            });
        }

        public async Task<string> GetLocalClientIp(Action cancellationAssertion = null)
        {
            string result = "<error>";

            await Task.Run(() =>
            {
                try
                {
                    Stopwatch timeout = Stopwatch.StartNew();

                    while (result == "<error>")
                    {
                        if (timeout.Elapsed > TimeSpan.FromSeconds(45))
                        {
                            break;
                        }

                        Process? get_ip_process = Process.Start(new ProcessStartInfo("cmd", $@"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q get {CurrentRoomId} ip") { RedirectStandardOutput = true, CreateNoWindow = true });
                        get_ip_process.OutputDataReceived += (s, e) =>
                        {
                            if (e.Data != null && e.Data.ToString().Length > 1 && e.Data.ToString().Contains(".") && result == "<error>")
                            {
                                result = e.Data.ToString();
                            }
                        };
                        get_ip_process.BeginOutputReadLine();
                        get_ip_process.WaitForExit();

                        Thread.Sleep(1000);
                    }

                    timeout.Stop();
                }
                catch(Exception ex)
                {
                    if (ex is TaskCanceledException)
                    {
                        result = "<error>";
                        return;
                    }
                }
            });

            return result;
        }

        private List<string> GetAllCurrentRooms()
        {
            string result = "";

            Process? get_ip_process = Process.Start(new ProcessStartInfo("cmd", $@"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q -j listnetworks") { RedirectStandardOutput = true, CreateNoWindow = true });
            get_ip_process.OutputDataReceived += (s, e) =>
            {
                result += e.Data?.ToString();
            };
            get_ip_process.BeginOutputReadLine();
            get_ip_process.WaitForExit();

            List<string> finalResult = new List<string>();

            try
            {
                finalResult = JArray.Parse(result).Select(x => x["id"].ToString()).ToList();
            }
            catch(Exception ex)
            {
                
            }

            return finalResult;
        }
    }
}