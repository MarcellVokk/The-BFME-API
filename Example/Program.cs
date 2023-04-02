using System.Diagnostics;
using System.Runtime.InteropServices;
using The_BFME_API_by_MarcellVokk.BFME1;
using The_BFME_API_by_MarcellVokk.Network;

namespace Example
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static async Task Main(string[] args)
        {
            AllocConsole();

            // This function demonstrates the BFME1 client interface portion of the API, and explains how to use it
            await Bfme1ClientDemo();

            // This function demonstrates the networking portion of the API, and explains how to use it
            //await NetworkDemo();

            Console.WriteLine("The program completed successfuly! Press return to exit.");
            Console.ReadLine();

            await BfmeClient.CloseGame();
        }

        static async Task NetworkDemo()
        {
            // Create a new room
            string newRoomId = await NetworkManagement.OpenRoom();

            Console.WriteLine($"Room created successfuly! Room id: {newRoomId}");

            // Create a network client.
            // The network client is what you use to connect the user to rooms
            // IMPORTANT: The app needs to be running under administrator privileges, otherwise creating a NetworkClient object will throw an exception!
            NetworkClient client = new NetworkClient();

            // Join the room we just created
            // When joining the room, you also get the users virtual network ip
            string networkIp = await client.JoinRoom(newRoomId);

            Console.WriteLine($"Joined room successfuly!");

            Console.WriteLine("Press return to continue");
            Console.ReadLine();

            // Leave the room
            await client.LeaveRoom();

            // Close the room we created
            // Please always close rooms you create after you are done using them!
            await NetworkManagement.CloseRoom(newRoomId);
        }

        static async Task Bfme1ClientDemo()
        {
            // Reload resolution configuration and map configuration. Do this every time before using this part of the API!
            ConfigManager.ReloadConfig();
            ConfigManager.ReloadMapSpotConfig();

            // Set the map, army, username, and color of the local player. If the player is host, this map will be used when creating the game.
            GameConfigManager.SetStartupNetworkSetup("maps_5Cmap_20mp_20adorn_20river_5Cmap_20mp_20adorn_20river_2Emap", 2, "Test", 14);

            Console.WriteLine($"Launching game...");

            try
            {
                // Launch BFME
                await BfmeClient.LaunchGame();

                // Wait for the first menu to apear, then go to the multiplayer menu
                await BfmeClient.WaitForMenu1(CancellationAssertion);
                await Task.Run(() => Thread.Sleep(500));
                await BfmeClient.GoToMultiplayerMenu();

                // Wait for the second menu to apear, then go to the network menu
                await BfmeClient.WaitForMenu2(CancellationAssertion);
                await Task.Run(() => Thread.Sleep(500));
                await BfmeClient.GoToNetworkMenu();

                // Wait for the network menu to apear
                await BfmeClient.WaitForCustomGameLobbyMenu(CancellationAssertion);
                await Task.Run(() => Thread.Sleep(500));

                bool isHost = true;
                if (isHost)
                {
                    Console.WriteLine($"Hosting game...");

                    // Host a game
                    await BfmeClient.CreateGame();

                    // Wait while the room gets created
                    await BfmeClient.WaitForIngameNetwork(true, CancellationAssertion);
                    await Task.Run(() => Thread.Sleep(500));

                    Console.WriteLine($"Selecting spot and team...");

                    // Select team and spot
                    await BfmeClient.SelectTeam(2, ComputerVisionManager.GetPlayerYLocationOnScreen());
                    await BfmeClient.SelectSpot("maps_5Cmap_20mp_20adorn_20river_5Cmap_20mp_20adorn_20river_2Emap", 2, true);
                    await Task.Run(() => Thread.Sleep(500));

                    Console.WriteLine($"Starting game...");

                    // Start the game once all players joined
                    BfmeClient.StartGame(CancellationAssertion);
                }
                else
                {
                    Console.WriteLine($"Joining game...");

                    // Join the current game
                    await BfmeClient.JoinGame(CancellationAssertion);

                    // Wait while bfme connects to host
                    await BfmeClient.WaitForIngameNetwork(false, CancellationAssertion);
                    await Task.Run(() => Thread.Sleep(500));

                    Console.WriteLine($"Selecting spot and team...");

                    // Select team and spot
                    await BfmeClient.SelectTeam(2, ComputerVisionManager.GetPlayerYLocationOnScreen());
                    await BfmeClient.SelectSpot("maps_5Cmap_20mp_20adorn_20river_5Cmap_20mp_20adorn_20river_2Emap", 2, false);
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException)
                {
                    return;
                }
            }

            try
            {
                // Detect army with 10 seconds of samples and 100ms sampling interval

                await BfmeClient.WaitForGameStarted(CancellationAssertion);

                Console.WriteLine($"Game started!");

                int sampleDurationSeconds = 10;
                int sampleFrequencyMiliseconds = 100;

                Console.WriteLine($"Detecting army...");

                int detectedArmy = await BfmeClient.GetArmy(sampleDurationSeconds, sampleFrequencyMiliseconds, CancellationAssertion);

                Console.WriteLine($"Detected army: {detectedArmy}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Army detection exited...");

                if (ex is TaskCanceledException)
                {
                    return;
                }
            }

            try
            {
                // Detect winner
                // WARNING: This only detects winners, and does not exit on defeat.

                Console.WriteLine($"Detecting winner...");

                bool isWinner = await BfmeClient.WaitForGameResult(CancellationAssertion);

                if (isWinner)
                {
                    Console.WriteLine("You won the game!");
                }
                else
                {
                    Console.WriteLine("You lost the game!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Win detection exited...");

                if (ex is TaskCanceledException)
                {
                    return;
                }
            }
        }

        static void CancellationAssertion()
        {
            // Use this to cancell any ongiong tasks by the API (for example WaitForGameStarted)

            bool shouldCancel = false;
            if (shouldCancel)
            {
                throw new TaskCanceledException();
            }
        }
    }
}