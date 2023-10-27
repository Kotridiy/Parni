using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class GamePlace : GameEntity
    {
        [SerializeField] protected int maxHealth;
        protected float health;

        public override int MaxHealth => maxHealth;
        public override float Health => health;

        public override void Initialization(GraphPoint point, Team team)
        {
            if (point.GamePlace != null)
                throw new System.InvalidOperationException($"Ошибка при размещении {Name} в {point}, он уже находится в точке [{Point.X}, {Point.Y}]");
            Point = point;
            transform.position = new Vector3(point.PosX, point.PosY, transform.position.z);
            point.GamePlace = this;

            Team = team;
            Memory = new Memory(this, 100); // TODO
            maxHealth = maxHealth > 0 ? maxHealth : 100;
            health = maxHealth;
        }

        public override void Attack(GameEntity attacker, float damage)
        {
            health -= damage;
            if (Health <= 0)
            {
                health = 0;
                Destroy(gameObject);
            }
        }

        public virtual void Start()
        {
            if (Point == null) throw new System.Exception($"{Name} не привязан к графу.");
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 1);
        }

        public override string Info
        {
            get
            {
                var strb = new StringBuilder();
                strb.AppendLine(Name);
                strb.AppendLine(Description);
                strb.AppendLine($"Позиция - {Point}");
                strb.AppendLine($"Команда - {Team}");
                return strb.ToString();
            }
        }

        // Для врагов
        public override float GetDunger()
        {
            return Point.GetUnits().Sum(u => u.GetDunger()) + Health;
        }

        // Для друзей
        public virtual float GetImportance()
        {
            return 50; //TODO
        }
    }
}