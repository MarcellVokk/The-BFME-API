using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using The_BFME_API.BFME_Shared;
using The_BFME_API.Logging;

namespace The_BFME_API.BFMERotWK
{
    public class BfmeRotWKClient
    {
        public bool IsHost { get; private set; } = false;
        public Action? CancelationAssertion;
        public string GameExecutableName = "lotrbfme2ep1.exe";

        public string Username = "";
        public PlayerColor PlayerColor = PlayerColor.Random;
        public string MapId = "";
        public PlayerArmy Army = PlayerArmy.Random;
        public PlayerHero Hero = PlayerHero.None;
        public PlayerTeam Team = PlayerTeam.Team1;
        public int SpotIndex = 1;

        public async Task LaunchAsHost(int initialResources = 1000, int commandPointFactor = 100, bool allowCustomHeroes = false, bool allowRingHeroes = false)
        {
            IsHost = true;

            GameDataManager.SetLaunchSettings(MapId, (int)Army, (int)Hero, Username, (int)PlayerColor, commandPointFactor, initialResources, allowCustomHeroes, allowRingHeroes);

            await LaunchGame();

            await WaitForMenu1();
            await GoToMultiplayerMenu();
            await GoToNetworkMenu();

            await WaitForNetworkMenu();
            await CreateGame();

            await WaitForIngameNetworkJoined();
            await SelectTeam();
            await SelectSpot();
        }

        public async Task LaunchAsOffhost(Action? waitForHost = null)
        {
            IsHost = false;

            GameDataManager.SetLaunchSettings(MapId, (int)Army, (int)Hero, Username, (int)PlayerColor, 0, 0, true, true);

            await LaunchGame();

            if (waitForHost != null) await Task.Run(() => waitForHost.Invoke());

            await WaitForMenu1();
            await GoToMultiplayerMenu();
            await GoToNetworkMenu();

            await WaitForNetworkMenu();
            await JoinGame();

            await WaitForIngameNetworkJoined();
            await SelectTeam();

            await GoToMap();
            await SelectSpot();

            await ReadyUp();
        }

        public async void StartGame()
        {
            if (!IsHost)
            {
                Logger.LogDiagnostic("Tried to start the game while not host...", "BfmeRotWKClient");
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
            Logger.LogDiagnostic("Waiting for Menu1...", "BfmeRotWKClient");

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

            Logger.LogDiagnostic("Waiting for Menu1... DONE!", "BfmeRotWKClient");
        }

        private async Task WaitForNetworkMenu()
        {
            Logger.LogDiagnostic("Waiting for NetworkMenu...", "BfmeRotWKClient");

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

            await Task.Run(() => Thread.Sleep(200));

            Logger.LogDiagnostic("Waiting for NetworkMenu... DONE!", "BfmeRotWKClient");
        }

        private async Task WaitForIngameNetworkJoined()
        {
            Logger.LogDiagnostic("Waiting for ingame network to join...", "BfmeRotWKClient");

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

            Logger.LogDiagnostic("Waiting for ingame network to join... DONE!", "BfmeRotWKClient");
        }

        private async Task GoToMultiplayerMenu()
        {
            Logger.LogDiagnostic("Going to multiplayer menu...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
                Thread.Sleep(100);
            });
        }

