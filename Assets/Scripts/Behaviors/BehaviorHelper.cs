using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public static class BehaviorHelper
    {
        public static IEnumerator CheckStatus(GameUnit unit)
        {
            if (unit.Brain.Status != BrainStatus.Normal)
            {
                yield return unit.Brain.Interrupt();
            }
        }

        public static GraphPoint GetRandomPoint(GameUnit unit, int range)
        {
            GraphPoint nextPoint;
            do
            {
                int x = unit.Point.X + Random.Range(-range, range + 1);
                int y = unit.Point.Y + Random.Range(-range, range + 1);
                nextPoint = HexGraph.Graph.GetGraphPoint(x, y);
            } while (nextPoint == null);
            return nextPoint;
        }
    }
}
