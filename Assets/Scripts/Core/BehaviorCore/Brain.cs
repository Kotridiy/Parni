using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.BehaviorCore
{
    public partial class Brain
    {
        private const int MAX_DEPTH = 10;

        public BrainStatus Status { get; set; }
        public Behavior ActiveBehavior { get; private set; }
        public BanList BanList { get; private set; }

        private IEnumerable<Behavior> behaviors;
        private GameUnit unit;

        private BrainTask awaiterTask;
        private EventHandler awaiterCallback;
        private int tasksDepth = 0;

        public Brain(IEnumerable<Behavior> behaviors, GameUnit unit)
        {
            this.behaviors = behaviors ?? throw new ArgumentNullException(nameof(behaviors));
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit));

            BanList = new BanList();
            Status = BrainStatus.None;
        }

        public IEnumerator StartThink()
        {
            Status = BrainStatus.Normal;
            return CreateTask(new BrainTask(BrainTaskType.Main, null));
        }

        public IEnumerator CreateTask(BrainTask task)
        {
            foreach (var behavior in behaviors)
            {
                if (behavior.CanRunTask(task))
                {
                    yield return RunTask(task, behavior);
                    yield break;
                }    
            }

            yield return new WaitForSeconds(5);
        }

        private IEnumerator RunTask(BrainTask task, Behavior behavior)
        {
            if (tasksDepth >= MAX_DEPTH)
            {
                Status = BrainStatus.Restart;
                yield break;
            }

            tasksDepth++;
            var previousBehavior = ActiveBehavior;
            ActiveBehavior = behavior;

            yield return behavior.RunTask(task);

            ActiveBehavior = previousBehavior;
            tasksDepth--;
        }

        public bool TrySayCommand(BrainTask task, EventHandler callback)
        {
            return OutterInteract(task, callback, BrainStatus.Slave);
        }

        public bool TryExchange(BrainTask task, EventHandler callback)
        {
            return OutterInteract(task, callback, BrainStatus.Exchange);
        }

        public bool TryFlee(GraphPoint point, EventHandler callback)
        {
            return OutterInteract(new BrainTask(BrainTaskType.Movement, point), callback, BrainStatus.Flee);
        }

        public void BecameAttacked(GameEntity attacker)
        {
            OutterInteract(new BrainTask(BrainTaskType.Fight, attacker), null, BrainStatus.Fight);
        }

        private bool OutterInteract(BrainTask task, EventHandler callback, BrainStatus status)
        {
            if ((int)status > (int)Status)
            {
                Status = status;
                awaiterTask = task;
                awaiterCallback = callback;
                return true;
            }
            return false;
        }

        public IEnumerator Interrupt()
        {
            yield return CreateTask(awaiterTask);

            Status = BrainStatus.Normal;
            awaiterCallback(unit, new EventArgs());
        }
    }

    public enum BrainStatus
    {
        None = 0,
        Normal,
        Exchange,
        Fight,
        Flee,
        Slave,
        Restart,
    }
}
