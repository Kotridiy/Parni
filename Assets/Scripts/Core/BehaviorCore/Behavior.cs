using System.Collections;

namespace Assets.Scripts.Core.BehaviorCore
{
    public abstract class Behavior
    {
        public abstract string ActionName { get; }
        protected GameUnit Unit { get; private set; }

        public Behavior(GameUnit unit)
        {
            Unit = unit;
        }

        public abstract bool CanRunTask(BrainTask task);
        public abstract IEnumerator RunTask(BrainTask task);
    }
}