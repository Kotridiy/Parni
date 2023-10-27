using System.Collections;

namespace Assets.Scripts.Core.BehaviorCore
{
    public abstract class Behavior
    {
        public string FullName { get => (Task?.Behavior != null ? Task.Behavior.LongName : "") + ActionName; }
        protected string LongName { get => (Task?.Behavior != null ? Task.Behavior.LongName : "") + ShortName + " -> "; }
        protected abstract string ActionName { get; }
        protected abstract string ShortName { get; }

        protected GameUnit Unit { get; private set; }
        protected BrainTask Task { get; set; }

        public Behavior(GameUnit unit)
        {
            Unit = unit;
        }

        public abstract bool CanRunTask(BrainTask task);
        public IEnumerator StartTask(BrainTask task)
        {
            Task = task;
            return RunTask(task);
        }
        protected abstract IEnumerator RunTask(BrainTask task);

        protected IEnumerator CreateTask(BrainTask task) => Unit.Brain.CreateTask(task);
        protected IEnumerator CreateTask(BrainTaskType type, object body) => Unit.Brain.CreateTask(new BrainTask(type, body, this));
    }
}