using System;
using UnityEngine;

namespace Transporter.Data
{
    [Serializable]
    public class GameData
    {
        public PlayerData playerData = new PlayerData();
        public WorldData worldData = new WorldData();


        public DateTime saveTime;
        public string version;
    }
}
