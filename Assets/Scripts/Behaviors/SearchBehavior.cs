using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Решает задачи типа "Search" с объектом типа Type
    /// Рыскает вокруг, пока не заметит объект нужного типа
    /// Внимание, объект должен влезть в память!
    /// </summary>
    public class SearchBehavior : Behavior
    {
        protected override string ActionName => "Ищет " + target.Name;
        protected override string ShortName => "Поиск";

        private Type target = null;

        public SearchBehavior(GameUnit unit) : base(unit)
        {
        }

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Search && task.TaskBody is Type;
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            target = task.TaskBody as Type;
            while (Unit != null)
            {
                GraphPoint nextPoint = BehaviorHelper.GetRandomPoint(Unit, 2);
                yield return CreateTask(BrainTaskType.Movement, nextPoint);
                if (ScanAndCheck(task.TaskBody as Type)) yield break;
                yield return new WaitForSeconds(1);
                if (ScanAndCheck(task.TaskBody as Type)) yield break;
            }
        }

        protected bool ScanAndCheck(Type type)
        {
            if (type == null) return false;
            Unit.Scan();
            return Unit.Memory.Memories.Any(info => info.Entities.Any(e => type.IsAssignableFrom(e.GetType())));
        } 
    }
}
