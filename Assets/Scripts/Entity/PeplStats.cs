using Assets.Scripts.Core;
using System;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class PeplStats : IUnitStats
    {
        const int AVERAGE_LIFESPAN = 50;
        const int LAST_YEARS = 5;
        const int AGE_OF_MAJORITY = 18;


        public int MaxHealth => Mathf.CeilToInt(statsSheet.maxHealth * Gens.Endurance * Mathf.Sqrt(Gens.Strength) * LevelFactor);

        public float Health { get => health; set => health = Mathf.Clamp(value, 0, MaxHealth); }
        public float HealthPercent => Health / MaxHealth;

        public int Lifespan => Mathf.RoundToInt(AVERAGE_LIFESPAN * Mathf.Sqrt(Gens.Endurance));
        public float Age { get; private set; }
        public PeplAgeCategory AgeCategory
        {
            get
            {
                if (Age < AGE_OF_MAJORITY) return PeplAgeCategory.Child;
                if (Age > Lifespan - LAST_YEARS) return PeplAgeCategory.Old;
                return PeplAgeCategory.Adult;
            }
        }

        public float Damage
        {
            get
            {
                float powerMultiplier;
                switch (MainStat)
                {
                    case PeplMainStat.Strength:
                        powerMultiplier = Gens.Strength;
                        break;
                    case PeplMainStat.Agility:
                        powerMultiplier = Gens.Agility;
                        break;
                    case PeplMainStat.Intellect:
                        powerMultiplier = Gens.Intelligence;
                        break;
                    default:
                        powerMultiplier = 1;
                        break;
                }
                return statsSheet.damage * powerMultiplier * LevelFactor;
            }
        }
        public float Range => MainStat == PeplMainStat.Strength ? 0.5f : statsSheet.range * Gens.Perception * Mathf.Sqrt(Gens.Agility);

        public float AttackSpeed => statsSheet.attackSpeed * Gens.Agility * Mathf.Sqrt(Gens.Courage) * Mathf.Sqrt(LevelFactor);

        public int VisualRange => Mathf.RoundToInt(statsSheet.visualRange * Gens.Perception);

        public int MemoryVolume => Mathf.RoundToInt(statsSheet.visualRange * Gens.Intelligence * Mathf.Sqrt(Gens.Perception));

        public float ProgressFactor => statsSheet.visualRange * Gens.Intelligence;

        public float Caution => statsSheet.caution / Gens.Courage * Mathf.Sqrt(Gens.Intelligence);

        public float SpeedMultiplier => statsSheet.speedMultiplier * Mathf.Sqrt(Load/LoadCapacity);

        public int Weight => Mathf.RoundToInt(statsSheet.weight / Mathf.Sqrt(Gens.Agility));

        public int Load { get; set; }
        public int LoadCapacity => Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(Gens.Strength) * 10) - Weight);

        public int Worth => statsSheet.worth * Level;

        public int Wealth { get; set; }


        public PeplGens Gens { get; private set; }
        public PeplMainStat MainStat { get; private set; }
        public int Level { get; private set; }

        private UnitStatsSheet statsSheet;
        private float health;
        private float LevelFactor => (Level + 2) / 3f; // С ростом уровня растут алгебраически растут боевые показатели с х1 на первом до х4 на 10-ом уровне

        public PeplStats(UnitStatsSheet statsSheet, PeplGens gens, float age, PeplMainStat mainStat = PeplMainStat.Strength)
        {
            this.statsSheet = statsSheet ?? throw new ArgumentNullException(nameof(statsSheet));
            Gens = gens ?? new PeplGens();
            MainStat = mainStat;
            Level = 1;

            if (age < 0 || age > Lifespan) throw new ArgumentException("Incorrect age - " + age, nameof(age));

            health = MaxHealth;
        }
    }

    public enum PeplMainStat
    {
        Strength,
        Agility,
        Intellect
    }

    public enum PeplAgeCategory
    {
        Child,
        Adult,
        Old
    }
}
