using NaughtyAttributes;
using System;
using UnityEngine;

namespace Main.Ingredient
{
    [CreateAssetMenu(fileName = "IngredientSO", menuName = "Scriptable Objects/IngredientSO")]
    public class IngredientSO : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private string id;
        public string Name => id;

        [SerializeField]
        private Type type;
        public Type Type => type;

        [SerializeField]
        private GameObject meshPrefab;
        public GameObject Prefab => meshPrefab;

        private void OnValidate()
        {
            id = name;
        }

        [SerializeField]
        private ProcessData processData;
        public ProcessData ProcessData => processData;

        [SerializeField]
        private bool isCombineAble;
        public bool IsCombineAble => isCombineAble;
    }

    public enum Type
    {
        None, Dough, Sauce, Topping
    }
}