using System.Collections.Generic;
using NetworkThread.Multiplayer.Packets;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Characters Data")]
    public class CharactersData : ScriptableObject
    {
        public List<CharacterPacket> Characters = new List<CharacterPacket>();
    }
}
