using Main.Recipe;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Order.UI
{
    public class OrderPaper : MonoBehaviour
    {
        public string ID { get; private set; }

        [SerializeField]
        private Image[] ingredientImages;

        public void Init(RecipeSO recipeSO)
        {
            // Defensive checks
            if (recipeSO == null)
            {
                Debug.LogWarning("OrderPaper.Init called with null RecipeSO.");
                DisableAllImages();
                return;
            }

            var ingredientList = recipeSO.IngredientList;
            if (ingredientList == null)
            {
                Debug.LogWarning("OrderPaper.Init: RecipeSO.IngredientList is null.");
                DisableAllImages();
                return;
            }

            ID = recipeSO.ID;

            for (int i = 0; i < ingredientImages.Length; i++)
            {
                var img = ingredientImages[i];
                if (img == null)
                {
                    continue;
                }

                if (i >= ingredientList.Count)
                {
                    img.sprite = null;
                    img.enabled = false;
                    continue;
                }

                img.sprite = ingredientList[i].Icon;
                img.enabled = true;
            }
        }

        private void DisableAllImages()
        {
            if (ingredientImages == null)
            {
                return;
            }

            for (int i = 0; i < ingredientImages.Length; i++)
            {
                var img = ingredientImages[i];
                if (img == null)
                {
                    continue;
                }

                img.sprite = null;
                img.enabled = false;
            }
        }
    }
}