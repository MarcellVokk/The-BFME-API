using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using The_BFME_API.Logging;

namespace The_BFME_API.Network
{
    public class NetworkClient
    {
        public string CurrentRoomId = "";
        private bool Initialized = false;

        public NetworkClient()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                throw new AccessViolationException("NetworkClient needs administrator privelidges to work!");
            }

            try
            {
                Logger.LogDiagnostic("Extracting self contained ZeroTier binaries...", "NetworkClient");

                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier")))
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

                Logger.LogDiagnostic("Extracting self contained ZeroTier binaries... DONE!", "NetworkClient");
            }
            catch
            {

            }

            Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -I") { CreateNoWindow = true })?.WaitForExit();

            RegistryHelper.DisableNewNetworkPopup();
        }

        public async Task Init()
        {
            Logger.LogDiagnostic("Initializing...", "NetworkClient");

            ServiceController service = new ServiceController("ZeroTierOneService");

            await Task.Run(() =>
            {
                if (service != null && service.Status == ServiceControllerStatus.Stopped)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            service.Start();
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(50);
                        }
                    }
                }

                foreach (string room in GetAllCurrentRooms())
                {
                    Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q leave {room}") { CreateNoWindow = true })?.WaitForExit();
                }
            });

            Initialized = true;

            Logger.LogDiagnostic("Initializing... DONE!", "NetworkClient");
        }

        public void Dispose()
        {
            Logger.LogDiagnostic("Disposing...", "NetworkClient");

            ServiceController service = new ServiceController("ZeroTierOneService");

            if (service != null && service.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    service.Stop();
                }
                catch
                {

                }
            }

            Logger.LogDiagnostic("Disposing... DONE!", "NetworkClient");
        }

        public static void Remove()
        {
            Logger.LogDiagnostic("Removing ZeroTier binaries...", "NetworkClient");

            Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -R") { CreateNoWindow = true })?.WaitForExit();

            Logger.LogDiagnostic("Removing ZeroTier binaries... DONE!", "NetworkClient");
        }

        public async Task<string> JoinRoom(string roomId, Action cancellationAssertion = null)
        {
            if (!Initialized) throw new Exception("Not initialized! Call .Init() on client instance after constructor.");

            await LeaveRoom(true);

            Logger.LogDiagnostic($"Joining room {roomId}...", "NetworkClient");

            CurrentRoomId = roomId;

            ServiceController service = new ServiceController("ZeroTierOneService");
            if (service != null && service.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    service.Start();
                }
                catch
                {

                }
            }
            RegistryHelper.DisableNewNetworkPopup();

            await Task.Run(() => Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q join {roomId}") { CreateNoWindow = true })?.WaitForExit());

            Logger.LogDiagnostic($"Waiting for room...", "NetworkClient");

            string ip = await GetLocalClientIp(cancellationAssertion);

            Logger.LogDiagnostic($"Joining room {roomId}... DONE!", "NetworkClient");

            return ip;
        }

        public async Task LeaveRoom(bool waitIfWasConnected = false)
        {
            if (!Initialized) throw new Exception("Not initialized! Call .Init() on client instance after constructor.");

            Logger.LogDiagnostic($"Leaving room...", "NetworkClient");

            CurrentRoomId = "";

            await Task.Run(() =>
            {
                bool wasInAnyRoom = false;

                foreach (string room in GetAllCurrentRooms())
                {
                    wasInAnyRoom = true;
                    Process.Start(new ProcessStartInfo("cmd", @$"/C ""{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ZeroTier", "One")}\zerotier-one_x64.exe"" -q leave {room}") { CreateNoWindow = true })?.WaitForExit();
                }

                if (wasInAnyRoom && waitIfWasConnected)
                {
                    Thread.Sleep(6000);
                }

                if (wasInAnyRoom)
                {
                    Logger.LogDiagnostic($"Leaving room... DONE!", "NetworkClient");
                }
                else
                {
                    Logger.LogDiagnostic($"Nothing to leave, not currently in a room...", "NetworkClient");
                }
            });
        }

        public async Task<string> GetLocalClientIp(Action cancellationAssertion = null)
        {
            if (!Initialized) throw new Exception("Not initialized! Call .Init() on client instance after constructor.");

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
                catch (Exception ex)
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
            if (!Initialized) throw new Exception("Not initialized! Call .Init() on client instance after constructor.");

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
            catch (Exception ex)
            {

            }

            return finalResult;
        }
    }
}