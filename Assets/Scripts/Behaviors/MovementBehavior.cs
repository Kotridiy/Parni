using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Movement" с объектом типа Point
    /// Идёт к точке по кратчайшему маршруту
    /// </summary>
    public class MovementBehavior : Behavior
    {
        private const float TIME_FACTOR = 1f;

        protected GraphPoint movingTarget;

        public override string ActionName => movingTarget != null ? string.Format($"Идёт к {movingTarget}") : "Движение";

        public MovementBehavior(GameUnit unit) : base(unit) { }

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Movement && task.TaskBody is GraphPoint;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            var destination = task.TaskBody as GraphPoint;
            if (destination == null)
                throw new Exception("Move task don't have point.");

            if (destination != Unit.Point)
            {
                movingTarget = destination;
                var way = HexGraph.Graph.FindShortestWay(GetRoadSpeed, Unit.Point, destination);
                if (way != null)
                {
                    foreach (var point in way)
                    {
                        //if (movingState == MovingState.ChangeTarget) break;
                        if (point == Unit.Point) continue;
                        yield return MovingToNextPoint(point);
                        Unit.Scan();
                    }
                }
                movingTarget = null;
            }
        }

        protected IEnumerator MovingToNextPoint(GraphPoint point)
        {
            if (Unit.OnRoad != null)
            {
                throw new InvalidOperationException($"{Unit.Name} are moving on road {Unit.OnRoad} while trying reach {point} from {Unit.Point}");
            }

            Unit.StartMoveOnRoad(HexGraph.Graph.GetGraphEdge(Unit.Point, point));

            float movingTime = GetRoadSpeed(Unit.OnRoad.Level) * TIME_FACTOR;
            float timeLeft = 0;
            var startPosition = Unit.transform.position;
            var destinationPosition = new Vector3(point.PosX, point.PosY, Unit.transform.position.z);

            while (timeLeft < movingTime)
            {
                Unit.transform.position = Vector3.Lerp(startPosition, destinationPosition, timeLeft / movingTime);
                timeLeft += Time.deltaTime;
                yield return null;
            }

            Unit.EndMoveRoad();
            Unit.transform.position = destinationPosition;
            Unit.Scan(true);
        }

        protected float GetRoadSpeed(EdgeLevel roadLevel)
        {
            switch (roadLevel)
            {
                case EdgeLevel.Foorpath:
                    return 0.8f;
                case EdgeLevel.Walkway:
                    return 0.6f;
                case EdgeLevel.Road:
                case EdgeLevel.Highway:
                    return 0.5f;
                case EdgeLevel.None:
                default:
                    return 2;
            }
        }
    }
}