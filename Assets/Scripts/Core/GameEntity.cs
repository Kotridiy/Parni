﻿using UnityEngine;

namespace Assets.Scripts.Core
{
    public abstract class GameEntity : MonoBehaviour
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Info { get; }
        public abstract void Initialization(GraphPoint point, Team team);
        public abstract float GetDunger();
        public GraphPoint Point { get; protected set; }
        public Team Team { get; protected set; }
        public Memory Memory { get; protected set; }

        public bool IsEnemy(GameEntity entity)
        {
            return Team.IsEnemy(entity.Team);
        }
    }
}