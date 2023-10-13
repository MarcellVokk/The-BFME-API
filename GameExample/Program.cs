using The_BFME_API.BFME1;

namespace GameExample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize the logger
            The_BFME_API.Logging.Logger.OnDiagnostic += (s, e) => { Console.WriteLine(e); };

            // This function demonstrates the BFME1 client interface portion of the API, and explains how to use it
            await Bfme1ClientDemo();

            Console.WriteLine("The program completed successfuly! Press return to exit.");
            Console.ReadLine();
        }

        static async Task Bfme1ClientDemo()
        {
            // This is just a demonstration, for the actual networking to work, you'd use the networking portion of the API
            // To create a room and put all the participating players in the same network before you start launching the game.

            Bfme1Client gameClient = new Bfme1Client();
            gameClient.CancelationAssertion = CancellationAssertion;
            gameClient.Username = "Hello world";
            gameClient.PlayerColor = 8;
            gameClient.MapId = "maps_5Cmap_20mp_20carnen_5Cmap_20mp_20carnen_2Emap";
            gameClient.Army = 2;
            gameClient.Team = 2;
            gameClient.Spot = 2;

            Console.WriteLine($"Launching game...");

            try
            {
                await gameClient.LaunchAsHost();

                //await gameClient.LaunchAsOffhost(() =>
                //{
                //    // Wait on the main menu until the host is ready...
                //    bool isHostReady = true;
                //    while (!isHostReady) { }
                //});

                Console.WriteLine($"Game launched and the player is in the ingame network lobby!");

                // You'd wait for all players to complete the launch process before calling this
                // And only the host can start the game obviously...
                gameClient.StartGame();

                Console.WriteLine($"Waiting for win screen to apear (EXPERIMENTAL)...");

                // Detect winner (EXPERIMENTAL)
                // WARNING: This only detects winners, and does not exit on defeat (so it just blocks the thread).
                //          You'd cancel this from the cancelation assertion once you know the other team has already won or the game is over.
                await gameClient.WaitForWinScreen();

                Console.WriteLine("You won the game!");

                // This is how you close the game
                await gameClient.CloseGame();
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException)
                {
                    return;
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void CancellationAssertion()
        {
            // Use this to cancell any ongoing tasks by the API

            bool shouldCancel = false;
            if (shouldCancel)
            {
                throw new TaskCanceledException();
            }
        }
    }
}