using System.Diagnostics;
using System.Drawing;
using The_BFME_API.BFME_Shared;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME1
{
    public class Bfme1Client
    {
        public bool IsHost { get; private set; } = false;
        public Action? CancelationAssertion;
        public string GameExecutableName = "lotrbfme.exe";

        public string Username = "";
        public PlayerColor PlayerColor = PlayerColor.Random;
        public string MapId = "";
        public PlayerArmy Army = PlayerArmy.Random;
        public PlayerTeam Team = PlayerTeam.Team1;
        public int SpotIndex = 1;

        public async Task LaunchAsHost()
        {
            IsHost = true;

            GameDataManager.SetPlayerSettings(MapId, (int)Army, Username, (int)PlayerColor);

            await CloseGame();

            await Task.Run(() =>
            {
                LaunchGame();

                WaitForMenu1();
                GoToMultiplayerMenu();

                WaitForMenu2();
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

            GameDataManager.SetPlayerSettings(MapId, (int)Army, Username, (int)PlayerColor);

            await CloseGame();

            await Task.Run(() =>
            {
                LaunchGame();

                if (waitForHost != null) waitForHost.Invoke();

                WaitForMenu1();
                GoToMultiplayerMenu();

                WaitForMenu2();
                GoToNetworkMenu();

                WaitForNetworkMenu();
                JoinGame();

                WaitForIngameNetworkJoined();
                SelectTeam();
                SelectSpot();

                Thread.Sleep(2000);
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
            Logger.LogDiagnostic("Waiting for Menu1...", "Bfme1Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenu1Visible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(1000);

            Logger.LogDiagnostic("Waiting for Menu1... DONE!", "Bfme1Client");
        }

        private void WaitForMenu2()
        {
            Logger.LogDiagnostic("Waiting for Menu2...", "Bfme1Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenu2Visible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for Menu2... DONE!", "Bfme1Client");
        }

        private void WaitForNetworkMenu()
        {
            Logger.LogDiagnostic("Waiting for NetworkMenu...", "Bfme1Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsMenuCustomGameLobbyVisible())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for NetworkMenu... DONE!", "Bfme1Client");
        }

        private void WaitForIngameNetworkJoined()
        {
            Logger.LogDiagnostic("Waiting for ingame network to join...", "Bfme1Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            Logger.LogDiagnostic("Waiting for ingame network to join... DONE!", "Bfme1Client");
        }

        private void GoToMultiplayerMenu()
        {
            Logger.LogDiagnostic("Going to multiplayer menu...", "Bfme1Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonMultiplayer"));
        }

        private void GoToNetworkMenu()
        {
            Logger.LogDiagnostic("Going to network menu...", "Bfme1Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonNetwork"));
        }

        private void CreateGame()
        {
            Logger.LogDiagnostic("Creating ingame room...", "Bfme1Client");

            InputHelper.Click(ConfigManager.GetPosFromConfig("ButtonCreateGame"));
        }

        private void JoinGame()
        {
            Logger.LogDiagnostic("Joining ingame room...", "Bfme1Client");

            while (true)
            {
                CancelationAssertion?.Invoke();

                if (ScreenReader.IsInLobby())
                    break;

                InputHelper.DoubleClick(ConfigManager.GetPosFromConfig("CurentGamesFirstItem"));

                Thread.Sleep(40);
            }

            Logger.LogDiagnostic("Joining ingame room... DONE!", "Bfme1Client");
        }

        private void SelectTeam()
        {
            Logger.LogDiagnostic("Selecting team...", "Bfme1Client");

            var teamButtonXPosAndHeight = ConfigManager.GetPosFromConfig("TeamButtonXAndSize");
            int playerYLocationOnScreen = ScreenReader.GetPlayerYLocationOnScreen();
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
                    armyDropdownHeightAndMinYPos = ScreenReader.GetArmyDropdownHeightAndMinYPos();
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

            Logger.LogDiagnostic("Selecting team... DONE!", "Bfme1Client");
        }

        private void SelectSpot()
        {
            Logger.LogDiagnostic("Selecting spot...", "Bfme1Client");

            Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");
            if (!IsHost) ingameMapTopLeft.Offset(ConfigManager.GetPosFromConfig("NonHostMapSpotOffset"));

            List<Rectangle> spots = SpotDetectionEngine.GetMapSpots(ScreenReader.GrabScreen(), new Rectangle(ingameMapTopLeft, new Size(ingameMapSize.X, ingameMapSize.Y)));
            InputHelper.Click(new Point(spots[SpotIndex].X + spots[SpotIndex].Width / 2, spots[SpotIndex].Y + spots[SpotIndex].Height / 2));

            Logger.LogDiagnostic("Selecting spot... DONE!", "Bfme1Client");
        }

        private void LaunchGame()
        {
            if (!Directory.Exists(GameDataManager.GetGameInstallDirectory()))
                throw new Exception("BFME1 is not installed!");

            Logger.LogDiagnostic("Launching game...", "Bfme1Client");

            foreach (string file in Directory.GetFiles(GameDataManager.GetGameInstallDirectory()).Where(x => Path.GetFileNameWithoutExtension(x) == "_aptpatch"))
                File.Move(file, Path.Combine(GameDataManager.GetGameInstallDirectory(), Path.GetFileNameWithoutExtension(file) + ".inv"), true);

            var proc = Process.Start(new ProcessStartInfo { FileName = "netsh", Arguments = $"firewall add allowedprogram program=\"{GameDataManager.GetGameInstallDirectory() + @"\game.dat"}\" name=\"The Battle for Middle-earth\" mode=ENABLE", CreateNoWindow = true });
            proc?.WaitForExit();

            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(GameDataManager.GetGameInstallDirectory(), GameExecutableName));
            info.ArgumentList.Add("-noshellmap");
            Process.Start(info);

            Logger.LogDiagnostic("Launching game... DONE!", "Bfme1Client");
        }
    }

    public enum PlayerColor
    {
        Random = -1,
        Green = 0,
        Red = 1,
        Pink = 2,
        Blue = 3,
        LightBlue = 4,
        Lime = 5,
        Turquoise = 6,
        Orange = 7,
        Yellow = 8,
        Purple = 9,
        LightPink = 10,
        Gray = 11,
        White = 12
    }

    public enum PlayerArmy
    {
        Random = -1,
        Rohan = 2,
        Gondor = 3,
        Isengard = 4,
        Mordor = 5
    }

    public enum PlayerTeam
    {
        Team1 = 1,
        Team2 = 2,
        Team3 = 3,
        Team4 = 4,
    }
}
