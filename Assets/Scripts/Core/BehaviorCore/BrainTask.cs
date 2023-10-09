namespace Assets.Scripts.Core.BehaviorCore
{
    public class BrainTask
    {
        public BrainTaskType TaskType { get; private set; }
        public object TaskBody { get; private set; }

        public BrainTask(BrainTaskType taskType, object taskBody)
        {
            TaskType = taskType;
            TaskBody = taskBody;
        }
    }
}