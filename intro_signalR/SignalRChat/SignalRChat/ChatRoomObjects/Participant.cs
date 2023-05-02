namespace SignalRChat.ChatRoomObjects;

public class Participant
{
    public string ConnectionId { get; set; }
    public string? Nickname { get; set; }
    public string? RoomId { get; set; }

    public Participant (string connectionId, string nickname)
    {
        ConnectionId = connectionId;
        Nickname     = nickname;
    }

    public Participant(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public void SetNickname(string nickname)
    {
        Nickname = nickname;
    }

    


    //public static Room GetRoom(string connectionId)
    //{

    //}




}
