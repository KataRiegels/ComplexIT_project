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
        static Dictionary<string, Participant>? connectionIdParticipantPairs = new Dictionary<string, Participant>();
        
        static Rooms ChatRooms = new Rooms();


        public async Task SendMessage( string message)
        {
            Participant callerParticipant = connectionIdParticipantPairs[Context.ConnectionId];
            string groupId = callerParticipant.roomId;

            await Clients.Group(groupId).SendAsync("ReceiveMessage", callerParticipant.Nickname, message);
        }

        //TODO: make sure generated ID doesn't already exist
        public async Task CreateRoom(string userName)
        {
            // Generating 8 digit group id
            string groupId = GenerateRoomID(8);

            await Console.Out.WriteLineAsync("created room");
            Participant creatorParticipant = new Participant(Context.ConnectionId, userName);
            // the new room
            Room newRoom = new Room(groupId, creatorParticipant);
            // Adding the room to the Hub's chat rooms
            AddRoomToHub(newRoom);
            // Adding the connected user to the new chat room
            JoinParticipantToRoom(groupId, creatorParticipant);
            //TODO: DELETEnewRoom
            await Clients.Caller.SendAsync("ReceiveGroupName", groupId);
        }

        // When a room as already been created, add a participant to said chat room
        public async Task JoinRoom(string userName, string roomName)
        {
            JoinParticipantToRoom(roomName, new Participant(Context.ConnectionId, userName));
        }

        public async void JoinParticipantToRoom(string roomName, Participant newParticipant)
        {
            Room roomToJoin = ChatRooms.GetRoom(roomName);
            AddParticipantToChatRoom(newParticipant, roomToJoin.GroupId);
        }



        private string GenerateRoomID(int length)
        {
            int idLength = length;

            // Characters and numbers to choose from
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();

            string id = "";

            Console.WriteLine(id);

            id = new string(Enumerable.Repeat(chars, idLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (ChatRooms.CheckIfRoomIdExists(id))

                return GenerateRoomID(idLength);

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
            connectionIdParticipantPairs.Add(newParticipant.ConnectionId, newParticipant);
        }

        private void AddParticipantToChatRoom(string connectionId, string roomID)
        {
            Groups.AddToGroupAsync(connectionId, roomID);
            ChatRooms.AddParticipantToRoom(connectionId, roomID);
        }

    }
}