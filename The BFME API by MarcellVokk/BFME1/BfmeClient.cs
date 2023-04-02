using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class BfmeClient
    {
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

        public static void LockInput()
        {
            InputHelper.LockInput();
        }

        public static void UnlockInput()
        {
            InputHelper.UnlockInput();
        }

        public static bool IsGameFocused()
        {
            const int nChar = 256;
            StringBuilder sb = new StringBuilder(nChar);

            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();

            if (GetWindowText(handle, sb, nChar) > 0) return sb.ToString() == "Lord of the Rings The Battle for Middle-earth";
            return false;
        }

        public static async void StartGame(Action cancelationAssertion)
        {
            LockInput();

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
                        cancelationAssertion();

                        if (ComputerVisionManager.IsStartFailedTextMessageVisible())
                        {
                            Thread.Sleep(1000);
                            StartGame(cancelationAssertion);
                            return;
                        }

                        Thread.Sleep(100);
                    }

                    UnlockInput();
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

        public static async Task WaitForGameStarted(Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        cancelationAssertion();

                        if (ComputerVisionManager.GetGameStartedVisibility())
                        {
                            return;
                        }

                        Thread.Sleep(1000);
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

        public static async Task<int> GetArmy(int sampleDurationSeconds, int sampleFrequencyMiliseconds, Action cancelationAssertion)
        {
            return await Task.Run(() =>
            {
                List<int> detectedArmies = new List<int>();
                Stopwatch sampleArmyTimer = Stopwatch.StartNew();

                while (sampleArmyTimer.Elapsed.TotalSeconds < sampleDurationSeconds)
                {
                    while (!IsGameFocused())
                    {
                        if (sampleArmyTimer.IsRunning)
                        {
                            sampleArmyTimer.Stop();
                        }

                        cancelationAssertion();

                        Thread.Sleep(500);
                    }

                    if (!sampleArmyTimer.IsRunning)
                    {
                        sampleArmyTimer.Start();
                    }

                    detectedArmies.Add(ComputerVisionManager.GetVisibleArmy());

                    cancelationAssertion();

                    Thread.Sleep(sampleFrequencyMiliseconds);

                    cancelationAssertion();
                }

                int detectedArmy = 0;

                if (detectedArmies.All(x => x == 4))
                {
                    detectedArmy = 4;
                }
                else
                {
                    detectedArmy = detectedArmies.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First(x => x != 4);
                }

                return detectedArmy;
            });
        }

        public static async Task WaitForMenu1(Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsMenu1Visible())
                    {
                        return;
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public static async Task WaitForMenu2(Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsMenu2Visible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });
        }

        public static async Task WaitForCustomGameLobbyMenu(Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsMenuCustomGameLobbyVisible())
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });
        }

        public static async Task WaitForIngameNetwork(bool isHost, Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsInLobby(isHost))
                    {
                        return;
                    }

                    Thread.Sleep(200);
                }
            });
        }

        public static async Task GoToMultiplayerMenu()
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

        public static async Task GoToNetworkMenu()
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

        public static async Task CreateGame()
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"), 1000);
                Thread.Sleep(100);
            });
        }

        public static async Task JoinGame(Action cancelationAssertion)
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsJoinFailedPopupVisible())
                    {
                        throw new TaskCanceledException();
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

        public static async Task DismissJoinFailedPopup()
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

        public static async Task SelectTeam(int teamId, int playerYLocationOnScreen)
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();

                for(int i = 0; i <= 5; i++)
                {
                    InputHelper.Click(new Point(0, 0), 20);

                    Thread.Sleep(50);

                    InputHelper.Click(new Point(ConfigManager.GetIntFromConfig("ButtonTeamBaseX"), playerYLocationOnScreen + 15), 20); //ButtonTeamBaseX

                    Thread.Sleep(50);

                    switch (teamId)
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

        public static async Task SelectSpot(string mapId, int spotId, bool isHost)
        {
            await Task.Run(() =>
            {
                ConfigManager.ReloadConfig();
                ConfigManager.ReloadMapSpotConfig();

                Point c = ConfigManager.GetMapSpotFromConfig(mapId, spotId);

                if (!isHost)
                {
                    c = new Point(c.X + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").X, c.Y + ConfigManager.GetPosFromConfig("NonHostMapSpotOffset").Y);
                }

                InputHelper.Click(c);
            });
        }

        public static async Task<bool> WaitForGameResult(Action cancelationAssertion)
        {
            bool result = false;

            await Task.Run(() =>
            {
                while (true)
                {
                    cancelationAssertion();

                    if (ComputerVisionManager.IsVictoriousTitleVisible())
                    {
                        result = true;
                        break;
                    }

                    Thread.Sleep(800);
                }
            });

            return result;
        }

        public static async Task LaunchGame(bool windowed = false)
        {
            await CloseGame();

            try
            {
                if (!Directory.Exists(LauncherInstallManagger.GameDirectory))
                {
                    return;
                }

                string[] files = Directory.GetFiles(LauncherInstallManagger.GameDirectory);

                foreach (string file in files.Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
                {
                    File.Move(file, Path.Combine(LauncherInstallManagger.GameDirectory, Path.GetFileNameWithoutExtension(file) + ".inv"), true);
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                await Task.Run(() =>
                {
                    var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{LauncherInstallManagger.GameDirectory + @"\game.dat"}\" name=\"The Battle for Middle-earth - 1.09 Launcher\" mode=ENABLE", CreateNoWindow = true });
                    proc?.WaitForExit();
                });

                ProcessStartInfo info = new ProcessStartInfo(LauncherInstallManagger.GameDirectory + @"\lotrbfme_game.exe");
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
            catch (Exception ex)
            {
                
            }
        }

        public static async Task CloseGame()
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
    }
}
