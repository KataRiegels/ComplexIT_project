namespace SignalRChat.ChatRoomObjects
{
    public interface IRooms
    {
        /// <summary>
        /// Add a Participant a Room in the List
        /// </summary>
        /// <param name="participant">The Participant to add</param>
        /// <param name="roomID">The ID for the Room Participant is being added to</param>
        void AddParticipantToRoom(Participant participant, string roomID);
        void AddParticipantToRoom(string connectionId, string groupId);
        /// <summary>
        /// Add a Room to the List
        /// </summary>
        /// <param name="room">Room instance</param>
        void AddRoom(Room room);

        /// <summary>
        /// Checks if there is a Room with that ID. 
        /// </summary>
        /// <param name="roomId">Room instance ID as string</param>
        /// <returns>boolean. True if the room is contained in the List.</returns>
        bool ContainsRoom(string roomId);

        /// <summary>
        /// Get Room instance
        /// </summary>
        /// <param name="id">ID for the desired Room</param>
        /// <returns>A room if it exists. NullPointException if the room does not exist </returns>
        Room GetRoom(string id);
        
        /// <summary>
        /// Remove a Participant from its room
        /// </summary>
        /// <param name="participant">Participant to remove from Room</param>
        void RemoveParticipantFromRoom(Participant participant);
        
        /// <summary>
        /// Removes the room for the List
        /// </summary>
        /// <param name="roomId">ID for the Room to be removed</param>
        void RemoveRoom(string roomId);
    }
}