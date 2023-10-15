using The_BFME_API.BFMERotWK;

namespace GameExampleBfmeRotWK
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize the logger
            The_BFME_API.Logging.Logger.OnDiagnostic += (s, e) => { Console.WriteLine(e); };

            // This function demonstrates the BFME RotWK client interface portion of the API, and explains how to use it
            await Bfme2ClientDemo();

            Console.WriteLine("The program completed successfuly! Press return to exit.");
            Console.ReadLine();
        }

        static async Task Bfme2ClientDemo()
        {
            // This is just a demonstration, for the actual networking to work, you'd use the networking portion of the API
            // To create a room and put all the participating players in the same network before you start launching the game.

            // Create a new BfmeRotWKClient instance, and assign our players settings
            BfmeRotWKClient gameClient = new BfmeRotWKClient
            {
                CancelationAssertion = CancellationAssertion,
                Username = "Hello world",
                PlayerColor = PlayerColor.Blue,
                MapId = "maps_5Cmap_20wor_20mordor_5Cmap_20wor_20mordor_2Emap",
                Army = PlayerArmy.Angmar,
                Hero = PlayerHero.None,
                Team = PlayerTeam.Team2,
                SpotIndex = 0
            };

            try
            {
                Console.WriteLine($"Launching game...");

                // Launch game as host
                await gameClient.LaunchAsHost();
                // Optionaly, you can also provide gamerules
                // await gameClient.LaunchAsHost(initialResources: 1000, commandPointFactor: 100, allowCustomHeroes: false, allowRingHeroes: false);

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