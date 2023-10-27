using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Memory
    {
        public const float MAX_STORAGE_TIME = 100;
        public const float BASE_IMPORTANCE = 5;

        private GameEntity owner;
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

        public Memory(GameEntity owner, int memoryVolume, Func<MemoryInfo, float> calculateDangerFunc = null, Func<MemoryInfo, float> calculateImportanceFunc = null)
        {
            this.owner = owner;
            MemoryVolume = memoryVolume;
            Memories = new List<MemoryInfo>();
            this.calculateDangerFunc = calculateDangerFunc;
            this.calculateImportanceFunc = calculateImportanceFunc;
        }

        public bool IsRemember(GameEntity entity)
        {
            foreach (var memoryInfo in Memories)
            {
                if (memoryInfo.Entities.Any(e => e == entity)) return true;
            }
            return false;
        }

        public bool TryGetMemory(GameEntity entity, out MemoryInfo memory)
        {
            foreach (var memoryInfo in Memories)
            {
                if (memoryInfo.Entities.Any(e => e == entity))
                {
                    memory = memoryInfo;
                    return true;
                }
            }
            memory = null;
            return false;
        }

        public bool TryGetMemory(GraphPointInfo point, out MemoryInfo memory)
        {
            foreach (var memoryInfo in Memories)
            {
                if (memoryInfo.Point == point)
                {
                    memory = memoryInfo;
                    return true;
                }
            }
            memory = null;
            return false;
        }

        public bool TryGetMemoryPosition(GameEntity entity, out GraphPointInfo position)
        {
            var ans = TryGetMemory(entity, out var memory);
            position = ans ? memory.Point : null;
            return ans;
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
                    memoriesCopy[i].DangerLevel += Memories[j].DangerLevel / 2;
                    memoriesCopy[j].DangerLevel += Memories[i].DangerLevel / 2;
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
            return memory.Entities.Aggregate(0f, (danger, entity) => entity.Team.IsEnemy(owner.Team) ? danger + entity.GetDunger() : danger);
        }

        private float CalculateImportance(MemoryInfo memory)
        {
            float timeMultiplier = math.sqrt(1 - math.min(Time.time - memory.LastUpdate, MAX_STORAGE_TIME - 1) / MAX_STORAGE_TIME);
            float placeValue = memory.Entities.Length == 0 ? 0 : 
                memory.Entities.Max(e => !e.Team.IsEnemy(owner.Team) && e is GamePlace place ? place.GetImportance() : 0);
            return timeMultiplier  * (BASE_IMPORTANCE + memory.DangerLevel + placeValue);
        }
    }

    public class MemoryInfo
    {
        public GraphPointInfo Point { get; private set; }
        public GameEntity[] Entities { get; private set; }
        public float LastUpdate { get; private set; }

        public float DangerLevel { get; set; }
        public float ImportanceLevel { get; set; }

        public MemoryInfo(MemoryInfo memory)
        {
            Point = memory.Point;
            Entities = memory.Entities;
            LastUpdate = memory.LastUpdate;
            DangerLevel = memory.DangerLevel;
            ImportanceLevel = memory.ImportanceLevel;
        }

        public MemoryInfo(ScanInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            Point = info.Point;
            Entities = info.Entities.ToArray();
            LastUpdate = Time.time;
            DangerLevel = 0;
            ImportanceLevel = 0;
        }

        public void Update(ScanInfo info)
        {
            if (info.Point.X != Point.X || info.Point.Y != Point.Y)
                throw new ArgumentException($"Wrong scan info! {info.Point} vs {Point}");

            Entities = info.Entities.ToArray();
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