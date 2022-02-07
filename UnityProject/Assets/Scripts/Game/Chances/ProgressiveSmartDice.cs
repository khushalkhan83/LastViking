using UnityEngine;

namespace Chances
{
    public class ProgressiveSmartDice : IProgressiveSmartDice
    {
        private readonly float baseChance;
        private readonly float chanceStep;

        public ProgressiveSmartDice(float baseChance, float chanceStep)
        {
            Chance = baseChance;

            this.baseChance = baseChance;
            this.chanceStep = chanceStep;
        }

        public float Chance { get; private set; }

        public bool? LastResult { get; private set; }
        public int ResultCombo { get; private set; }

        public bool GetRollResult()
        {
            bool success = UnityEngine.Random.value <= Chance;
            RegisterResult(success);
            CalculateChance();
            return success;
        }

        private void RegisterResult(bool result)
        {
            if (!LastResult.HasValue)
            {
                LastResult = result;
                ResultCombo++;
            }
            else
            {
                if (LastResult == result)
                {
                    ResultCombo++;
                }
                else
                {
                    LastResult = result;
                    ResultCombo = 0;
                }
            }
        }

        private void CalculateChance()
        {
            var newChance = baseChance;
            var modifier = chanceStep * ResultCombo;

            if (LastResult.Value)
            {
                newChance -= modifier;
            }
            else
            {
                newChance += modifier;
            }

            Chance = Mathf.Clamp01(newChance);
        }
    }
}