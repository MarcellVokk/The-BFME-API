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
            Logger.LogDiagnostic("Waiting for Menu1...", "BfmeRotWKClient");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenu1Visible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(1000);

            Logger.LogDiagnostic("Waiting for Menu1... DONE!", "BfmeRotWKClient");
        }

        private void WaitForNetworkMenu()
        {
            Logger.LogDiagnostic("Waiting for NetworkMenu...", "BfmeRotWKClient");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenuCustomGameLobbyVisible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for NetworkMenu... DONE!", "BfmeRotWKClient");
        }

        private void WaitForIngameNetworkJoined()
        {
            Logger.LogDiagnostic("Waiting for ingame network to join...", "BfmeRotWKClient");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for ingame network to join... DONE!", "BfmeRotWKClient");
        }

        private void GoToMultiplayerMenu()
        {
            Logger.LogDiagnostic("Going to multiplayer menu...", "BfmeRotWKClient");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
        }

        private void GoToNetworkMenu()
        {
            Logger.LogDiagnostic("Going to network menu...", "BfmeRotWKClient");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonNetwork"));
        }

        private void CreateGame()
        {
            Logger.LogDiagnostic("Creating ingame room...", "BfmeRotWKClient");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"));
        }

        private void JoinGame()
        {
            Logger.LogDiagnostic("Joining ingame room...", "BfmeRotWKClient");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                InputHelper.DoubleClick(ConfigManager.GetPosFromConfig("CurentGamesFirstItem"));

                Thread.Sleep(40);
            }

            Logger.LogDiagnostic("Joining ingame room... DONE!", "BfmeRotWKClient");
        }

        private void SelectTeam()
        {
            Logger.LogDiagnostic("Selecting team...", "BfmeRotWKClient");

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

            Logger.LogDiagnostic("Selecting team... DONE!", "BfmeRotWKClient");
        }

        private void GoToMap()
        {
            Logger.LogDiagnostic("Going to map...", "BfmeRotWKClient");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMap"));
        }

        private void SelectSpot()
        {
            Logger.LogDiagnostic("Selecting spot...", "BfmeRotWKClient");

            Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");

            List<Rectangle> spots = SpotDetectionEngine.GetMapSpots(ScreenReader.GrabScreen(), new Rectangle(ingameMapTopLeft, new Size(ingameMapSize.X, ingameMapSize.Y)));
            InputHelper.Click(new Point(spots[SpotIndex].X + spots[SpotIndex].Width / 2, spots[SpotIndex].Y + spots[SpotIndex].Height / 2));

            Logger.LogDiagnostic("Selecting spot... DONE!", "BfmeRotWKClient");
        }

        private void ReadyUp()
        {
            Logger.LogDiagnostic("Readying up...", "BfmeRotWKClient");

            int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen(!IsHost);
            InputHelper.Click(new Point(ConfigManager.GetPosFromConfig("ButtonReady").X + (!IsHost ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0), playerYLocationOnScreen), 200);
        }

        private void LaunchGame()
        {
            if (!Directory.Exists(GameDataManager.GetGameInstallDirectory()))
                throw new Exception("BFME2 is not installed!");

            Logger.LogDiagnostic("Launching game...", "BfmeRotWKClient");

            foreach (string file in Directory.GetFiles(GameDataManager.GetGameInstallDirectory()).Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
                File.Move(file, Path.Combine(GameDataManager.GetGameInstallDirectory(), Path.GetFileNameWithoutExtension(file) + ".inv"), true);

            var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{GameDataManager.GetGameInstallDirectory() + @"\game.dat"}\" name=\"The Battle for Middle-earth\" mode=ENABLE", CreateNoWindow = true });
            proc?.WaitForExit();

            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(GameDataManager.GetGameInstallDirectory(), GameExecutableName));
            info.ArgumentList.Add("-noshellmap");
            Process.Start(info);

            Logger.LogDiagnostic("Launching game... DONE!", "BfmeRotWKClient");
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
