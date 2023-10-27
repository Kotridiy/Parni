using UnityEngine;

namespace Assets.Scripts.Entity
{
    [CreateAssetMenu(fileName = "Unit stats", menuName = "Custom/Create Unit Stats", order = 2)]
    public class UnitStatsSheet : ScriptableObject
    {
        public int maxHealth = 10;
        public float damage = 1;
        public float range = 0.5f;
        public float attackSpeed = 1;
        public int visualRange = 1;
        public int memoryVolume = 10;
        public float caution = 1;
        public int speedMultiplier = 1;
        public int weight = 3;
        public int worth = 1;
    }
}