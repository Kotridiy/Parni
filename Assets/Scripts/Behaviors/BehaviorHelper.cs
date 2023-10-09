using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections;

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
    }
}
