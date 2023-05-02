namespace SignalRChat.ChatRoomObjects
{
    public interface IRooms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participant">The Participant to add</param>
        /// <param name="roomID">The </param>
        void AddParticipantToRoom(Participant participant, string roomID);
        void AddParticipantToRoom(string connectionId, string groupId);
        void AddRoom(Room room);
        bool CheckIfRoomIdExists(string roomId);
        bool ContainsRoom(string roomId);
        Room GetRoom(string id);
        void RemoveParticipantFromRoom(Participant participant);
        void RemoveRoom(string roomId);
    }
}