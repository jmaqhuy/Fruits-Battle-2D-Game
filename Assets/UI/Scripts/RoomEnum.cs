namespace RoomEnum
{
    public enum RoomMode
    {
        Normal,
        BotArea,
        Rank
    }

    public enum RoomStatus
    {
        InMatch,
        InLobby
    }

    public enum RoomType
    {
        OneVsOne = 2,
        TwoVsTwo = 4,
        FourVsFour = 8,
        None
    }
    
    public enum Team
    {
        Team1, Team2
    }

    public static class RoomModeTransfer
    {
        public static RoomMode RoomMode { get; set; }
    }

}