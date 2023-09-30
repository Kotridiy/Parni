using Assets.Scripts.Core;

namespace Assets.Scripts.Entity
{
    public class Pepl : GameUnit
    {
        public override string Name => "Чиловик";

        public override string Description => "Базовая еденица людей, способна размножаться, развиватся и менять профессию";

        public override float Power => 1;

        public override void Initialization(GraphPoint point, Team team)
        {
            base.Initialization(point, team);

            if (Team.Race != Race.Pepl) throw new System.Exception($"{typeof(Pepl).Name} can't be {Team.Race}!");
        }
    }
}