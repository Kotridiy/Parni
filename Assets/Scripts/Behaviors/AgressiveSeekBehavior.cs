using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Movement" с объектом типа GameUnit
    /// Подходит к выбранному объекту, пока не окажется в зоне досягаемости
    /// Используется в сочитании с StepMovement
    /// safe заставляет избегать врагов, сильнее себя
    /// range определяет дальность атаки, где 1 = длина дороги 
    /// </summary>
    public class AgressiveSeekBehavior : SeekBehavior
    {
        private float range;
        private bool safe;

        public AgressiveSeekBehavior(GameUnit unit, bool safe = false, float range = 0.5f) : base(unit)
        {
            this.range = range;
            this.safe = safe;
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            target = task.TaskBody as GameUnit;
            while (Unit != null && target != null && !HexGraph.Graph.IsInRange(Unit.transform.position, target.transform.position, range))
            {
                Unit.Scan();
                if (Unit.Memory.TryGetMemory(target, out var info) && (!safe || info.DangerLevel <= Unit.GetDunger()))
                { 
                    if (target.OnRoad != null && target.Point != Unit.Point)
                    {
                        yield return new WaitUntil(() => !HexGraph.Graph.IsInRange(Unit.transform.position, target.transform.position, range));
                    }
                    else
                    {
                        yield return CreateTask(BrainTaskType.Movement, target.Point);
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
