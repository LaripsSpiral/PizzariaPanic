using Main.Ingredient;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;

namespace Main.Recipe
{
    [CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
    public class RecipeSO : ScriptableObject
    {
        [ReadOnly]
        public string ID;
        public string DisplayName;
        public Sprite Image;

        [SerializeField]
        private List<IngredientSO> ingredientSOList = new();
        public List<FixedString32Bytes> IngredientsIDList = new();

        private void OnValidate()
        {
            ID = name;
            DisplayName ??= name;

            BakeIngredient();
        }

        [Button]
        private void BakeIngredient()
        {
            IngredientsIDList = ingredientSOList
                .Where(iso => iso != null) // Safety check for empty slots in Inspector
                .Select(iso => (FixedString32Bytes)iso.Name)
                .ToList();
        }
    }
}