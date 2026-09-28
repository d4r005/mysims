using UnityEngine;

namespace MySims
{
    public enum VacationActivityType { Snorkel, TomarElSol, Excursion, Souvenirs, CheckInHotel, Esqui }

    /// <summary>
    /// Pack Bon Voyage / Vida Isleña / Aventura en la Isla / A la Aventura:
    /// actividades especiales de viaje. Colocar en zonas de playa, montaña o
    /// exploracion junto a su punto de interaccion.
    /// </summary>
    public class VacationActivity : MonoBehaviour
    {
        public VacationActivityType activityType;
        public Transform interactionPoint;
        public float cost;

        public bool TryDo()
        {
            if (cost > 0f && (EconomySystem.Instance == null || !EconomySystem.Instance.TrySpend(cost))) return false;

            switch (activityType)
            {
                case VacationActivityType.Snorkel:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.4f);
                    SkillSystem.Instance?.GainXP(SkillType.Fitness, 4f);
                    break;
                case VacationActivityType.TomarElSol:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.25f);
                    NeedsSystem.Instance?.Recover(NeedType.Energia, 0.15f);
                    break;
                case VacationActivityType.Excursion:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.5f);
                    SkillSystem.Instance?.GainXP(SkillType.Fitness, 6f);
                    AchievementSystem.Instance?.Unlock("explorador");
                    break;
                case VacationActivityType.Souvenirs:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.1f);
                    break;
                case VacationActivityType.CheckInHotel:
                    NeedsSystem.Instance?.Recover(NeedType.Energia, 0.6f);
                    NeedsSystem.Instance?.Recover(NeedType.Higiene, 0.5f);
                    break;
                case VacationActivityType.Esqui:
                    NeedsSystem.Instance?.Recover(NeedType.Diversión, 0.45f);
                    SkillSystem.Instance?.GainXP(SkillType.Fitness, 5f);
                    break;
            }
            return true;
        }
    }
}
