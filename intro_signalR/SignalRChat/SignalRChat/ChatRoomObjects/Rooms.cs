using System.Drawing;
using System.Text.RegularExpressions;

namespace SignalRChat.ChatRoomObjects;

public class Rooms : List<Room>
{

    public Rooms() { }

    public Rooms(Room room) 
    {
        Add(room);
    }


    public Room GetRoom(string id)
    {
        //TODO: NULL?
        Room room = Find(x => x.GroupId.Equals(id));
        return room;
    }

    public Room GetRoomById(string roomName)
    {
        //TODO: NULL?
        Room room = Find(x => x.RoomName.Equals(roomName));
        return room;
    }


    // No need to use this specifically - personal preference. Simply just Rooms.Add(Room) otherwise. 
    public void AddRoom(Room room)
    {
        this.Add(room);
    }
        
    public void RemoveRoom(string roomId) {

        Remove(GetRoom(roomId));
    }

    public void AddParticipantToRoom(Participant participant, string groupId)
    {
        GetRoom(groupId).AddParticipant(participant);
        participant.roomId = groupId;
    }

    public void RemoveParticipantFromRoom(Participant participant)
    {
        GetRoom(participant.roomId).RemoveParticipant(participant);
    }


    public void AddParticipantToRoom(string connectionId, string groupId)
    {
        //GetRoom(groupId).AddParticipant(participant);
    }

    public bool CheckIfRoomIdExists(string roomId)
    {
        foreach (Room room in this)
        {
            if (room.GroupId.Equals(roomId)){
                return true;
            }
        }
        return false;
    }

    public bool ContainsRoom(string roomId)
    {
        if (!CheckIfRoomIdExists(roomId))
        {
            return false;
        }
        return true;
    }


}
