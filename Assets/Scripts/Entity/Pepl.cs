using Assets.Scripts.Core;
using System;
using UnityEngine;

namespace Assets.Scripts.Entity
{
    public class Pepl : GameUnit
    {
        [SerializeField] private UnitStatsSheet startingStats;

        public override string Name => "Чиловик " + secondName;
        private string secondName;

        public override string Description => "Базовая еденица людей, способна размножаться, развиватся и менять профессию";

        public override void Initialization(GraphPoint point, Team team)
        {
            base.Initialization(point, team);

            if (Team.Race != Race.Pepl) throw new System.Exception($"{typeof(Pepl).Name} can't be {Team.Race}!");

            Stats = new PeplStats(startingStats, null, 20);
            Brain = BrainBuilder.Slave(this);
            secondName = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        }
    }
}