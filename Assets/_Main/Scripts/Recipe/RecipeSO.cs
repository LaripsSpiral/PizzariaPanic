using Main.Ingredient;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Recipe
{
    [CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
    public class RecipeSO : ScriptableObject
    {
        [ReadOnly]
        public string ID;
        public string DisplayName;
        public Sprite Image;

        public List<IngredientSO> ingredients;

        private void OnValidate()
        {
            ID = name;
            DisplayName ??= name;
        }
    }
}