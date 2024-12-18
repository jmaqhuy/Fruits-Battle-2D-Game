using System.Collections.Generic;
using NetworkThread.Multiplayer.Packets;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Positions Data")]
    public class PositionsData : ScriptableObject
    {
        public List<SpawnPlayerPacket> spawnPlayerPackets;
    }
}