using System.Collections.Generic;
using System.Linq;
using NetworkThread.Multiplayer.Packets;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Characters Data")]
    public class CharactersData : ScriptableObject
    {
        private static List<CharacterPacket> _characters = new();
        public List<CharacterPacket> Characters => _characters;
        public CharacterPacket GetCurrentCharacter() => _characters.FirstOrDefault(c => c.IsSelected);
    }
}
