using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
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

            ConfigManager.Load();

            GameDataManager.SetPlayerSettings(MapId, Army, Username, PlayerColor);

            await LaunchGame();

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

            ConfigManager.Load();

            GameDataManager.SetPlayerSettings(MapId, Army, Username, PlayerColor);

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
            Logger.LogDiagnostic("Waiting for Victory screen...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsVictoriousTitleVisible())
                    {
                        break;
                    }

                    Thread.Sleep(800);
                }
            });

            Logger.LogDiagnostic("Waiting for Victory screen... DONE!", "Bfme1Client");
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
            Logger.LogDiagnostic("Waiting for Menu1...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsMenu1Visible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(1100));

            Logger.LogDiagnostic("Waiting for Menu1... DONE!", "Bfme1Client");
        }

        private async Task WaitForMenu2()
        {
            Logger.LogDiagnostic("Waiting for Menu2...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsMenu2Visible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(1100));

            Logger.LogDiagnostic("Waiting for Menu2... DONE!", "Bfme1Client");
        }

        private async Task WaitForNetworkMenu()
        {
            Logger.LogDiagnostic("Waiting for NetworkMenu...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsMenuCustomGameLobbyVisible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(1100));

            Logger.LogDiagnostic("Waiting for NetworkMenu... DONE!", "Bfme1Client");
        }

        private async Task WaitForIngameNetworkJoined()
        {
            Logger.LogDiagnostic("Waiting for ingame network to join...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsInLobby())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });

            await Task.Run(() => Thread.Sleep(500));

            Logger.LogDiagnostic("Waiting for ingame network to join... DONE!", "Bfme1Client");
        }

        private async Task GoToMultiplayerMenu()
        {
            Logger.LogDiagnostic("Going to multiplayer menu...", "Bfme1Client");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task GoToNetworkMenu()
        {
            Logger.LogDiagnostic("Going to network menu...", "Bfme1Client");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonNetwork"));
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task CreateGame()
        {
            Logger.LogDiagnostic("Creating ingame room...", "Bfme1Client");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"), 1000);
                Thread.Sleep(100);
            });
        }

        private async Task JoinGame()
        {
            Logger.LogDiagnostic("Joining ingame room...", "Bfme1Client");

            await Task.Run(() =>
            {
                while (true)
                {
                    CancelationAssertion?.Invoke();

                    if (ScreenReader.IsInLobby())
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

        private async Task SelectTeam()
        {
            Logger.LogDiagnostic("Selecting team...", "Bfme1Client");

            await Task.Run(() =>
            {
                int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen();

                for (int i = 0; i <= 5; i++)
                {
                    InputHelper.Click(new Point(0, 0), 20);

                    Thread.Sleep(50);

                    InputHelper.Click(new Point(ConfigManager.GetPosFromConfig("TeamButton1").X, playerYLocationOnScreen + 15), 20);

                    Thread.Sleep(50);

                    switch (Team)
                    {
                        case 1:
                            InputHelper.Click(ConfigManager.GetPosFromConfig("TeamButton1", new Point(0, playerYLocationOnScreen)), 20);
                            break;
                        case 2:
                            InputHelper.Click(ConfigManager.GetPosFromConfig("TeamButton2", new Point(0, playerYLocationOnScreen)), 20);
                            break;
                        case 3:
                            InputHelper.Click(ConfigManager.GetPosFromConfig("TeamButton3", new Point(0, playerYLocationOnScreen)), 20);
                            break;
                        case 4:
                            InputHelper.Click(ConfigManager.GetPosFromConfig("TeamButton4", new Point(0, playerYLocationOnScreen)), 20);
                            break;
                    }

                    Thread.Sleep(50);
                }
            });

            Logger.LogDiagnostic("Selecting team... DONE!", "Bfme1Client");
        }

        private async Task SelectSpot()
        {
            Logger.LogDiagnostic("Selecting spot...", "Bfme1Client");

            await Task.Run(() =>
            {
                Point c = ConfigManager.GetMapSpotFromConfig(MapId, Spot);

                if (!IsHost)
                {
                    c = new Point(c.X + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").X, c.Y + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").Y);
                }

                InputHelper.Click(c);
            });

            await Task.Run(() => Thread.Sleep(500));

            Logger.LogDiagnostic("Selecting spot... DONE!", "Bfme1Client");
        }

        private async Task LaunchGame(bool windowed = false)
        {
            await CloseGame();

            if (!Directory.Exists(GameDataManager.GetGameInstallDirectory()))
            {
                throw new Exception();
            }

            Logger.LogDiagnostic("Launching game...", "Bfme1Client");

            string[] files = Directory.GetFiles(GameDataManager.GetGameInstallDirectory());

            foreach (string file in files.Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
            {
                File.Move(file, Path.Combine(GameDataManager.GetGameInstallDirectory(), Path.GetFileNameWithoutExtension(file) + ".inv"), true);
            }

            await Task.Run(() =>
            {
                var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{GameDataManager.GetGameInstallDirectory() + @"\game.dat"}\" name=\"The Battle for Middle-earth - 1.09 Launcher\" mode=ENABLE", CreateNoWindow = true });
                proc?.WaitForExit();
            });

            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(GameDataManager.GetGameInstallDirectory(), GameExecutableName));
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

                Size curentResolution = GameDataManager.GetCurentResolution();
                Rectangle screen = ScreenReader.GetPrimaryScreenBounds();
                SetWindowPos(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, IntPtr.Zero, screen.X, screen.Y, curentResolution.Width, curentResolution.Height, SWP_NOZORDER);
            }

            Logger.LogDiagnostic("Launching game... DONE!", "Bfme1Client");
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
