using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using Assets.Scripts.Entity;
using System.Collections;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Основное поведение
    /// Ищет ближайшего человика, подходит к нему и бьёт
    /// </summary>
    public class BullyBehavior : Behavior
    {
        public override string ActionName => "Терроризирует население";

        public BullyBehavior(GameUnit unit) : base(unit)
        {
        }

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Main;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            UnityEngine.Debug.Log($"{Unit.Name} ищет кого бы побить.");
            GameUnit target = null;
            while (Unit != null)
            {
                if (target == null)
                {
                    Unit.Scan(true);
                    var memory = Unit.Memory.Memories.Aggregate(GetCloserEnemy);
                    while (memory == null || memory.Entities.Length == 0)
                    {
                        yield return CreateTask(BrainTaskType.Search, typeof(Pepl));
                        memory = Unit.Memory.Memories.Aggregate(GetCloserEnemy);
                    }
                    target = memory.Entities.First(e => e is Pepl) as GameUnit;
                }

                while (target != null && Unit.Memory.TryGetMemoryPosition(target, out GraphPointInfo position))
                {
                    if (!target.Point.Equals(position) || !(HexGraph.Graph.IsInRange(target.transform.position, Unit.transform.position)))
                    {
                        yield return CreateTask(BrainTaskType.Movement, target);
                    }

                    yield return CreateTask(BrainTaskType.Fight, target);
                    target = null;
                }
            }
        }

        private MemoryInfo GetCloserEnemy (MemoryInfo current, MemoryInfo next)
        {
            if (next.Entities.Any(e => e is Pepl) &&
                Vector2.Distance(next.Point.Pos, (Vector2)Unit.transform.position) < Vector2.Distance(current.Point.Pos, (Vector2)Unit.transform.position))
            {
                return next;
            }
            else
            {
                return current;
            }
        }
    }
}