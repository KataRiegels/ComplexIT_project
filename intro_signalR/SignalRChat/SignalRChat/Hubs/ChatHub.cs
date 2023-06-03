using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using SignalRChat.ChatRoomObjects;
using System.Runtime.CompilerServices;
using System.Net.Http;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        static Dictionary<string, Participant> connectionId_Participant_Pairs = new Dictionary<string, Participant>();
        static readonly Rooms chatRooms = new Rooms();

        /// <summary>
        /// For the client to see the latest message id: Method to call remotely from the client
        /// </summary>
        /// <returns>The current message ID. Returned to client when invoked.</returns>
        public async Task<int> GetMessageId()
        {
            Participant callerParticipant = connectionId_Participant_Pairs[Context.ConnectionId];
            string groupId = callerParticipant.RoomId;
            Room room = chatRooms.GetRoom(groupId);
            return await Task.FromResult(room.messageID);
        }

        /// <summary>
        /// Relaying messages to other clients: Method to call remotely from the client
        /// </summary>
        /// <param name="message">The data received from one client that should be relayed to others in that room</param>
        /// <returns></returns>
        public async Task SendMessage(object message)
        {
            Participant callerParticipant = connectionId_Participant_Pairs[Context.ConnectionId];
            string groupId = callerParticipant.RoomId;
            Room room = chatRooms.GetRoom(groupId);
            // Sends message to everyone in the room (Group) besides the Caller
            await Clients.OthersInGroup(groupId).SendAsync(method: "ReceiveMessage", arg1: callerParticipant.Nickname, message, room.messageID);
            // increase message ID such that all clients can keep track of the ID
            room.messageID++;
        }


        /// <summary>
        /// Create a new room: Method to call remotely from the client
        /// </summary>
        /// <param name="userName">Username of the user creating the room</param>
        /// <returns></returns>
        public async Task CreateRoom(string userName)
        {
            Participant creatorParticipant = new Participant(Context.ConnectionId, userName);

            // Adding the room to the Hub's chat rooms
            string roomID = GenerateRoomID(8);
            Room   newRoom = new Room(roomID, creatorParticipant);
            chatRooms.Add(newRoom);

            await Console.Out.WriteLineAsync("created room with room ID: " + newRoom.RoomId);
            
            // Adding the connected user to the new chat room
            AddParticipantToChatRoom(newParticipant: creatorParticipant, roomID);

            await Clients.Caller.SendAsync("ReceiveGroupName", roomID);
        }

        /// <summary>
        /// Join existing room: Method to call remotely from the client
        /// </summary>
        /// <param name="userName">Username of the user joining</param>
        /// <param name="roomID">ID of the room the user is trying to join</param>
        /// <param name="publicKey">Public key of the joining particpant</param>
        /// <returns></returns>
        // Callback to join room button on client
        public async Task JoinRoom(string userName, string roomID, string publicKey = "") { 

            // If the room doesn't exist
            if (!chatRooms.ContainsRoom(roomID))
            {
                await Clients.Caller.SendAsync("RoomNotFound"); // not handled yet
                return;
            }

            Room currentRoom = chatRooms.GetRoom(roomID);
          
            // Sends and waits for client (key responsible) result to get the symmetric key
            string encryptedSymmetricKey = await AskForSymmetricKey(connectionId: currentRoom.KeyResponsible.ConnectionId,
                                                                      publicKey);

            // Sends out the encrypted key parameter to ReceieveKey callback(?)
            await Clients.Caller.SendAsync("ReceiveKey", encryptedSymmetricKey);
            await Clients.Caller.SendAsync("GetRoomId", currentRoom.messageID);

            // Add the participant to the client list and their room
            AddParticipantToChatRoom(new Participant(Context.ConnectionId, userName), roomID);
        }

        /// <summary>
        /// Invokes client's "SendKey" and waits for a reply. Meant to retreive encrypted symmetric key
        /// </summary>
        /// <param name="connectionId">Connection ID for the Participant meant to generate the key</param>
        /// <returns>A string with the encrypted symmetric key</returns>
        public async Task<string> AskForSymmetricKey(string connectionId, string publicKey)
        {
            var key = await Clients.Client(connectionId)
                .InvokeAsync<string>(method: "SendKey", publicKey,  new CancellationToken());
            return key;
        }


        
        // Overrides SignalR's method that gets triggered when the client uses the HubConnection.start(). 
        public override  Task OnConnectedAsync()
        {
            Console.Out.WriteLineAsync("Connected ID: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        // Overrides SignalR's method that gets triggered when a disconnection is detected. 
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Removes participant when closing the browser tab
            RemoveParticipantFromChatRoom(Context.ConnectionId);
            Console.Out.WriteLineAsync("Disconnected ID " + Context.ConnectionId);
            
            return base.OnDisconnectedAsync(exception);
        }



        /**
         HELPER METHODS!
         */

        

        /// <summary>
        /// Adds Participant to the chatRoom List and Hub.Group. Includes adding the Participant to the participant Dictionary.
        /// </summary>
        /// <param name="newParticipant">The Participant that is joining the chat</param>
        private void AddParticipantToChatRoom(Participant newParticipant, string roomID)
        {
            if (connectionId_Participant_Pairs.ContainsKey(newParticipant.ConnectionId))
            {
                //TODO: What if the person tries to join the room again?
            }

            connectionId_Participant_Pairs.Add(newParticipant.ConnectionId, newParticipant);
            Console.WriteLine("participant: " + connectionId_Participant_Pairs[newParticipant.ConnectionId]);


            // Adding participants to the Group and the equivalent Room
            Groups   .AddToGroupAsync(connectionId: newParticipant.ConnectionId, groupName: roomID);
            chatRooms.AddParticipantToRoom(newParticipant, roomID);
        }

        /// <summary>
        /// Removes a Participant from the Hub.Group, chatRoom and the Participant Dictionary, includes handling empty rooms.
        /// </summary>
        /// <param name="connectionId">ConnectionId for the Participant to remove</param>
        private void RemoveParticipantFromChatRoom(string connectionId)
        {
            Participant participant = connectionId_Participant_Pairs[connectionId];
            string roomId = participant.RoomId;

            // Removing participants from all the necessary places
            connectionId_Participant_Pairs.Remove(connectionId);
            Room room = chatRooms.GetRoom(roomId);


            // Removing for SignalR's Group - if it is the last participant, the Group automatically gets removed
            Groups.RemoveFromGroupAsync(participant.ConnectionId, roomId);
            chatRooms.RemoveParticipantFromRoom(participant);

            // Send file storage HTTP request to remove files associated to the room.
            if (!chatRooms.Contains(room))
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DeleteAsync("http://77.33.131.226:3000/api/databaseapi/delete/room/" + roomId);
            }


        }

        /// <summary>
        /// Generates a Room ID with random letters and numbers
        /// </summary>
        /// <param name="length">Length of the ID</param>
        /// <returns>A string of randomly assembled characters</returns>
        private string GenerateRoomID(int length)
        {
            Random random = new Random();
            int idLength = length;

            // Characters and numbers to choose from
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Assembling the characters randomly to form the ID string
            string id = new string(Enumerable.Repeat(chars, idLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Recursive in case the ID had already been generated
            if (chatRooms.ContainsRoom(id))
                return GenerateRoomID(idLength);

            return id;
        }




    }
}