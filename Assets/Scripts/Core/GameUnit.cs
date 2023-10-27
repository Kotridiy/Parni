using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public abstract class GameUnit : GameEntity
    {
        [SerializeField] protected float ScanInterval = 1;

        [SerializeField] protected UnitSpriteController spriteController;

        public IUnitStats Stats { get; protected set; }
        public override int MaxHealth => Stats.MaxHealth;
        public override float Health => Stats.Health;

        public UnitSpriteController SpriteController { get => spriteController; }

        public GraphEdge OnRoad
        {
            get; private set;
        }
        public Brain Brain { get; protected set; }

        public override void Initialization(GraphPoint point, Team team)
        {
            Point = point;
            transform.position = new Vector3(point.PosX, point.PosY, transform.position.z);
            point.AddUnit(this);

            Team = team;
            Memory = new Memory(this, 10);
            Brain = new Brain(new List<Behavior>(), this);
        }

        public override void Attack(GameEntity attacker, float damage)
        {
            Stats.Health -= damage;
            if (Stats.Health <= 0)
            {
                StartCoroutine(Die());
                return;
            }
            Brain.BecameAttacked(attacker);
        }

        private IEnumerator Die()
        {
            yield return new WaitForEndOfFrame();
            Stats.Health = 0;
            Destroy(gameObject);
            Point.RemoveUnit(this);
        }

        public void StartMoveOnRoad(GraphEdge road)
        {
            if (OnRoad != null) throw new Exception($"{Name} on road {OnRoad} now!");

            if (road.First != Point && road.Second != Point) throw new Exception($"Road {road} don't have point {Point}");

            OnRoad = road;
        }

        public void EndMoveRoad()
        {
            Point.RemoveUnit(this);
            Point = OnRoad.Second == Point ? OnRoad.First : OnRoad.Second;
            Point.AddUnit(this);
            OnRoad = null;
        }

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

            var scanInfos = HexGraph.Graph.ScanPoint(Point, Stats.VisualRange).ToList();
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
            StartCoroutine(Brain.StartThink());
        }

        public override string Info
        {
            get
            {
                var strb = new StringBuilder();
                strb.AppendLine(Name);
                //strb.AppendLine(Description);
                strb.AppendLine($"Команда - {Team}");
                strb.AppendLine($"Сила = {Stats.Damage}");
                strb.AppendLine($"{Stats.Health}/{Stats.MaxHealth} ХП");
                strb.AppendLine(Brain.ActiveBehavior.FullName);
                //if (lastScanInfos == null) Scan();
                //var visibleObjects = lastScanInfos.Aggregate(new List<GameEntity>(), (entities, info) => { entities.AddRange(info.Entities); return entities; });
                //strb.AppendLine($"Видно объектов - {visibleObjects.Count}");
                //strb.AppendLine($"Помню объектов - {Memory.Memories.Count} из {Memory.MemoryVolume}");
                return strb.ToString();
            }
        }

        public override float GetDunger()
        {
            return Stats.Damage * Stats.AttackSpeed * Stats.Health * Mathf.Sqrt(Stats.Range);
        }
    }
}