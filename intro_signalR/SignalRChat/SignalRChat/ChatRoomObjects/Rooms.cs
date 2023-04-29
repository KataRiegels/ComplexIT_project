using System.Drawing;

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
        Console.WriteLine("Rooms.GetRoom: " + room.RoomName);
        Console.WriteLine("Rooms.GetRoom: " + room.GroupId);
        return room;
    }

    public Room GetRoomByName(string roomName)
    {
        //TODO: NULL?
        Console.WriteLine("roomname: " + roomName);
        Console.WriteLine("room size: " + this.Count());
        foreach (var r in this)
        {
            Console.WriteLine(r.RoomName);
            Console.WriteLine(r.GroupId);
        }
        Room room = Find(x => x.RoomName.Equals(roomName));
        Console.WriteLine("Rooms.GetRoomByName: " + room.RoomName + ", id: " + room.GroupId);
        return room;
    }


    // No need to use this specifically - personal preference. Simply just Rooms.Add(Room) otherwise. 
    public void AddRoom(Room room)
    {
        this.Add(room);
    }
        
    public void RemoveRoom(int id) { }

    public void AddParticipantToRoom(Participant participant, string groupId)
    {
        GetRoom(groupId).AddParticipant(participant);
    }

    public void AddParticipantToRoom(string connectionId, string groupId)
    {
        //GetRoom(groupId).AddParticipant(participant);
    }


}
