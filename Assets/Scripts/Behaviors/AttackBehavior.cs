﻿using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Fight" с объектом типа GameEntity
    /// Наносит урон цели, пока та в зоне досягаемости
    /// range определяет дальность атаки, где 1 = длина дороги
    /// cooldown определяет время между атаками
    /// </summary>
    public class AttackBehavior : Behavior
    {
        protected override string ActionName => "Атакую " + target.Name;
        protected override string ShortName => "Атака";

        private float range;
        private float cooldown;
        private GameEntity target;

        public AttackBehavior(GameUnit unit, float range = 0.5f, float cooldown = 1f) : base(unit)
        {
            this.range = range;
            this.cooldown = cooldown;
        }

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Fight && task.TaskBody is GameEntity entity && 
                HexGraph.Graph.IsInRange(Unit.transform.position, entity.transform.position, range);
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            Unit.Brain.Status = BrainStatus.Fight;
            target = task.TaskBody as GameEntity;
            while (Unit != null && target != null && HexGraph.Graph.IsInRange(Unit.transform.position, target.transform.position, range) && target.Health > 0)
            {
                target.Attack(Unit, Unit.Stats.Damage);
                yield return new WaitForSeconds(cooldown);
            }
            yield return null;
        }
    }
}