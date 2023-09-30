using System.Collections;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class GamePlace : GameEntity
    {
        public override void Initialization(GraphPoint point, Team team)
        {
            if (point.GamePlace != null)
                throw new System.InvalidOperationException($"Ошибка при размещении {Name} в {point}, он уже находится в точке [{Point.X}, {Point.Y}]");
            Point = point;
            transform.position = new Vector3(point.PosX, point.PosY, transform.position.z);
            point.GamePlace = this;

            Team = team;
            Memory = new Memory(team, 100); // TODO
        }

        public virtual void Start()
        {
            if (Point == null) throw new System.Exception($"{Name} не привязан к графу.");
        }

        public override string Info
        {
            get
            {
                var strb = new StringBuilder();
                strb.AppendLine(Name);
                strb.AppendLine(Description);
                strb.AppendLine($"Позиция - [{Point.X}, {Point.Y}]");
                return strb.ToString();
            }
        }

        // Для врагов
        public override float GetDunger()
        {
            return 50; // TODO-
        }

        // Для друзей
        public virtual float GetImportance()
        {
            return 50;
        }
    }
}