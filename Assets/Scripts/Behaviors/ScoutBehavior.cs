using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public class ScoutBehavior : Behavior
    {
        public ScoutBehavior(GameUnit unit) : base(unit)
        {
        }

        public override string ActionName => "Рыскает вокруг";

        public override bool CanRunTask(BrainTask task)
        {
            return task.TaskType == BrainTaskType.Main;
        }

        public override IEnumerator RunTask(BrainTask task)
        {
            UnityEngine.Debug.Log($"{Unit.Name} обыскивает местность.");
            int movements = 0;
            while (true)
            {

                if (movements < 5)
                {
                    yield return BehaviorHelper.CheckStatus(Unit);

                    GraphPoint nextPoint;
                    do
                    {
                        int x = Unit.Point.X + Random.Range(-4, 5);
                        int y = Unit.Point.Y + Random.Range(-4, 5);
                        nextPoint = HexGraph.Graph.GetGraphPoint(x, y);
                    } while (nextPoint == null);
                    // 1. Пойти в случайное место
                    yield return Unit.Brain.CreateTask(new BrainTask(BrainTaskType.Movement, nextPoint));

                    yield return BehaviorHelper.CheckStatus(Unit);

                    // 2. Подождать немного
                    yield return new WaitForSeconds(2);
                    movements++;
                }
                else
                {
                    // 3. Вернутся домой
                    BrainTask nextTask = null;
                    if (!Unit.Memory.Memories.Any(info => info.Entities.Any(e => e is GamePlace)))
                    {
                        // Но сперва попытатся найти его
                        nextTask = new BrainTask(BrainTaskType.Search, typeof(GamePlace));
                        yield return Unit.Brain.CreateTask(nextTask);

                        yield return BehaviorHelper.CheckStatus(Unit);
                    }

                    GraphPointInfo homePoint = Unit.Memory.Memories.FirstOrDefault(info => info.Entities.Any(e => e is GamePlace))?.Point;
                    if (homePoint != null)
                    {
                        UnityEngine.Debug.Log($"{Unit.Name} идёт домой.");
                        nextTask = new BrainTask(BrainTaskType.Movement, HexGraph.Graph.GetGraphPoint(homePoint.X, homePoint.Y));
                        yield return Unit.Brain.CreateTask(nextTask);
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"{Unit.Name} не знает где дом :<");
                    }
                    movements = 0;
                }
            }
        }
    }
}
