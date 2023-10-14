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
    /// </summary>
    public class AgressiveSeekBehavior : SeekBehavior
    {
        private float range;

        public AgressiveSeekBehavior(GameUnit unit, float range = 0.5f) : base(unit)
        {
            this.range = range;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            target = task.TaskBody as GameUnit;
            while (Unit != null && target != null && !HexGraph.Graph.IsInRange(Unit.transform.position, target.transform.position, range))
            {
                Unit.Scan();
                if (target.OnRoad != null && target.Point != Unit.Point)
                {
                    yield return new WaitUntil(() => !HexGraph.Graph.IsInRange(Unit.transform.position, target.transform.position, range));
                }
                else
                {
                    yield return CreateTask(BrainTaskType.Movement, target.Point);
                }
            }
        }
    }
}
