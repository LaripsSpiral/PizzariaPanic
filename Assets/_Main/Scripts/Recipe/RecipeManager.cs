using System.Collections.Generic;
using UnityEngine;

namespace Main.Recipe
{
    public class RecipeManager : MonoBehaviour
    {
        public static RecipeManager Instance;

        public List<RecipeSO> recipes = new();

        private void Awake()
        {
            Instance = this;
        }
    }
}