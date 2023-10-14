namespace Assets.Scripts.Core.BehaviorCore
{
    public class BrainTask
    {
        public BrainTaskType TaskType { get; private set; }
        public object TaskBody { get; private set; }
        public Behavior Behavior { get; private set; }

        public BrainTask(BrainTaskType taskType, object taskBody, Behavior behavior)
        {
            TaskType = taskType;
            TaskBody = taskBody;
            Behavior = behavior;
        }
    }
}