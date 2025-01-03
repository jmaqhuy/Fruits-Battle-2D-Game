using NetworkThread.Multiplayer;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/User Data")]
    public class UserData : ScriptableObject
    {
        public BasicUserInfoPacket UserInfo;
        public CurrentRankPacket CurrentRank;
    }
}
