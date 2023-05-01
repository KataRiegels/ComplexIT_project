using System.Collections;

namespace SignalRChat.ChatRoomObjects
{
    public class Room
    {
        public string GroupId;
        public string? RoomName;
        public List<Participant> Participants;
        public Participant? KeyResponsible; //TODO: What if key responsible is null?

        public Room() { }
        public Room(string groupId, string roomName)
        {
            GroupId = groupId;
            RoomName = roomName;
            Participants = new List<Participant>();
        }

        public Room(string groupId)
        {
            GroupId = groupId;
            Participants = new List<Participant>();
        }

        public Room(string groupId, Participant keyResponsible)
        {
            GroupId = groupId;
            Participants = new List<Participant>();
            KeyResponsible = keyResponsible;
        }

        // Adds a participant to the list of users for this specific room
        public void AddParticipant(Participant participant)
        {
            Participants.Add(participant);
            foreach (var item in Participants)
            {
                Console.WriteLine("participant: " + item.ConnectionId);
                
            }
        }

        // Removes a participant from the list of users for this specific room
        public void RemoveParticipant(Participant participant) 
        {  
            Participants.Remove(participant);
        }


        // Whenever the room needs to find a new key responsible if the previous has disconnected.
        public void FindNewKeyResponsible()
        {
            KeyResponsible = Participants.First();
        }

        // ToString() that just lets you identify a room form its name
        public override string ToString()
        {
            return "Room with room name: " + RoomName;
        }

    }
}