        private async Task GoToNetworkMenu()
        {
            Logger.LogDiagnostic("Going to network menu...", "BfmeRotWKClient");

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
            Logger.LogDiagnostic("Creating ingame room...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"), 1000);
                Thread.Sleep(100);
            });
        }

        private async Task JoinGame()
        {
            Logger.LogDiagnostic("Joining ingame room...", "BfmeRotWKClient");

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
            Logger.LogDiagnostic("Selecting team...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen(!IsHost);
                Tuple<int, int> armyDropdownHeightAndMinYPos = new Tuple<int, int>(0, 0);

                for (int i = 0; i <= 5; i++)
                {
                    var teamButtonXPosAndHeight = ConfigManager.GetPosFromConfig("TeamButtonXAndSize");
                    teamButtonXPosAndHeight.Offset(new Point(!IsHost ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0, 0));

                    InputHelper.Click(new Point(0, 0), 20);

                    Thread.Sleep(50);

                    InputHelper.Click(new Point(teamButtonXPosAndHeight.X, playerYLocationOnScreen), 20);

                    Thread.Sleep(50);

                    if(i == 0)
                    {
                        Thread.Sleep(200);

                        armyDropdownHeightAndMinYPos = ScreenReader.GetArmyDropdownHeightAndMinYPos(!IsHost);
                    }

                    switch (Team)
                    {
                        case PlayerTeam.Team1:
                            InputHelper.Click(new Point(teamButtonXPosAndHeight.X, armyDropdownHeightAndMinYPos.Item2 + (int)(armyDropdownHeightAndMinYPos.Item1 / 5d * 1.5d)), 20);
                            break;
                        case PlayerTeam.Team2:
                            InputHelper.Click(new Point(teamButtonXPosAndHeight.X, armyDropdownHeightAndMinYPos.Item2 + (int)(armyDropdownHeightAndMinYPos.Item1 / 5d * 2.5d)), 20);
                            break;
                        case PlayerTeam.Team3:
                            InputHelper.Click(new Point(teamButtonXPosAndHeight.X, armyDropdownHeightAndMinYPos.Item2 + (int)(armyDropdownHeightAndMinYPos.Item1 / 5d * 3.5d)), 20);
                            break;
                        case PlayerTeam.Team4:
                            InputHelper.Click(new Point(teamButtonXPosAndHeight.X, armyDropdownHeightAndMinYPos.Item2 + (int)(armyDropdownHeightAndMinYPos.Item1 / 5d * 4.5d)), 20);
                            break;
                    }

                    Thread.Sleep(50);
                }
            });

            Logger.LogDiagnostic("Selecting team... DONE!", "BfmeRotWKClient");
        }

        private async Task GoToMap()
        {
            Logger.LogDiagnostic("Going to map...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMap"));
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task SelectSpot()
        {
            Logger.LogDiagnostic("Selecting spot...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                Thread.Sleep(300);

                Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
                Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");
                List<Rectangle> spots = SpotDetectionEngine.GetMapSpots(ScreenReader.GrabScreen(), new Rectangle(ingameMapTopLeft, new Size(ingameMapSize.X, ingameMapSize.Y)));

                Point c = new Point(spots[SpotIndex].X + spots[SpotIndex].Width / 2, spots[SpotIndex].Y + spots[SpotIndex].Height / 2);

                InputHelper.Click(c);
            });

            await Task.Run(() => Thread.Sleep(500));

            Logger.LogDiagnostic("Selecting spot... DONE!", "BfmeRotWKClient");
        }

        private async Task ReadyUp()
        {
            Logger.LogDiagnostic("Readying up...", "BfmeRotWKClient");

            await Task.Run(() =>
            {
                Thread.Sleep(500);

                int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen(!IsHost);
                InputHelper.Click(new Point(ConfigManager.GetPosFromConfig("ButtonReady").X + (!IsHost ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0), playerYLocationOnScreen), 200);
                Thread.Sleep(100);
                InputHelper.SetMousePos(new Point(0, 0));
                Thread.Sleep(100);
            });
        }

        private async Task LaunchGame(bool windowed = false)
        {
            await CloseGame();

            if (!Directory.Exists(GameDataManager.GetGameInstallDirectory()))
            {
                throw new Exception();
            }

            Logger.LogDiagnostic("Launching game...", "BfmeRotWKClient");

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

                SetWindowLong(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, GWL_STYLE, (IntPtr)currentstyle);

                Size curentResolution = GameDataManager.GetCurentResolution();
                Rectangle screen = ScreenReader.GetPrimaryScreenBounds();
                SetWindowPos(Process.GetProcessesByName("game.dat")[0].MainWindowHandle, IntPtr.Zero, screen.X, screen.Y, curentResolution.Width, curentResolution.Height, SWP_NOZORDER);
            }

            Logger.LogDiagnostic("Launching game... DONE!", "BfmeRotWKClient");
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

    public enum PlayerColor
    {
        Random = -1,
        Blue = 0,
        Red = 1,
        LightGreen = 2,
        Green = 3,
        Orange = 4,
        LightBlue = 5,
        Purple = 6,
        Pink = 7,
        Gray = 8,
        White = 9
    }

    public enum PlayerArmy
    {
        Random = -1,
        Men = 3,
        Elves = 5,
        Dwarves = 6,
        Isengard = 7,
        Mordor = 8,
        Goblins = 9,
        Angmar = 10
    }

    public enum PlayerHero
    {
        Random = -2,
        None = -1,
        Fhaleen = 0,
        Morven = 1,
        Alcarin = 2,
        Thrugg = 3,
        Krashnak = 4,
        Idrial = 5,
        Berethor = 6,
        Hadhod = 7
    }

    public enum PlayerTeam
    {
        Team1 = 1,
        Team2 = 2,
        Team3 = 3,
        Team4 = 4,
    }
}
