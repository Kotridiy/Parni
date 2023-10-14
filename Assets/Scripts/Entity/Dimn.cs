using Assets.Scripts.Core;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class Dimn : GameUnit
    {
        public override string Name => "Димн";

        public override string Description => "Базовый демон, ходит и ест всё подряд";

        public override float Power => 3;

        public override void Initialization(GraphPoint point, Team team)
        {
            base.Initialization(point, team);

            if (Team.Race != Race.Dimn) throw new System.Exception($"{typeof(Dimn).Name} can't be {Team.Race}!");
            Brain = BrainBuilder.Bully(this);
            Memory = new Memory(Team, MemoryVolume, null, CalculateImportance);
        }

        private float CalculateImportance(MemoryInfo memory)
        {
            float timeMultiplier = math.sqrt(1 - math.min(Time.time - memory.LastUpdate, Memory.MAX_STORAGE_TIME / 4 - 1) / Memory.MAX_STORAGE_TIME / 4);
            float placeValue = memory.Entities.Length == 0 ? 0 :
                memory.Entities.Max(e => !e.Team.IsEnemy(Team) && e is GamePlace place ? place.GetImportance() : 0);
            return timeMultiplier * (1 + memory.DangerLevel * 2 + placeValue);
        }
    }
}