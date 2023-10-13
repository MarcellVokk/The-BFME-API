using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using The_BFME_API.Logging;

namespace The_BFME_API.Network
{
    public class NetworkManagement
    {
        public static string API_KEY = "MmTSN1ZxHweXlKz5gOhFYQdl0jBpAyJO";

        public static void Init(string apiKey)
        {
            API_KEY = apiKey;
        }

        public static async Task<string> OpenRoom()
        {
            Logger.LogDiagnostic("Opening room...", "NetworkManagement");

            string roomId = "";

            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var url = "https://api.zerotier.com/api/v1/network";

                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpRequest.Method = "POST";

                        httpRequest.Headers["Authorization"] = $"Bearer {API_KEY}";
                        httpRequest.ContentType = "application/json";

                        var data = "{ }";

                        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                        {
                            streamWriter.Write(data);
                        }

                        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            dynamic response = JObject.Parse(streamReader.ReadToEnd());

                            roomId = response.id;
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDiagnostic("Failed to open room... This can hapen, trying again in 2 seconds!", "NetworkManagement");

                        Thread.Sleep(2000);
                    }
                }
            });

            Logger.LogDiagnostic("Opening room... DONE!", "NetworkManagement");

            await ConfigureRoom(roomId);

            return roomId;
        }

        public static async Task ConfigureRoom(string roomId)
        {
            Logger.LogDiagnostic("Configuring room...", "NetworkManagement");

            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var url = $"https://api.zerotier.com/api/v1/network/{roomId}";

                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpRequest.Method = "POST";

                        httpRequest.Headers["Authorization"] = $"Bearer {API_KEY}";
                        httpRequest.ContentType = "application/json";

                        var data = @"{
    ""type"": ""Network"",
    ""config"": {
        ""authTokens"": null,
        ""capabilities"": [],
        ""enableBroadcast"": true,
        ""ipAssignmentPools"": [{
            ""ipRangeStart"": ""10.0.0.0"",
            ""ipRangeEnd"": ""10.0.0.25""
        }],
        ""lastModified"": 0,
        ""mtu"": 2800,
        ""multicastLimit"": 32,
        ""name"": """",
        ""private"": false,
        ""remoteTraceLevel"": 0,
        ""remoteTraceTarget"": null,
        ""routes"": [{
            ""target"": ""10.0.0.0/25""
        }],
        ""rules"": [{
            ""etherType"": 2048,
            ""not"": true,
            ""or"": false,
            ""type"": ""MATCH_ETHERTYPE""
        }, {
            ""etherType"": 2054,
            ""not"": true,
            ""or"": false,
            ""type"": ""MATCH_ETHERTYPE""
        }, {
            ""etherType"": 34525,
            ""not"": true,
            ""or"": false,
            ""type"": ""MATCH_ETHERTYPE""
        }, {
            ""type"": ""ACTION_DROP""
        }, {
            ""type"": ""ACTION_ACCEPT""
        }],
        ""tags"": [],
        ""v4AssignMode"": {
            ""zt"": true
        },
        ""v6AssignMode"": {
            ""6plane"": false,
            ""rfc4193"": false,
            ""zt"": false
        },
        ""dns"": {
            ""domain"": """",
            ""servers"": null
        },
        ""ssoConfig"": {
            ""enabled"": false,
            ""mode"": 0
        }
    },
    ""description"": """",
    ""rulesSource"": ""#\n# This is a default rule set that allows IPv4 and IPv6 traffic but otherwise\n# behaves like a standard Ethernet switch.\n#\n# Please keep in mind that ZeroTier versions prior to 1.2.0 do NOT support advanced\n# network rules.\n#\n# Since both senders and receivers enforce rules, you will get the following\n# behavior in a network with both old and new versions:\n#\n# (old: 1.1.14 and older, new: 1.2.0 and newer)\n#\n# old \u003c--\u003e old: No rules are honored.\n# old \u003c--\u003e new: Rules work but are only enforced by new side. Tags will NOT work, and\n#               capabilities will only work if assigned to the new side.\n# new \u003c--\u003e new: Full rules engine support including tags and capabilities.\n#\n# We recommend upgrading all your devices to 1.2.0 as soon as convenient. Version\n# 1.2.0 also includes a significantly improved software update mechanism that is\n# turned on by default on Mac and Windows. (Linux and mobile are typically kept up\n# to date using package/app management.)\n#\n\n#\n# Allow only IPv4, IPv4 ARP, and IPv6 Ethernet frames.\n#\ndrop\n\tnot ethertype ipv4\n\tand not ethertype arp\n\tand not ethertype ipv6\n;\n\n#\n# Uncomment to drop non-ZeroTier issued and managed IP addresses.\n#\n# This prevents IP spoofing but also blocks manual IP management at the OS level and\n# bridging unless special rules to exempt certain hosts or traffic are added before\n# this rule.\n#\n#drop\n#\tnot chr ipauth\n#;\n\n# Accept anything else. This is required since default is 'drop'.\naccept;\n"",
    ""onlineMemberCount"": 0,
    ""authorizedMemberCount"": 0,
    ""totalMemberCount"": 0,
    ""capabilitiesByName"": {},
    ""tagsByName"": {},
    ""ui"": {
        ""membersHelpCollapsed"": true,
        ""rulesHelpCollapsed"": true,
        ""settingsHelpCollapsed"": true,
        ""v4EasyMode"": true
    }
}";

                        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                        {
                            streamWriter.Write(data);
                        }

                        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDiagnostic("Failed to configure room... This can hapen, trying again in 2 seconds!", "NetworkManagement");

                        Thread.Sleep(2000);
                    }
                }
            });

            Logger.LogDiagnostic("Configuring room... DONE!", "NetworkManagement");
        }

        public static async Task CloseRoom(string roomId)
        {
            Logger.LogDiagnostic("Closing room...", "NetworkManagement");

            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var url = $"https://api.zerotier.com/api/v1/network/{roomId}";

                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpRequest.Method = "DELETE";

                        httpRequest.Headers["Authorization"] = $"Bearer {API_KEY}";

                        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDiagnostic("Failed to close room... This can hapen, trying again in 2 seconds!", "NetworkManagement");

                        Thread.Sleep(2000);
                    }
                }
            });

            Logger.LogDiagnostic("Closing room... DONE!", "NetworkManagement");
        }
    }
}
