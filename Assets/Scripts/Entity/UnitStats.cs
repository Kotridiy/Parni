using Assets.Scripts.Core;
using System;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class UnitStats : IUnitStats
    {
        public int MaxHealth => statsSheet.maxHealth;

        public float Health { get => health; set => health = Mathf.Clamp(value, 0, MaxHealth); }
        public float HealthPercent => Health / MaxHealth;

        public float Damage => statsSheet.damage;

        public float Range => statsSheet.range;

        public float AttackSpeed => statsSheet.attackSpeed;

        public int VisualRange => statsSheet.visualRange;

        public int MemoryVolume => statsSheet.memoryVolume;

        public float ProgressFactor { get; private set; }

        public float Caution => statsSheet.caution;

        public float SpeedMultiplier => statsSheet.speedMultiplier;

        public int Weight => statsSheet.weight;

        public int Worth => statsSheet.worth;

        private float health;
        private UnitStatsSheet statsSheet;

        public UnitStats(UnitStatsSheet statsSheet, float progressFactor = 1)
        {
            this.statsSheet = statsSheet ?? throw new ArgumentNullException(nameof(statsSheet));
            ProgressFactor = progressFactor;
            health = MaxHealth;
        }
    }
}
