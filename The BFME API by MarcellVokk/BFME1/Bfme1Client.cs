using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using The_BFME_API_by_MarcellVokk.Logging;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public class Bfme1Client
    {
        public bool IsHost { get; private set; } = false;
        public Action? CancelationAssertion;
        public string GameExecutableName = "lotrbfme.exe";

        public string Username = "";
        public int PlayerColor = 1;
        public string MapId = "";
        public int Team = 1;
        public int Spot = 1;
        public int Army = 1;

        public async Task LaunchAsHost()
        {
            IsHost = true;

            ConfigManager.ReloadConfig();
            ConfigManager.ReloadMapSpotConfig();

            GameConfigManager.SetPlayerSettings(MapId, Army, Username, PlayerColor);

            await LaunchGame(true);

            await WaitForMenu1();
            await GoToMultiplayerMenu();

            await WaitForMenu2();
            await GoToNetworkMenu();

            await WaitForNetworkMenu();
            await CreateGame();

            await WaitForIngameNetworkJoined();
            await SelectTeam();
            await SelectSpot();
        }

        public async Task LaunchAsOffhost()
        {
            IsHost = false;

            ConfigManager.ReloadConfig();
            ConfigManager.ReloadMapSpotConfig();

            GameConfigManager.SetPlayerSettings(MapId, Army, Username, PlayerColor);

            await LaunchGame();

            await WaitForMenu1();
            await GoToMultiplayerMenu();

            await WaitForMenu2();
            await GoToNetworkMenu();

            await WaitForNetworkMenu();
            await JoinGame();

            await WaitForIngameNetworkJoined();
            await SelectTeam();
            await SelectSpot();
        }

        public async void StartGame()
        {
            if (!IsHost)
            {
                Logger.LogDiagnostic("Tried to start the game while not host...", "Bfme1Client");
            }

            ConfigManager.ReloadConfig();
            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonStartGame"));

            await Task.Run(() => Thread.Sleep(10000));

            Stopwatch gameStartFailTimeout = Stopwatch.StartNew();

            await Task.Run(() =>
            {
                try
                {
                    while (gameStartFailTimeout.Elapsed.TotalSeconds < 5)
                    {
                        CancelationAssertion?.Invoke();

                        if (ComputerVisionManager.IsStartFailedTextMessageVisible())
                        {
                            Thread.Sleep(1000);
                            StartGame();
                            return;
                        }

                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException)
                    {
                        return;
                    }
                }
            });

        }

        public async Task WaitForWinScreen()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsVictoriousTitleVisible())
                    {
                        break;
                    }

                    Thread.Sleep(800);
                }
            });
        }

        public async Task CloseGame()
        {
            await Task.Run(() =>
            {
                bool wasGameRunning = false;

                foreach (Process p in Process.GetProcessesByName("game.dat"))
                {
                    wasGameRunning = true;

                    p.Kill();
                    p.WaitForExit();
                }

                if (wasGameRunning)
                {
                    Thread.Sleep(10000);
                }
            });
        }

        public bool IsGameFocused()
        {
            const int nChar = 256;
            StringBuilder sb = new StringBuilder(nChar);

            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();

            if (GetWindowText(handle, sb, nChar) > 0) return sb.ToString() == "Lord of the Rings The Battle for Middle-earth";
            return false;
        }

        private async Task WaitForMenu1()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsMenu1Visible())
                    {
                        return;
                    }

                    Thread.Sleep(1000);
                }
            });

            await Task.Run(() => Thread.Sleep(500));
        }

        private async Task WaitForMenu2()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsMenu2Visible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(500));
        }

        private async Task WaitForNetworkMenu()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsMenuCustomGameLobbyVisible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(500));
        }

        private async Task WaitForIngameNetworkJoined()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsInLobby(IsHost))
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(500));
        }

        private async Task GoToMultiplayerMenu()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task GoToNetworkMenu()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonNetwork"));
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task CreateGame()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"), 1000);
                Thread.Sleep(100);
            });
        }

        private async Task JoinGame()
        {
            await Task.Run(async () =>
            {
                ConfigManager.ReloadConfig();

                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ComputerVisionManager.IsJoinFailedPopupVisible())
                    {
                        await DismissJoinFailedPopup();
                        await JoinGame();
                        return;
                    }

                    if (ComputerVisionManager.IsInLobby(false))
                    {
                        return;
                    }

                    InputHelper.Click(ConfigManager.GetPosFromConfig("CurentGamesFirstItem"));
                    Thread.Sleep(40);
                    InputHelper.Click(ConfigManager.GetPosFromConfig("CurentGamesFirstItem"));
                    Thread.Sleep(40);
                }
            });
        }

        private async Task DismissJoinFailedPopup()
        {
            await Task.Run(() =>
            {
                System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();

                ConfigManager.ReloadConfig();

                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonJoinFailedOK"));

                Thread.Sleep(1500);

                InputHelper.Click(new Point(curentResolution.Width - 60, curentResolution.Height - 35));

                Thread.Sleep(500);
            });
        }

        private async Task SelectTeam()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                int playerYLocationOnScreen = ComputerVisionManager.GetPlayerYLocationOnScreen();

                for (int i = 0; i <= 5; i++)
                {
                    InputHelper.Click(new Point(0, 0), 20);

                    Thread.Sleep(50);

                    InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + 15), 20); //ButtonTeamBaseX

                    Thread.Sleep(50);

                    switch (Team)
                    {
                        case 1:
                            InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + ConfigManager.GetIntFromConfig("ButtonTeam1OffsetY")), 20); //ButtonTeamBaseX, ButtonTeam1OffsetY
                            break;
                        case 2:
                            InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + ConfigManager.GetIntFromConfig("ButtonTeam2OffsetY")), 20); //ButtonTeamBaseX, ButtonTeam2OffsetY
                            break;
                        case 3:
                            InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + ConfigManager.GetIntFromConfig("ButtonTeam3OffsetY")), 20); //ButtonTeamBaseX, ButtonTeam3OffsetY
                            break;
                        case 4:
                            InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + ConfigManager.GetIntFromConfig("ButtonTeam4OffsetY")), 20); //ButtonTeamBaseX, ButtonTeam4OffsetY
                            break;
                    }

                    Thread.Sleep(50);
                }
            });
        }

        private async Task SelectSpot()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();
                ConfigManager.ReloadMapSpotConfig();

                Point c = ConfigManager.GetMapSpotFromConfig(MapId, Spot);

                if (!IsHost)
                {
                    c = new Point(c.X + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").X, c.Y + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").Y);
                }

                InputHelper.Click(c);
            });

            await Task.Run(() => Thread.Sleep(500));
        }

        private async Task LaunchGame(bool windowed = false)
        {
            await CloseGame();

            if (!Directory.Exists(BaseGameInstallManagger.GameDirectory))
            {
                throw new Exception();
            }

            if (!ResolutionManager.IsResolutionSupported(ResolutionManager.GetResolutionFromSettings()))
            {
                throw new Exception("The curent resolution is not supported...");
            }

            string[] files = Directory.GetFiles(BaseGameInstallManagger.GameDirectory);

            foreach (string file in files.Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
            {
                File.Move(file, Path.Combine(BaseGameInstallManagger.GameDirectory, Path.GetFileNameWithoutExtension(file) + ".inv"), true);
            }

            await Task.Run(() =>
            {
                var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{BaseGameInstallManagger.GameDirectory + @"\game.dat"}\" name=\"The Battle for Middle-earth - 1.09 Launcher\" mode=ENABLE", CreateNoWindow = true });
                proc?.WaitForExit();
            });

            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(BaseGameInstallManagger.GameDirectory, GameExecutableName));
            info.ArgumentList.Add("-noshellmap");

            if (windowed)
            {
                info.ArgumentList.Add("-win");
            }

            Process.Start(info);

            if (windowed)
            {
                await Task.Run(() => Thread.Sleep(4000));

                const uint SWP_NOZORDER = 0x0004;
                const uint WS_BORDER = 0x00800000;
                const uint WS_DLGFRAME = 0x00400000;
                const uint WS_THICKFRAME = 0x00040000;
                const uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
                const uint WS_MINIMIZE = 0x20000000;
                const uint WS_MAXIMIZE = 0x01000000;
                const uint WS_SYSMENU = 0x00080000;
                const int GWL_STYLE = -16;

                uint currentstyle = (uint)GetWindowLong(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, GWL_STYLE);
                uint[] styles = new uint[] { WS_CAPTION, WS_THICKFRAME, WS_MINIMIZE, WS_MAXIMIZE, WS_SYSMENU };

                foreach (uint style in styles)
                {
                    if ((currentstyle & style) != 0)
                    {
                        currentstyle &= ~style;
                    }
                }

                SetWindowLong(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, GWL_STYLE, (IntPtr)(currentstyle));

                System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
                SetWindowPos(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, IntPtr.Zero, Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, curentResolution.Width, curentResolution.Height, SWP_NOZORDER);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder sb, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
    }
}
