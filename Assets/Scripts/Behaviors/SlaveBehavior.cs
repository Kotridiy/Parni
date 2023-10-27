using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    /// <summary>
    /// Основное поведение
    /// Ничего не делает, пока не прикажут
    /// </summary>
    public class SlaveBehavior : Behavior
    {
        public SlaveBehavior(GameUnit unit) : base(unit)
        {
        }

        protected override string ActionName => "Ждёт команды";
        protected override string ShortName => "Исполнение";

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Main;
        }

        protected override IEnumerator RunTask(BrainTask task)
        {
            UnityEngine.Debug.Log($"{Unit.Name} готов вкалывать.");
            while (Unit != null)
            {
                if (Unit.Brain.Status != BrainStatus.Normal)
                {
                    yield return Unit.Brain.Interrupt();
                }
                else
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
}
