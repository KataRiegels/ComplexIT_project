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

        static Dictionary<string, Participant> connectionId_Participant_Pairs = new Dictionary<string, Participant>();
        static readonly Rooms chatRooms = new Rooms();


        public async Task SendMessage(string message)
        {
            Participant callerParticipant = connectionId_Participant_Pairs[Context.ConnectionId];
            string groupId = callerParticipant.RoomId;

            // Sends message to everyone in the room (Group) besides the Caller
            await Clients.OthersInGroup(groupId).SendAsync(method: "ReceiveMessage", arg1: callerParticipant.Nickname, message);
            
            //TODO: maybe send tick that everyone received it
        
        }

        ////TODO: make sure generated ID doesn't already exist
        public async Task CreateRoom(string userName)
        {

            Participant creatorParticipant = new Participant(Context.ConnectionId, userName);

            // Adding the room to the Hub's chat rooms
            string roomID = GenerateRoomID(8);
            Room   newRoom = new Room(roomID, creatorParticipant);
            AddRoomToHub(newRoom);
            await Console.Out.WriteLineAsync("created room with room ID: " + newRoom.GroupId);
            
            // Adding the connected user to the new chat room
            AddParticipantToChatRoom(newParticipant: creatorParticipant, roomID);

            await Clients.Caller.SendAsync("ReceiveGroupName", roomID);
        }



        public async Task<string> AskForSymmetricKey(string connectionId)
        {
            //todo: cancellation token!
            var key = await Clients.Client(connectionId).InvokeAsync<string>(
                "SendKey", new CancellationToken());
            return key;
        }

        // Called from "Join room" button on Client
        public async Task JoinRoom(string userName, string roomName, string encryptedPublicKey = "")
        {

            if (!chatRooms.ContainsRoom(roomName))
            {
                //TODO: Room does not exist. Inform client!
            }


            //? Client should call invoke("JoinRoom").then().invoke("ReceiveKey")
            Room currentRoom = chatRooms.GetRoom(roomName);
            ISingleClientProxy keyResp = Clients.Client(connectionId: currentRoom.KeyResponsible.ConnectionId);

            // Sending request to key responsible - Client will invoke ReceiveKey() method
            await keyResp.SendAsync("RequestKey", encryptedPublicKey);
            //// string message = await WaitForMessage(currentRoom.KeyResponsible.ConnectionId);
            //// await Console.Out.WriteLineAsync("Returned message " + message);

            // Sends and waits for client (host) result to get the symmetric key
            //!? uncomment !
            // string encryptedSymmetricKey = await AskForSymmetricKey(currentRoom.KeyResponsible.ConnectionId);

            // Sends out the encrypted key parameter to ReceieveKey callback(?)
            //!? uncomment !
            // await Clients.Caller.SendAsync("ReceiveKey", encryptedSymmetricKey);

            AddParticipantToChatRoom( new Participant(Context.ConnectionId, userName), roomName);
        }




        //!   Delete THIS
        ////private void PrintPartPairs()
        ////{
        ////    foreach (var item in connectionId_Participant_Pairs)
        ////    {
        ////        Console.WriteLine("connected: " + item.Key);
        ////    }
        ////}


        //? static?
        private static void AddRoomToHub(Room newRoom)
        {
            chatRooms.AddRoom(newRoom);
        }

        // Adding a new participant to an existing room
        private void AddParticipantToChatRoom(Participant newParticipant, string roomID)
        {
            if (connectionId_Participant_Pairs.ContainsKey(newParticipant.ConnectionId))
            {
                //TODO: What if the person tries to join the room again?
            }



            ////Room roomToJoin = chatRooms.GetRoom(roomID);
            connectionId_Participant_Pairs.Add(newParticipant.ConnectionId, newParticipant);

            // Adding participants to the Group and the equivalent Room
            Groups.AddToGroupAsync(newParticipant.ConnectionId, roomID);
            chatRooms.AddParticipantToRoom(newParticipant, roomID);


        }

        // Deletes participants from the Group and related room
        // Handles case of empty room
        private void RemoveParticipantFromChatRoom(string connectionId)
        {
            Participant participant = connectionId_Participant_Pairs[connectionId];
            string roomId = participant.RoomId;
            ////Room room = chatRooms.GetRoom(roomId);
            
            // Removing participants from all the necessary places
            connectionId_Participant_Pairs.Remove(connectionId);
            
            // Removing for SignalR's Group - if it is the last participant, the Group automatically gets removed
            Groups.RemoveFromGroupAsync(participant.ConnectionId, roomId);
            chatRooms.RemoveParticipantFromRoom(participant);

            // Closing the room if the last person left
            ////if (room.Participants.Count <= 0)
            ////{
            ////    RemoveRoom(roomId);
            ////}

        }

        
        ////private void RemoveRoom(string roomId)
        ////{
        ////    chatRooms.RemoveRoom(roomId);
        ////}




        
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
            Console.Out.WriteLineAsync("Disconnected ID " + Context.ConnectionId);
            
            return base.OnDisconnectedAsync(exception);
        }



        // Generates an *length* digit room ID
        private string GenerateRoomID(int length)
        {
            Random random = new Random();
            int idLength = length;

            // Characters and numbers to choose from
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            string id = new string(Enumerable.Repeat(chars, idLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (chatRooms.CheckIfRoomIdExists(id))
                return GenerateRoomID(idLength);

            return id;
        }




    }
}