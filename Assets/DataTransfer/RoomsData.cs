using System.Collections.Generic;
using UnityEngine;

namespace DataTransfer
{
    [CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data/Rooms Data")]
    public class RoomsData : ScriptableObject
    {
        public List<RoomData> rooms = new List<RoomData>();
    }
}