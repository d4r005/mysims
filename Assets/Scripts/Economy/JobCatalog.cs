using System;
using System.Collections.Generic;
using UnityEngine;

namespace MySims
{
    [Serializable]
    public class JobOffer
    {
        public Job job;
        [Tooltip("Habilidad requerida para poder aceptar el empleo")]
        public SkillType requiredSkill;
        public int requiredLevel = 1;
    }

    /// <summary>
    /// Catálogo de empleos disponibles con requisitos de habilidad.
    /// El menú de empleos consulta este catálogo y JobSystem acepta el cambio.
    /// </summary>
    public class JobCatalog : MonoBehaviour
    {
        public static JobCatalog Instance { get; private set; }

        public List<JobOffer> offers = new List<JobOffer>
        {
            new JobOffer { job = new Job { name = "Aseador",       hourlyPay = 10f }, requiredSkill = SkillType.Fitness,      requiredLevel = 1 },
            new JobOffer { job = new Job { name = "Mesero",        hourlyPay = 15f }, requiredSkill = SkillType.Carisma,     requiredLevel = 2 },
            new JobOffer { job = new Job { name = "Cocinero",      hourlyPay = 22f }, requiredSkill = SkillType.Cocina,      requiredLevel = 3 },
            new JobOffer { job = new Job { name = "Contador",     hourlyPay = 30f }, requiredSkill = SkillType.Logica,       requiredLevel = 4 },
            new JobOffer { job = new Job { name = "Artista local", hourlyPay = 35f }, requiredSkill = SkillType.Creatividad,  requiredLevel = 5 }
        };

        void Awake() { Instance = this; }

        public bool MeetsRequirements(JobOffer offer)
        {
            return SkillSystem.Instance != null
                && SkillSystem.Instance.GetLevel(offer.requiredSkill) >= offer.requiredLevel;
        }
    }
}
