using Assets.Scripts.Core;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts
{
    public class MemoryDrawer : MonoBehaviour
    {
        [SerializeField] GameObject memoryPrefab;

        private Memory memory;

        private void Start()
        {
            if (Application.isPlaying && (memoryPrefab == null || memoryPrefab.GetComponent<SpriteRenderer>() == null))
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

        public void DrawMemory(Memory memory)
        {
            if (memory != null) memory.MemoryChanged -= OnDrawMemory;

            this.memory = memory;
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
            if (memory != sender) throw new Exception($"{typeof(MemoryDrawer)} has old memory!");
            if (!Application.isPlaying) return;
            Clear();

            foreach (var memoryInfo in memory.Memories)
            {
                var pointObj = Instantiate(memoryPrefab, transform);

                pointObj.transform.position = new Vector3(memoryInfo.Point.PosX, memoryInfo.Point.PosY);

                float size = math.min(memoryInfo.VisitCount, 10) / 10;
                pointObj.transform.localScale = new Vector3(size, size, 1);

                var renderer = pointObj.GetComponent<SpriteRenderer>();
                var safe = 1 - math.min(memoryInfo.DangerLevel, 5) / 5;
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