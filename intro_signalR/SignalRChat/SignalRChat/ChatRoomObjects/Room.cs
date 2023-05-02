using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections;

namespace SignalRChat.ChatRoomObjects
{
    public class Room
    {
        public string RoomId;
        public string? RoomName;
        public List<Participant> Participants;
        public Participant KeyResponsible; //TODO: What if key responsible is null?
        ////private bool disposedValue;

        //public Room() { }
        ////public Room(string groupId, string roomName)
        ////{
        ////    RoomId = groupId;
        ////    RoomName = roomName;
        ////    Participants = new List<Participant>();
        ////}

        ////public Room(string groupId)
        ////{
        ////    RoomId = groupId;
        ////    Participants = new List<Participant>();
        ////}

        public Room(string groupId, Participant keyResponsible)
        {
            RoomId = groupId;
            Participants = new List<Participant>();
            KeyResponsible = keyResponsible;
        }

        // Adds a participant to the list of users for this specific room
        public void AddParticipant(Participant participant)
        {
            Participants.Add(participant);
        }

        // Removes a participant from the list of users for this specific room
        public void RemoveParticipant(Participant participant) 
        {
            Participants.Remove(participant);
            
            if (participant.Equals(KeyResponsible))
            {
                Console.WriteLine("Host left! :" + KeyResponsible.ConnectionId);
                Participant? newKeyResponsible = Participants.FirstOrDefault();
                if (newKeyResponsible != null)
                    KeyResponsible = newKeyResponsible;
                    Console.WriteLine("New host is: " + KeyResponsible.ConnectionId);
            }



        }

        

        // Whenever the room needs to find a new key responsible if the previous has disconnected.
        public void FindNewKeyResponsible()
        {
            KeyResponsible = Participants.First();
        }

        // ToString() that just lets you identify a room form its name
        public override string ToString()
        {
            return "Room with room name: " + RoomId;
        }
    }
}
