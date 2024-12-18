using Lidgren.Network;

namespace RoomEnum
{
    public enum RoomMode
    {
        All,
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
        All = 0,
        OneVsOne = 2,
        TwoVsTwo = 4,
        FourVsFour = 8,
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