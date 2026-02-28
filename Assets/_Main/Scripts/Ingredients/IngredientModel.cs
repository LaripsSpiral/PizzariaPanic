using NaughtyAttributes;
using System;
using UnityEngine;

namespace Main.Ingredient
{
    [Serializable]
    public class IngredientModel
    {
        private IngredientController owner;
        private ProcessData processesData => owner.Data.ProcessData;
        private float currProcessTime
        {
            get => owner.currProcessTime.Value;
            set => owner.currProcessTime.Value = value;
        }

        public void Init(IngredientController owner)
        {
            this.owner = owner;
        }

        public void DoProcess(Processor processBy)
        {
            // Must match of processor
            if (processesData.ProcessWith != processBy)
            {
                Debug.Log($"{this} require {processesData.ProcessWith} can't process with {processBy};");
                return;
            }

            // Processed
            if (currProcessTime <= 0)
            {
                owner.SetDataRPC(processesData.ProcessedData.Name);
                return;
            }

            // Decrease process timer;
            Debug.Log($"{this} is Processing");
            currProcessTime -= Time.fixedDeltaTime;
        }

        public bool TryAddIngredient(IngredientController addingIngredient)
        {
            var addingTargetData = addingIngredient.Data;
            var addingTargetType = addingTargetData.Type;

            if (owner.Data.IsCombineAble == false)
                return false;

            // Must add ingredient in dough and Can't add dough in dough
            Debug.Log($"Owner({owner}) {owner.Data.Type} Target({addingIngredient}) {addingTargetType}");
            if (owner.Data.Type != Type.Dough || addingTargetType == Type.Dough)
                return false;

            AddIngredient(addingIngredient);
            return true;
        }

        private void AddIngredient(IngredientController addingIngredient)
        {
            owner.CombinedComponentsId.Add(addingIngredient.DataId);
            addingIngredient.transform.SetParent(owner.transform);
            addingIngredient.transform.localPosition = Vector3.zero;
        }
    }

}