using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Spawner", menuName = "Custom/Create Spawner", order = 1)]
    public class Spawner : ScriptableObject
    {
        public bool active = true;
        public GameEntity entity;
        public Vector2Int spawnPosition;
    }
}