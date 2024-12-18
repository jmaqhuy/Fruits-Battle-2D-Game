using System.Collections.Generic;
using NetworkThread.Multiplayer.Packets;
using RoomEnum;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Room Data")]
    public class RoomData : ScriptableObject
    {
        public RoomPacket RoomPacket;
        public List<PlayerInRoomPacket> PlayersInRoom;
    }
}
