using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Memory
    {
        const float MAX_STORAGE_TIME = 10;
        const float MAX_VISITS_COUNT = 5;

        private Team ownerTeam;
        private int memoryVolume;

        private Func<MemoryInfo, float> calculateDangerFunc;
        private Func<MemoryInfo, float> calculateImportanceFunc;

        public int MemoryVolume
        {
            get => memoryVolume;
            set
            {
                if (value < 1) { throw new ArgumentException("Can't be zero or less", nameof(MemoryVolume)); }
                memoryVolume = value;
            }
        }

        public event EventHandler MemoryChanged;

        public List<MemoryInfo> Memories { get; private set; }

        public Memory(Team team, int memoryVolume, Func<MemoryInfo, float> calculateDangerFunc = null, Func<MemoryInfo, float> calculateImportanceFunc = null)
        {
            ownerTeam = team;
            MemoryVolume = memoryVolume;
            Memories = new List<MemoryInfo>();
            this.calculateDangerFunc = calculateDangerFunc;
            this.calculateImportanceFunc = calculateImportanceFunc;
        }

        public void LoadInfo(IEnumerable<ScanInfo> infos)
        {
            foreach (var info in infos)
            {
                if (Memories.Any(i => i.Point == info.Point))
                {
                    Memories.First(i => i.Point == info.Point).Update(info);
                }
                else
                {
                    Memories.Add(new MemoryInfo(info));
                }
            }
            UpdateDanger();
            UpdateImportance();
            ForgotMemories();
            MemoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ExchangeMemories(IEnumerable<MemoryInfo> infos)
        {
            foreach (var info in infos)
            {
                if (Memories.Any(i => i.Point == info.Point))
                {
                    Memories.First(i => i.Point == info.Point).Update(info);
                }
                else
                {
                    Memories.Add(info);
                }
            }
            UpdateDanger();
            UpdateImportance();
            ForgotMemories();
            MemoryChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateDanger()
        {
            // Первая итерация
            var calculateFunc = calculateDangerFunc ?? CalculateDanger;
            Memories.ForEach(memory => memory.DangerLevel = calculateFunc(memory));

            // Вторая итерация
            var memoriesCopy = Memories.Select(m => new MemoryInfo(m)).ToList();
            for (var i = 0; i < Memories.Count - 1; i++)
            {
                for (var j = i; j < Memories.Count; j++)
                {
                    var pointA = Memories[i].Point;
                    var pointB = Memories[j].Point;
                    if (HexGraph.Graph.GetGraphEdge(pointA.X, pointA.Y, pointB.X, pointB.Y) == null) continue;
                    memoriesCopy[i].DangerLevel += Memories[j].DangerLevel;
                    memoriesCopy[j].DangerLevel += Memories[i].DangerLevel;
                }
            }
            Memories = memoriesCopy;
        }

        private void UpdateImportance()
        {
            var calculateFunc = calculateImportanceFunc ?? CalculateImportance;
            Memories.ForEach(memory => memory.ImportanceLevel = calculateFunc(memory));
        }

        private void ForgotMemories()
        {
            while (Memories.Count > memoryVolume)
            {
                Memories.Remove(Memories.Aggregate((min, memory) => memory.ImportanceLevel < min.ImportanceLevel ? memory : min));
            }
        }

        private float CalculateDanger(MemoryInfo memory)
        {
            return memory.Entities.Aggregate(0f, (danger, entity) => entity.Team.IsEnemy(ownerTeam) ? danger + entity.GetDunger() : danger);
        }

        private float CalculateImportance(MemoryInfo memory)
        {
            float timeMultiplier = math.sqrt(1 - math.min(Time.time - memory.LastUpdate, MAX_STORAGE_TIME) / MAX_STORAGE_TIME);
            float visitsMultiplier = math.sqrt(1 - math.min(memory.VisitCount, MAX_VISITS_COUNT) / MAX_VISITS_COUNT);
            float placeValue = memory.Entities.Length == 0 ? 0 : 
                memory.Entities.Max(e => !e.Team.IsEnemy(ownerTeam) && e is GamePlace place ? place.GetImportance() : 0);
            return timeMultiplier * visitsMultiplier * (1 + memory.DangerLevel + placeValue);
        }
    }

    public class MemoryInfo
    {
        public GraphPointInfo Point { get; private set; }
        public GameEntity[] Entities { get; private set; }
        public int VisitCount { get; private set; }
        public float LastUpdate { get; private set; }

        public float DangerLevel { get; set; }
        public float ImportanceLevel { get; set; }

        public MemoryInfo(MemoryInfo memory)
        {
            Point = memory.Point;
            Entities = memory.Entities;
            VisitCount = memory.VisitCount;
            LastUpdate = memory.LastUpdate;
            DangerLevel = memory.DangerLevel;
            ImportanceLevel = memory.ImportanceLevel;
        }

        public MemoryInfo(ScanInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            Point = info.Point;
            Entities = info.Entities.ToArray();
            VisitCount = 1;
            LastUpdate = Time.time;
            DangerLevel = 0;
            ImportanceLevel = 0;
        }

        public void Update(ScanInfo info)
        {
            if (info.Point.X != Point.X || info.Point.Y != Point.Y)
                throw new ArgumentException($"Wrong scan info! {info.Point} vs {Point}");

            Entities = info.Entities.ToArray();
            VisitCount++;
            LastUpdate = Time.time;
        }

        public void Update(MemoryInfo info)
        {
            if (info.LastUpdate < LastUpdate) return;
            if (info.Point.X != Point.X || info.Point.Y != Point.Y)
                throw new ArgumentException($"Wrong scan info! {info.Point} vs {Point}");

            Entities = info.Entities;
            LastUpdate = Time.time;
        }
    }
}