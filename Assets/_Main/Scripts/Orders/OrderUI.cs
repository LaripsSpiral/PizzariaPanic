using Main.Recipe;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Order.UI
{
    public class OrderUI : MonoBehaviour
    {
        [SerializeField]
        private OrderPaper orderPaperPrefab;

        [SerializeField]
        private Transform container;

        [SerializeField, ReadOnly]
        private List<OrderPaper> orderPaperList = new();

        public void Add(RecipeSO recipeSO)
        {
            var newOrder = Instantiate(orderPaperPrefab, container);
            newOrder.Init(recipeSO);

            orderPaperList.Add(newOrder);
        }

        public void Remove(string recipeID)
        {
            var targetOrder = orderPaperList.Find(order => order.ID == recipeID);
            if (targetOrder == null)
                return;

            orderPaperList.Remove(targetOrder);
        }

        public void Clear()
        {
            orderPaperList.ForEach(item => Destroy(item.gameObject));
            orderPaperList.Clear();
        }
    }
}