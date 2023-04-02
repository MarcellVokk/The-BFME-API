using The_BFME_API_by_MarcellVokk.Network;

namespace Example
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // This function demonstrates the networking portion of the API, and explains how to use it
            await NetworkDemo();
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
    }
}