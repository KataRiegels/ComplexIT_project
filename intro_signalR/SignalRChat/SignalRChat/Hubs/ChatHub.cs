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


        // Asks client witn connID to return a encrypted symmetric key
        

        // Callback to join room button on client
        public async Task JoinRoom(string userName, string roomID, string publicKey2 = "")
        {

            if (!chatRooms.ContainsRoom(roomID))
            {
                //TODO: Room does not exist. Inform client!
            }


            //? Client should call invoke("JoinRoom").then().invoke("ReceiveKey")
            Room currentRoom = chatRooms.GetRoom(roomID);
            foreach (var room in chatRooms)
            {
                await Console.Out.WriteLineAsync(room.RoomId);
            }

            // Sends and waits for client (host) result to get the symmetric key
            string encryptedSymmetricKey = await AskForSymmetricKey(connectionId: currentRoom.KeyResponsible.ConnectionId,
                                                                      publicKey2);

            // Sends out the encrypted key parameter to ReceieveKey callback(?)
            await Clients.Caller.SendAsync("ReceiveKey", encryptedSymmetricKey);
            AddParticipantToChatRoom(new Participant(Context.ConnectionId, userName), roomID);
        }

        /// <summary>
        /// Invokes client's "SendKey" and waits for a reply. Meant to retreive encrypted symmetric key
        /// </summary>
        /// <param name="connectionId">Connection ID for the Participant meant to generate the key</param>
        /// <returns>A string with the encrypted symmetric key</returns>
        public async Task<string> AskForSymmetricKey(string connectionId, string publicKey3)
        {
            //todo: cancellation token!
            await Console.Out.WriteLineAsync("symmetric key: " + publicKey3);
            ////var key2 = await Clients.Client(connectionId).InvokeAsync<string>("SendKey", cancellationToken: new CancellationToken());
                ////.InvokeAsync<string>(method: "SendKey", arg1: publicKey2, new CancellationToken());
            var key = await Clients.Client(connectionId)
                .InvokeAsync<string>(method: "SendKey", publicKey3,  new CancellationToken());
            return "";
        }


        
        // Uses SignalR's method that gets triggered when the client uses the HubConnection.start(). 
        public override  Task OnConnectedAsync()
        {
            Console.Out.WriteLineAsync("Connected ID: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //todo: Let frontend know about leaving?
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
        /// <param name="roomID">The Room to add Participant to</param>
        ////private static void AddRoomToHub(Room newRoom)
        ////{
        ////    chatRooms.AddRoom(newRoom);
        ////}

        // Adding a new participant to an existing room
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

            ////Room roomToJoin = chatRooms.GetRoom(roomID);
            connectionId_Participant_Pairs.Add(newParticipant.ConnectionId, newParticipant);
            Console.WriteLine("participant: " + connectionId_Participant_Pairs[newParticipant.ConnectionId]);


            // Adding participants to the Group and the equivalent Room
            Groups   .AddToGroupAsync(connectionId: newParticipant.ConnectionId, groupName: roomID);
            chatRooms.AddParticipantToRoom(newParticipant, roomID);
        }

        // Deletes participants from the Group and related room
        // Handles case of empty room
        /// <summary>
        /// Removes a Participant from the Hub.Group, chatRoom and the Participant Dictionary.
        /// </summary>
        /// <param name="connectionId">ConnectionId for the Participant to remove</param>
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


        // Generates an *length* digit room ID
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