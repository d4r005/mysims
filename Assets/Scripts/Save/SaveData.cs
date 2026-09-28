using System;
using System.Collections.Generic;

namespace MySims
{
    [Serializable]
    public class SaveData
    {
        public int day;
        public float hour;
        public Vector3Serializable playerPosition;
        public List<NeedData> needs = new List<NeedData>();
        public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
    }

    [Serializable]
    public class NeedData { public int type; public float value; }

    [Serializable]
    public class PlacedObjectData
    {
        public string prefabName;
        public Vector3Serializable position;
        public float rotationY;
    }

    [Serializable]
    public class Vector3Serializable { public float x, y, z; }
}
