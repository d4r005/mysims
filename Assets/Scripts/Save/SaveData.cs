using System;
using System.Collections.Generic;

namespace MySims
{
    [Serializable]
    public class SaveData
    {
        public int day;
        public float hour;
        public float money;
        public JobData job;
        public string currentLocationId;
        public string homeLocationId;
        public string skinColor, hairColor, shirtColor, pantsColor;
        public Vector3Serializable playerPosition;
        public List<NeedData> needs = new List<NeedData>();
        public List<SkillData> skills = new List<SkillData>();
        public List<PlacedObjectData> placedObjects = new List<PlacedObjectData>();
        public List<WallData> walls = new List<WallData>();
        public List<string> achievements = new List<string>();
    }

    [Serializable]
    public class JobData
    {
        public string name;
        public float startHour, endHour, hourlyPay;
        public bool worksWeekend;
    }

    [Serializable]
    public class SkillData { public int type; public int level; public float xp; }

    [Serializable]
    public class WallData
    {
        public string prefabName;
        public Vector3Serializable position;
        public float rotationY;
        public Vector3Serializable scale;
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
