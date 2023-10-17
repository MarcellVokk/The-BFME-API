using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using The_BFME_API.BFME_Shared;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME2
{
    public class Bfme2Client
    {
        public bool IsHost { get; private set; } = false;
        public Action? CancelationAssertion;
        public string GameExecutableName = "lotrbfme2.exe";

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

            await CloseGame();

            await Task.Run(() =>
            {
                LaunchGame();

                WaitForMenu1();
                GoToMultiplayerMenu();
                Thread.Sleep(100);
                GoToNetworkMenu();

                WaitForNetworkMenu();
                CreateGame();

                WaitForIngameNetworkJoined();
                SelectTeam();
                SelectSpot();

                Thread.Sleep(2000);
            });
        }

        public async Task LaunchAsOffhost(Action? waitForHost = null)
        {
            IsHost = false;

            GameDataManager.SetLaunchSettings(MapId, (int)Army, (int)Hero, Username, (int)PlayerColor, 0, 0, true, true);

            await CloseGame();

            await Task.Run(() =>
            {
                LaunchGame();

                if (waitForHost != null) waitForHost.Invoke();

                WaitForMenu1();
                GoToMultiplayerMenu();
                Thread.Sleep(100);
                GoToNetworkMenu();

                WaitForNetworkMenu();
                JoinGame();

                WaitForIngameNetworkJoined();
                SelectTeam();

                GoToMap();
                SelectSpot();

                Thread.Sleep(2000);

                ReadyUp();

                Thread.Sleep(1000);
            });
        }

        public void StartGame()
        {
            if (!IsHost)
                return;

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonStartGame"));
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

        private void WaitForMenu1()
        {
            Logger.LogDiagnostic("Waiting for Menu1...", "Bfme2Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenu1Visible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(1000);

            Logger.LogDiagnostic("Waiting for Menu1... DONE!", "Bfme2Client");
        }

        private void WaitForNetworkMenu()
        {
            Logger.LogDiagnostic("Waiting for NetworkMenu...", "Bfme2Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenuCustomGameLobbyVisible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for NetworkMenu... DONE!", "Bfme2Client");
        }

        private void WaitForIngameNetworkJoined()
        {
            Logger.LogDiagnostic("Waiting for ingame network to join...", "Bfme2Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for ingame network to join... DONE!", "Bfme2Client");
        }

        private void GoToMultiplayerMenu()
        {
            Logger.LogDiagnostic("Going to multiplayer menu...", "Bfme2Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
        }

        private void GoToNetworkMenu()
        {
            Logger.LogDiagnostic("Going to network menu...", "Bfme2Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonNetwork"));
        }

        private void CreateGame()
        {
            Logger.LogDiagnostic("Creating ingame room...", "Bfme2Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"));
        }

        private void JoinGame()
        {
            Logger.LogDiagnostic("Joining ingame room...", "Bfme2Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                InputHelper.DoubleClick(ConfigManager.GetPosFromConfig("CurentGamesFirstItem"));

                Thread.Sleep(40);
            }

            Logger.LogDiagnostic("Joining ingame room... DONE!", "Bfme2Client");
        }

        private void SelectTeam()
        {
            Logger.LogDiagnostic("Selecting team...", "Bfme2Client");

            var teamButtonXPosAndHeight = ConfigManager.GetPosFromConfig("TeamButtonXAndSize");
            teamButtonXPosAndHeight.Offset(new Point(!IsHost ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0, 0));

            int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen(!IsHost);
            Tuple<int, int> armyDropdownHeightAndMinYPos = new Tuple<int, int>(0, 0);

            for (int i = 0; i <= 5; i++)
            {
                InputHelper.Click(new Point(0, 0), 20);
                Thread.Sleep(50);
                InputHelper.Click(new Point(teamButtonXPosAndHeight.X, playerYLocationOnScreen), 20);
                Thread.Sleep(50);

                if (i == 0)
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

            Logger.LogDiagnostic("Selecting team... DONE!", "Bfme2Client");
        }

        private void GoToMap()
        {
            Logger.LogDiagnostic("Going to map...", "Bfme2Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMap"));
        }

        private void SelectSpot()
        {
            Logger.LogDiagnostic("Selecting spot...", "Bfme2Client");

            Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");

            List<Rectangle> spots = SpotDetectionEngine.GetMapSpots(ScreenReader.GrabScreen(), new Rectangle(ingameMapTopLeft, new Size(ingameMapSize.X, ingameMapSize.Y)));
            InputHelper.Click(new Point(spots[SpotIndex].X + spots[SpotIndex].Width / 2, spots[SpotIndex].Y + spots[SpotIndex].Height / 2));

            Logger.LogDiagnostic("Selecting spot... DONE!", "Bfme2Client");
        }

        private void ReadyUp()
        {
            Logger.LogDiagnostic("Readying up...", "Bfme2Client");

            int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen(!IsHost);
            InputHelper.Click(new Point(ConfigManager.GetPosFromConfig("ButtonReady").X + (!IsHost ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0), playerYLocationOnScreen), 200);
        }

        private void LaunchGame()
        {
            if (!Directory.Exists(GameDataManager.GetGameInstallDirectory()))
                throw new Exception("BFME2 is not installed!");

            Logger.LogDiagnostic("Launching game...", "Bfme2Client");

            foreach (string file in Directory.GetFiles(GameDataManager.GetGameInstallDirectory()).Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
                File.Move(file, Path.Combine(GameDataManager.GetGameInstallDirectory(), Path.GetFileNameWithoutExtension(file) + ".inv"), true);

            var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{GameDataManager.GetGameInstallDirectory() + @"\game.dat"}\" name=\"The Battle for Middle-earth\" mode=ENABLE", CreateNoWindow = true });
            proc?.WaitForExit();

            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(GameDataManager.GetGameInstallDirectory(), GameExecutableName));
            info.ArgumentList.Add("-noshellmap");
            Process.Start(info);

            Logger.LogDiagnostic("Launching game... DONE!", "Bfme2Client");
        }
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
        Goblins = 9
    }

    public enum PlayerHero
    {
        Random = -2,
        None = -1,
        Fhaleen = 0,
        Morven = 1,
        Thrugg = 2,
        Krashnak = 3,
        Idrial = 4,
        Berethor = 5,
        Hadhod = 6,
    }

    public enum PlayerTeam
    {
        Team1 = 1,
        Team2 = 2,
        Team3 = 3,
        Team4 = 4,
    }
}
