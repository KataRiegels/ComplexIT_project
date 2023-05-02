namespace SignalRChat.ChatRoomObjects;

public class Participant
{
    public string ConnectionId { get; set; }
    public string Nickname { get; set; }
    public string RoomId { get; set; }

    public Participant (string connectionId, string nickname)
    {
        ConnectionId = connectionId;
        Nickname     = nickname;
        RoomId       = "Undefined room";
    }

    public Participant(string connectionId)
    {
        Nickname     = "Anonymous";
        RoomId       = "Undefined room";
        ConnectionId = connectionId;
    }


    //public static Room GetRoom(string connectionId)
    //{

    //}




}
