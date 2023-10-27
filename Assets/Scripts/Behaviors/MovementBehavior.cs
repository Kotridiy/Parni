using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Movement" с объектом типа Point
    /// Идёт к точке по кратчайшему маршруту
    /// safeCoefficient заставляет выбирать более безопасный маршрут
    /// </summary>
    public class MovementBehavior : Behavior
    {
        private const float TIME_FACTOR = 1f;

        protected GraphPoint movingTarget;
        protected float safeCoefficient;

        protected override string ActionName => "Идёт к " + movingTarget;
        protected override string ShortName => "Движение";

        public MovementBehavior(GameUnit unit, float safeCoefficient = 0) : base(unit)
        {
            this.safeCoefficient = safeCoefficient;
        } 

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Movement && task.TaskBody is GraphPoint;
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            var destination = task.TaskBody as GraphPoint;
            if (destination == null)
                throw new Exception("Move task don't have point.");

            if (destination != Unit.Point)
            {
                movingTarget = destination;
                var way = GetWay();
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

        protected virtual List<GraphPoint> GetWay()
        {
            if (safeCoefficient > 0) return HexGraph.Graph.FindSafeWay(GetRoadSpeed, Unit.Point, movingTarget, Unit.Memory, safeCoefficient);
            else return HexGraph.Graph.FindShortWay(GetRoadSpeed, Unit.Point, movingTarget);
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