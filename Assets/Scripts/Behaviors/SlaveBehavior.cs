using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public class SlaveBehavior : Behavior
    {
        public SlaveBehavior(GameUnit unit) : base(unit)
        {
        }

        public override string ActionName => "Ждёт команды";

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Main;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            UnityEngine.Debug.Log($"{Unit.Name} готов вкалывать.");
            while (true)
            {
                if (Unit.Brain.Status != BrainStatus.Normal)
                {
                    yield return Unit.Brain.Interrupt();
                } 
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }
        }
    }
}
