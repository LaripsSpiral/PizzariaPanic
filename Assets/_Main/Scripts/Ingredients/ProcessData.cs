using System;

namespace Main.Ingredient
{
    public enum Processor
    {
        None, Counter, Oven
    }

    [Serializable]
    public struct ProcessData
    {
        public Processor ProcessWith;
        public float ProcessTime;
        public IngredientSO ProcessedData;
    }
}