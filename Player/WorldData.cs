using System;


namespace Transporter.Data
{
    [Serializable]
    public class WorldData
    {
        public int worldSeed = 0;
        public float gameTime = 0f;

        // Extend later: discovered ports, weather state, economy snapshot, etc.
    }
}
