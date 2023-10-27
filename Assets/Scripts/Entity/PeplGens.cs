using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Entity
{
    public class PeplGens
    {
        const int GENS_COUNT = 6;
        const int EVOLVE_COUNT = 3;
        const float EVOLVE_POWER = 0.2f;

        public float Strength { get => gens[0].Value; }
        public float Endurance { get => gens[1].Value; }
        public float Agility { get => gens[2].Value; }
        public float Perception { get => gens[3].Value; }
        public float Intelligence { get => gens[4].Value; }
        public float Courage { get => gens[5].Value; }

        private readonly Gen[] gens;

        public PeplGens()
        {
            gens = new Gen[GENS_COUNT];
            for (int i = 0; i < gens.Length; i++)
            {
                gens[i] = new Gen(1);
            }
        }

        public PeplGens(float[] values)
        {
            if (values.Length != GENS_COUNT) throw new ArgumentException($"Array should have {GENS_COUNT} params");
            gens = new Gen[GENS_COUNT];
            for (int i = 0; i < gens.Length; i++)
            {
                gens[i] = new Gen(values[i]);
            }
        }

        public PeplGens(PeplGens parent1, PeplGens parent2)
        {
            gens = new Gen[GENS_COUNT];
            for (int i = 0; i < gens.Length; i++)
            {
                float minValue, maxValue;
                if (parent1.gens[i].Value < parent2.gens[i].Value)
                {
                    minValue = parent1.gens[i].Value;
                    maxValue = parent2.gens[i].Value;
                }
                else
                {
                    minValue = parent2.gens[i].Value;
                    maxValue = parent1.gens[i].Value;
                }
                gens[i] = new Gen(Random.Range(minValue, maxValue));
            }

            for (int i = 0; i < EVOLVE_COUNT; i++)
            {
                EvolveGen(Random.Range(0, GENS_COUNT), Random.Range(-1f, 1f) * EVOLVE_POWER);
            }
        }

        private void EvolveGen(int genNum, float amount)
        {
            if (genNum < 0 || genNum >= GENS_COUNT) throw new ArgumentOutOfRangeException(nameof(genNum));

            for (int i = 0; i < gens.Length; i++)
            {
                if (i == genNum) gens[i] = new Gen(gens[i].Value + amount);
                else gens[i] = new Gen(gens[i].Value - amount/(GENS_COUNT - 1));
            }

            float sumDiff = GENS_COUNT - gens.Sum(g => g.Value);
            if (sumDiff != 0)
            {
                for (int i = 0; i < gens.Length; i++)
                {
                    gens[i] = new Gen(gens[i].Value + sumDiff / GENS_COUNT);
                }
            }
        }
    }

    internal readonly struct Gen
    {
        public readonly float Value;

        public Gen(float value)
        {
            if (value < 0.5f) value = 0.5f;
            if (value > 2) value = 2;
            Value = value;
        }
    }
}
