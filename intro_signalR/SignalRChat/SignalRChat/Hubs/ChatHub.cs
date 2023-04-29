using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using SignalRChat.ChatRoomObjects;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        //public KeyValuePair<string, string>[] groupIdNamePair;
        Dictionary<string, string>? groupIdNamePairs = new Dictionary<string, string>();
        static Rooms ChatRooms = new Rooms(new Room("4", "k"));

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //public async Task CreateRoom(string groupName)
        public async Task CreateRoom(string roomName = "dummy")
        {
            // Generating 8 digit group id
            string groupId = GenerateRoomID(8);


            // the new room
            Room newRoom = new Room(groupId, roomName);

            // Adding the room to the Hub's chat rooms
            AddRoomToHub(newRoom);
            // Adding the connected user to the new chat room
            await Console.Out.WriteLineAsync("room name: " + newRoom.RoomName);
            JoinRoom(roomName);
            //TODO: DELETEnewRoom
            await Console.Out.WriteLineAsync(Clients.Group(groupId).ToString());


        }

        // When a room as already been created, add a participant to said chat room
        public async Task JoinRoom(string roomName)
        {
            JoinParticipantToRoom(roomName);
            Room roomToJoin = ChatRooms.GetRoomByName(roomName);
            //await Console.Out.WriteLineAsync(roomToJoin.GroupId);
            //AddParticipantToChatRoom(new Participant(Context.ConnectionId), roomToJoin.GroupId);
        }

        public async void JoinParticipantToRoom(string roomName)
        {
            await Console.Out.WriteLineAsync("room size bitch: " + ChatRooms.Count());
            Room roomToJoin = ChatRooms.GetRoomByName(roomName);
            await Console.Out.WriteLineAsync(roomToJoin.GroupId);
            AddParticipantToChatRoom(new Participant(Context.ConnectionId), roomToJoin.GroupId);
        }



        private string GenerateRoomID(int length)
        {
            int idLength = 8;

            // Characters and numbers to choose from
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();

            // Generate the ID
            string id = new string(Enumerable.Repeat(chars, idLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return id;
        }


        // Uses SignalR's method that gets triggered when the client uses the HubConnection.start(). 
        public override  Task OnConnectedAsync()
        {
            Console.Out.WriteLineAsync("Connected ID: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        private void AddRoomToHub(Room newRoom)
        {
            Console.WriteLine(newRoom.GroupId);
            ChatRooms.AddRoom(newRoom);
            // 
        }

        // Adding a new participant to an existing room
        private void AddParticipantToChatRoom(Participant newParticipant, string roomID)
        {
            Groups.AddToGroupAsync(newParticipant.ConnectionId, roomID);
            ChatRooms.AddParticipantToRoom(newParticipant, roomID);
        }

        private void AddParticipantToChatRoom(string connectionId, string roomID)
        {
            Groups.AddToGroupAsync(connectionId, roomID);
            ChatRooms.AddParticipantToRoom(connectionId, roomID);
        }

    }
}