using The_BFME_API.BFME1;

namespace GameExampleBfme1
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

            // Create a new Bfme1Client instance, and assign our players settings
            Bfme1Client gameClient = new Bfme1Client
            {
                CancelationAssertion = CancellationAssertion,
                Username = "Hello world",
                PlayerColor = PlayerColor.Orange,
                MapId = "maps_5Cmap_20mp_20dagorlad_5Cmap_20mp_20dagorlad_2Emap",
                Army = PlayerArmy.Mordor,
                Team = PlayerTeam.Team2,
                SpotIndex = 0
            };

            try
            {
                Console.WriteLine($"Launching game...");

                // Launch game as host
                await gameClient.LaunchAsHost();

                // Launch game as offhost
                // await gameClient.LaunchAsOffhost(() =>
                // {
                //     // Wait on the main menu until the host is ready...
                //     bool isHostReady = true;
                //     while (!isHostReady) { }
                // });

                // Start the game (only works on host)
                gameClient.StartGame();

                Console.WriteLine($"Game started!");

                // Close the game
                // await gameClient.CloseGame();
            }
            catch (Exception ex)
            {
                if (ex is not TaskCanceledException)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void CancellationAssertion()
        {
            // Throw a TaskCanceledException exception here to cancel any ongoing tasks by the API
            // throw new TaskCanceledException();
        }
    }
}