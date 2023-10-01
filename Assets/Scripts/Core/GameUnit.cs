using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class GameUnit : GameEntity
    {
        [SerializeField] protected int VisualRange = 1;
        [SerializeField] protected float ScanInterval = 1;
        [SerializeField] protected int MemoryVolume = 10;

        public GraphEdge OnRoad { get; private set; }
        public virtual float Power { get; }

        private GraphPoint movingTarget;
        private MovingState movingState;
        private Coroutine changeMoveCoroutine;

        public override void Initialization(GraphPoint point, Team team)
        {
            Point = point;
            transform.position = new Vector3(point.PosX, point.PosY, transform.position.z);
            point.AddUnit(this);

            Team = team;
            Memory = new Memory(team, MemoryVolume);
        }

        #region Moving
        public void Move(GraphPoint point)
        {
            if (changeMoveCoroutine != null) StopCoroutine(changeMoveCoroutine);

            changeMoveCoroutine = StartCoroutine(BeforeMoving(point));
        }

        private IEnumerator BeforeMoving(GraphPoint point)
        {
            if (movingTarget != null && point != movingTarget)
            {
                movingState = MovingState.ChangeTarget;
                yield return new WaitWhile(() => movingState != MovingState.Stand);
            }
            if (movingState == MovingState.Stand && point != Point)
            {
                StartCoroutine(MovingToDestination(point));
                movingTarget = point;
                movingState = MovingState.Moving;
            }
        }

        private IEnumerator MovingToDestination(GraphPoint destination)
        {
            var way = HexGraph.Graph.FindShortestWay(GetRoadSpeed, Point, destination);
            if (way != null)
            {
                foreach (var point in way)
                {
                    if (movingState == MovingState.ChangeTarget) break;
                    if (point == Point) continue;
                    yield return MovingToNextPoint(point);
                }
            }
            movingTarget = null;
            movingState = MovingState.Stand;
        }

        private IEnumerator MovingToNextPoint(GraphPoint point)
        {
            if (OnRoad != null)
            {
                throw new InvalidOperationException($"{Name} are moving on road {OnRoad} while trying reach {point} from {Point}");
            }

            OnRoad = HexGraph.Graph.GetGraphEdge(Point, point);
            float movingTime = GetRoadSpeed(OnRoad.Level);
            float timeLeft = 0;
            var startPosition = transform.position;
            var destinationPosition = new Vector3(point.PosX, point.PosY, transform.position.z);

            while (timeLeft < movingTime)
            {
                transform.position = Vector3.Lerp(startPosition, destinationPosition, timeLeft / movingTime);
                timeLeft += Time.deltaTime;
                yield return null;
            }

            Point.RemoveUnit(this);
            point.AddUnit(this);
            Point = point;
            transform.position = destinationPosition;
            OnRoad = null;
            Scan(true);
        }

        private float GetRoadSpeed(EdgeLevel roadLevel)
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

        enum MovingState
        {
            Stand,
            Moving,
            Wait,
            ChangeTarget
        }
        #endregion

        #region Scanning
        private ScanInfo[] lastScanInfos = null;
        private float lastScanTime = 0;

        public ScanInfo[] Scan(bool ignoreTime = false)
        {
            if (!ignoreTime &&
                lastScanInfos != null &&
                Time.realtimeSinceStartup - lastScanTime < ScanInterval)
            {
                return lastScanInfos;
            }

            lastScanTime = Time.realtimeSinceStartup;

            var scanInfos = HexGraph.Graph.ScanPoint(Point, VisualRange).ToList();
            foreach (var info in scanInfos)
            {
                if (Point.Equals(info.Point))
                {
                    info.Entities = info.Entities.Where(e => e != this);
                }
            }
            lastScanInfos = scanInfos.ToArray();
            Memory.LoadInfo(lastScanInfos);

            return lastScanInfos;
        }
        #endregion

        public virtual void Start()
        {
            if (Point == null) throw new System.Exception($"{Name} не привязан к графу.");
            movingState = MovingState.Stand;
        }

        public override string Info
        {
            get
            {
                var strb = new StringBuilder();
                strb.AppendLine(Name);
                //strb.AppendLine(Description);
                //strb.AppendLine($"Позиция - {Point}");
                strb.AppendLine($"Команда - {Team}");
                if (lastScanInfos == null) Scan();
                var visibleObjects = lastScanInfos.Aggregate(new List<GameEntity>(), (entities, info) => {entities.AddRange(info.Entities); return entities;});
                strb.AppendLine($"Видно объектов - {visibleObjects.Count}");
                foreach ( var obj in visibleObjects)
                {
                    strb.AppendLine($"--{obj.Name}, {obj.Point}--");
                }
                return strb.ToString();
            }
        }

        public override float GetDunger()
        {
            return Power;
        }
    }
}