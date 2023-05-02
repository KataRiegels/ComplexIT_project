using Microsoft.AspNetCore.Server.IIS.Core;
using System.Collections;

namespace SignalRChat.ChatRoomObjects
{
    public class Room : IDisposable
    {
        public string GroupId;
        public string? RoomName;
        public List<Participant> Participants;
        public Participant? KeyResponsible; //TODO: What if key responsible is null?
        private bool disposedValue;

        //public Room() { }
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
        }

        // Removes a participant from the list of users for this specific room
        public void RemoveParticipant(Participant participant) 
        {
            Participants.Remove(participant);
            
            if (participant.Equals(KeyResponsible))
            {
                Console.WriteLine("Host left! :" + KeyResponsible.ConnectionId);
                KeyResponsible = Participants.FirstOrDefault();
                if (KeyResponsible != null)
                    Console.WriteLine("New host is: " + KeyResponsible.ConnectionId);
            }

            if (Participants.Count() <= 0)
            {
                Console.WriteLine("Should close room?");
                Dispose(true);

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
            return "Room with room name: " + GroupId;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Room()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            if (disposedValue)
            {
                return;
            }

            Dispose(true);

            disposedValue = true;
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
