using The_BFME_API.Network;

namespace NetworkingExample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize the logger
            The_BFME_API.Logging.Logger.OnDiagnostic += (s, e) => { Console.WriteLine(e); };

            // This function demonstrates the networking portion of the API, and explains how to use it
            await NetworkDemo();

            Console.WriteLine("The program completed successfuly! Press return to exit.");
            Console.ReadLine();
        }

        static async Task NetworkDemo()
        {
            string newRoomId = await NetworkManagement.OpenRoom();
            Console.WriteLine($"Room created successfuly! Room id: {newRoomId}");

            // IMPORTANT: The app needs to be running under administrator privileges, otherwise creating a NetworkClient will throw an exception!
            NetworkClient client = new NetworkClient();
            await client.CleanUp();

            await client.JoinRoom(newRoomId);
            Console.WriteLine($"Joined room successfuly!");

            Console.WriteLine("Press return to leave the room");
            Console.ReadLine();

            await client.LeaveRoom();

            await NetworkManagement.CloseRoom(newRoomId);

            client.Dispose();
        }
    }
}