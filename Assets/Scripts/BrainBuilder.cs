using Assets.Scripts.Behaviors;
using Assets.Scripts.Core;
using Assets.Scripts.Core.BehaviorCore;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class BrainBuilder
    {
        public static Brain Slave(GameUnit unit)
        {
            List<Behavior> brainBehaviors = new List<Behavior>
            {
                new SlaveBehavior(unit),
                new MovementBehavior(unit),
            };

            return new Brain(brainBehaviors, unit);
        }

        public static Brain Scout(GameUnit unit)
        {
            List<Behavior> brainBehaviors = new List<Behavior>
            {
                new ScoutBehavior(unit),
                new MovementBehavior(unit),
            };

            return new Brain(brainBehaviors, unit);
        }

        public static Brain Bully(GameUnit unit)
        {
            List<Behavior> brainBehaviors = new List<Behavior>
            {
                new BullyBehavior(unit),
                new SearchBehavior(unit),
                new AgressiveSeekBehavior(unit),
                new StepMovementBehavior(unit),
                new MovementBehavior(unit),
                new AttackBehavior(unit),
            };

            return new Brain(brainBehaviors, unit);
        }

        //...
    }
}
