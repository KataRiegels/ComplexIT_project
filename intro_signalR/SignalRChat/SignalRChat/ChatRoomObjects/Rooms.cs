using System.Drawing;
using System.Text.RegularExpressions;

namespace SignalRChat.ChatRoomObjects;

public class Rooms : List<Room>, IRooms
{

    public Rooms() { }

    public Rooms(Room room)
    {
        Add(room);
    }


    public Room GetRoom(string id)
    {
        Room? room = Find(x => x.RoomId.Equals(id));

        if (room != null)
            return room;

        // No room with that ID in List
        throw new NullReferenceException();
    }

    ////public Room GetRoomById(string roomName)
    ////{
    ////    //TODO: NULL?
    ////    Room? room = Find(x => x.RoomName.Equals(roomName));
    ////    return room;
    ////}


    // No need to use this specifically - personal preference. Simply just Rooms.Add(Room) otherwise. 
    public void AddRoom(Room room)
    {
        Add(room);
    }

    public void RemoveRoom(string roomId)
    {
        Remove(GetRoom(roomId));
    }

    public void AddParticipantToRoom(Participant participant, string groupId)
    {
        GetRoom(groupId).AddParticipant(participant);
        participant.RoomId = groupId;
    }

    public void RemoveParticipantFromRoom(Participant participant)
    {
        var room = GetRoom(participant.RoomId);
        room.RemoveParticipant(participant);
        if (room.Participants.Count() <= 0)
        {
            RemoveRoom(room.RoomId);
        }
        Console.WriteLine("room exists? " + GetRoom(participant.RoomId));


    }


    public void AddParticipantToRoom(string connectionId, string groupId)
    {
        //GetRoom(groupId).AddParticipant(participant);
    }
    public bool ContainsRoom(string roomId)
    {
        foreach (Room room in this)
        {
            if (room.RoomId.Equals(roomId))
            {
                return true;
            }
        }
        return false;
    }


}
