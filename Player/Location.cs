using System.Collections.Generic;
using UnityEngine;


namespace Transporter.World
{

    [System.Serializable]
    public struct Location
    {
        public string Id;
        public string Name;
        public Vector3 WorldPosition;


        public Location(string id, string name, Vector3 position)
        {
            Id = id;
            Name = name;
            WorldPosition = position;
        }

    }
}