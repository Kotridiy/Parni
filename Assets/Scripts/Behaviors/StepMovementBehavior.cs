using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Movement" с объектом типа Point
    /// Делает один шаг в направлении к точке по кратчайшему маршруту
    /// </summary>
    public class StepMovementBehavior : MovementBehavior
    {
        private List<GraphPoint> way;

        public StepMovementBehavior(GameUnit unit) : base(unit)
        {
        }

        public override bool CanRunTask(BrainTask task)
        {
            return base.CanRunTask(task) && task.Behavior is SeekBehavior;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            var destination = task.TaskBody as GraphPoint;
            if (destination == null)
                throw new Exception("Move task don't have point.");

            int wayIndex = way != null ? way.IndexOf(Unit.Point) : -1;
            if (movingTarget == destination && wayIndex > -1)
            {
                var point = way[wayIndex+1];
                if (point != Unit.Point) yield return MovingToNextPoint(point);
            }

            if (destination != Unit.Point)
            {
                movingTarget = destination;
                way = HexGraph.Graph.FindShortestWay(GetRoadSpeed, Unit.Point, destination);
                if (way != null)
                {
                    var point = way[1];
                    if (point != Unit.Point) yield return MovingToNextPoint(point);
                }
                movingTarget = null;
            }
        }
    }
}
