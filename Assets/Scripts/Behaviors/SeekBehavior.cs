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
    public class SeekBehavior : Behavior
    {
        protected override string ActionName => "Преследую " + target.Name;
        protected override string ShortName => "Преследование";

        protected GameUnit target;

        public SeekBehavior(GameUnit unit) : base(unit)
        {
        }

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Movement && task.TaskBody is GameUnit;
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            target = task.TaskBody as GameUnit;
            while (Unit != null && target != null && !HexGraph.Graph.IsNear(Unit, target))
            {
                Unit.Scan();
                if (target.OnRoad != null && target.Point != Unit.Point)
                {
                    yield return new WaitUntil(() => Unit.Point != target.Point);
                }
                else
                {
                    yield return CreateTask(BrainTaskType.Movement, target.Point);
                }
            }
        }
    }
}
