using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    public enum HobbyType { Pintura, Musica, Jardineria, Pesca, Yoga, Cocina }

    [Serializable]
    public class Club
    {
        public string name;
        public HobbyType hobby;
        public List<string> members = new List<string>();
    }

    /// <summary>
    /// Pack Y Sus Hobbies / ¿Quedamos? / Al Caer la Noche: el sim practica un
    /// hobby que entrena una habilidad, y puede fundar o unirse a clubes con
    /// vecinos para reuniones grupales.
    /// </summary>
    public class HobbyClubSystem : MonoBehaviour
    {
        public static HobbyClubSystem Instance { get; private set; }

        public List<Club> clubs = new List<Club>();
        public HobbyType favoriteHobby = HobbyType.Pintura;

        void Awake() { Instance = this; }

        static SkillType SkillFor(HobbyType hobby)
        {
            switch (hobby)
            {
                case HobbyType.Pintura: return SkillType.Creatividad;
                case HobbyType.Musica: return SkillType.Creatividad;
                case HobbyType.Jardineria: return SkillType.Logica;
                case HobbyType.Pesca: return SkillType.Fitness;
                case HobbyType.Yoga: return SkillType.Fitness;
                default: return SkillType.Cocina;
            }
        }

        /// <summary>Practicar el hobby favorito. Llamar mientras el jugador hace la actividad.</summary>
        public void Practice(float xpPerCall = 3f)
        {
            SkillSystem.Instance?.GainXP(SkillFor(favoriteHobby), xpPerCall);
            NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.2f);
        }

        public Club CreateClub(string name, HobbyType hobby)
        {
            var club = new Club { name = name, hobby = hobby };
            club.members.Add("Jugador");
            clubs.Add(club);
            AchievementSystem.Instance?.Unlock("club");
            return club;
        }

        public bool JoinClub(string clubName, string npcName)
        {
            var club = clubs.Find(c => c.name == clubName);
            if (club == null || club.members.Contains(npcName)) return false;
            club.members.Add(npcName);
            return true;
        }

        /// <summary>Reunion de club: sube Social y la habilidad del hobby.</summary>
        public void HoldMeeting(string clubName)
        {
            var club = clubs.Find(c => c.name == clubName);
            if (club == null) return;
            NeedsSystem.Instance?.Recover(NeedType.Social, 0.5f);
            SkillSystem.Instance?.GainXP(SkillFor(club.hobby), 5f);
        }
    }
}
