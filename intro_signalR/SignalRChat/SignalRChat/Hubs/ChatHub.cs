using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using SignalRChat.ChatRoomObjects;
using System.Runtime.CompilerServices;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        static Dictionary<string, Participant>? connectionIdParticipantPairs = new Dictionary<string, Participant>();
        
        static Rooms ChatRooms = new Rooms();


        public async Task SendMessage( string message)
        {
            Participant callerParticipant = connectionIdParticipantPairs[Context.ConnectionId];
            string groupId = callerParticipant.roomId;

            await Clients.OthersInGroup(groupId).SendAsync("ReceiveMessage", callerParticipant.Nickname, message);
            
            //TODO: maybe send tick that everyone received it
        
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
            AddParticipantToChatRoom(creatorParticipant, groupId);

            //TODO: DELETEnewRoom
            await Clients.Caller.SendAsync("ReceiveGroupName", groupId);
        }

        // Called from "Join room" button on Client
        public async Task JoinRoom(string userName, string roomName)
        {

            if (!ChatRooms.ContainsRoom(roomName))
            {
                //TODO: Room does not exist. Inform client!
            }



            AddParticipantToChatRoom( new Participant(Context.ConnectionId, userName), roomName);
        }



        private string GenerateRoomID(int length)
        {
            int idLength = length;

            // Characters and numbers to choose from
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


            Random random = new Random();

            string id = new string(Enumerable.Repeat(chars, idLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (ChatRooms.CheckIfRoomIdExists(id))
                return GenerateRoomID(idLength);

            return id;
        }



        private void PrintPartPairs()
        {
            foreach (var item in connectionIdParticipantPairs)
            {
                Console.WriteLine("connected: " + item.Key);
            }
        }

        private void AddRoomToHub(Room newRoom)
        {
            ChatRooms.AddRoom(newRoom);
            // 
        }

        // Adding a new participant to an existing room
        private void AddParticipantToChatRoom(Participant newParticipant, string roomID)
        {
            if (connectionIdParticipantPairs.ContainsKey(newParticipant.ConnectionId))
            {
                //TODO: What if the person tries to join the room again?
            }

            Room roomToJoin = ChatRooms.GetRoom(roomID);
            connectionIdParticipantPairs.Add(newParticipant.ConnectionId, newParticipant);

            Groups.AddToGroupAsync(newParticipant.ConnectionId, roomID);
            ChatRooms.AddParticipantToRoom(newParticipant, roomID);


        }

        private void RemoveParticipantFromChatRoom(string connectionId)
        {
            Participant participantToRemove = connectionIdParticipantPairs[connectionId];
            string roomId = participantToRemove.roomId;
            Room room = ChatRooms.GetRoom(roomId);
            
            // Removing participants from all the necessary places
            connectionIdParticipantPairs.Remove(connectionId);
            
            // Removing for SignalR's Group - if it is the last participant, the Group automaticall gets trashed
            Groups.RemoveFromGroupAsync(participantToRemove.ConnectionId, roomId);
            ChatRooms.RemoveParticipantFromRoom(participantToRemove);


            // Closing the room if the last person left
            if (room.Participants.Count <= 0)
            {
                RemoveRoom(roomId);
            }

        }

        private void RemoveRoom(string roomId)
        {
            ChatRooms.RemoveRoom(roomId);
        }




        
        // Uses SignalR's method that gets triggered when the client uses the HubConnection.start(). 
        public override  Task OnConnectedAsync()
        {
            Console.Out.WriteLineAsync("Connected ID: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Removes participant when closing the browser tab
            RemoveParticipantFromChatRoom(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }


    }
}