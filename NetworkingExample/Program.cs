using System.Diagnostics;
using System.Runtime.InteropServices;
using The_BFME_API_by_MarcellVokk.BFME1;
using The_BFME_API_by_MarcellVokk.Network;

namespace NetworkingExample
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static async Task Main(string[] args)
        {
            AllocConsole();

            // Initialize the logger
            The_BFME_API_by_MarcellVokk.Logging.Logger.OnDiagnostic += Logger_OnDiagnostic;

            // This function demonstrates the networking portion of the API, and explains how to use it
            await NetworkDemo();

            Console.WriteLine("The program completed successfuly! Press return to exit.");
            Console.ReadLine();
        }

        private static void Logger_OnDiagnostic(object? sender, string e)
        {
            Console.WriteLine(e);
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