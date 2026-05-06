using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Main.Ingredient
{
    public class ProcessProgressUI : MonoBehaviour
    {
        [SerializeField]
        private Image progressImage;

        public void Setup(float currVal, float maxVal)
        {
            if (currVal == 0 || maxVal == 0)
            {
                progressImage.enabled = false;
                return;
            }

            progressImage.enabled = true;
            progressImage.fillAmount = currVal / maxVal;
        }
    }
}