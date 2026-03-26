using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Main.Ingredient
{
    public class IngredientView : MonoBehaviour
    {
        private GameObject mainView;
        public GameObject MainView => mainView;

        private Dictionary<FixedString32Bytes, GameObject> viewByID = new();
        
        public GameObject LoadView(IngredientSO data)
        {
            var go = Instantiate(data.Prefab, transform, false);
            AddView(data.name, go);
            mainView = go;

            return go;
        }

        public void AddView(FixedString32Bytes id, GameObject go)
        {
            if (viewByID.ContainsKey(id))
                return;

            go.transform.SetParent(transform);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            viewByID.Add(id, go);
        }

        public void RemoveView(FixedString32Bytes id)
        {
            if (!viewByID.TryGetValue(id, out var go))
                return;

            Destroy(go);
            viewByID.Remove(id);
        }

        public void UpdateCookedView()
        {
            Debug.Log($"{this}: Updating Cooked View");

            // Make a snapshot
            var keys = viewByID.Keys.ToList();

            foreach (var key in keys)
            {
                if (!viewByID.TryGetValue(key, out var oldView))
                    continue;

                var data = IngredientController.LoadData(key).Result;
                if (data == null || data.CookedPrefab == null)
                {
                    Debug.LogWarning($"{this}: {data?.Name ?? key.ToString()} don't have cooked prefab");
                    continue;
                }

                // Instantiate cooked
                var instanceCooked = Instantiate(data.CookedPrefab, oldView.transform.parent);
                instanceCooked.transform.SetParent(oldView.transform.parent);
                instanceCooked.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                // Remove old
                Destroy(oldView);

                // Set new
                viewByID[key] = instanceCooked;
            }

            Debug.Log($"{this}: Updated Cooked View");
        }
    }
}