using Assets.Scripts.Core;

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
        }
    }
}