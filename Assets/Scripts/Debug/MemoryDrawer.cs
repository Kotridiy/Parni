using Assets.Scripts.Core;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Debug
{
    public class MemoryDrawer : MonoBehaviour
    {
        [SerializeField] GameObject memoryPrefab;

        private Memory memory;
        private GameEntity owner;

        private void Start()
        {
            if (Application.isPlaying && (memoryPrefab == null || memoryPrefab.GetComponentInChildren<SpriteRenderer>() == null))
                throw new Exception($"{typeof(MemoryDrawer)} has no actual memory prefab!");
        }

        private void OnEnable()
        {
            if (memory != null) memory.MemoryChanged += OnDrawMemory;
        }

        private void OnDisable()
        {
            if (memory != null) memory.MemoryChanged -= OnDrawMemory;
        }

        public void StartDraw(Memory memory, GameEntity owner)
        {
            if (!enabled) return;
            if (this.memory != null) this.memory.MemoryChanged -= OnDrawMemory;

            this.memory = memory;
            this.owner = owner;
            memory.MemoryChanged += OnDrawMemory;
            OnDrawMemory(memory, EventArgs.Empty);
        }

        public void StopDraw()
        {
            if (memory == null) return;

            Clear();
            memory.MemoryChanged -= OnDrawMemory;
            memory = null;
        }

        private void OnDrawMemory(object sender, EventArgs e)
        {
            if (memory != sender) 
                throw new Exception($"{typeof(MemoryDrawer)} has old memory!");
            if (!Application.isPlaying) return;
            Clear();
            Draw();
        }

        private void Draw()
        {
            foreach (var memoryInfo in memory.Memories)
            {
                var pointObj = Instantiate(memoryPrefab, transform);

                pointObj.transform.position = new Vector3(memoryInfo.Point.PosX, memoryInfo.Point.PosY, pointObj.transform.position.z);

                float size = (5 + math.min(memoryInfo.ImportanceLevel, 20)) / 25f;
                pointObj.transform.localScale = new Vector3(size, size, 1);

                var renderer = pointObj.GetComponentInChildren<SpriteRenderer>();
                var maxSafeFactor = 5 * owner.GetDunger();
                var safe = 1 - math.min(memoryInfo.DangerLevel, maxSafeFactor) / maxSafeFactor;
                renderer.color = new Color(1, safe, safe);

            }
        }

        private void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }
    }
}