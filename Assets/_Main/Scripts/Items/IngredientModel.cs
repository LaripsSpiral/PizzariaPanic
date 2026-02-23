using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class IngredientModel
{
    private PizzaComponentController owner;
    private ProcessData processesData => owner.Data.ProcessData;
    private float currProcessTime
    {
        get => owner.currProcessTime.Value;
        set => owner.currProcessTime.Value = value;
    }

    public void Init(PizzaComponentController owner)
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

    public bool TryAddIngredient(PizzaComponentController targetParent)
    { 
        var parentData = targetParent.Data;
        var parentType = parentData.Type;

        if (parentData.IsCombineAble == false)
            return false;

        // Must add ingredient in dough and Can't add dough in dough
        if (parentType != Type.Dough || owner.Data.Type == Type.Dough)
            return false;

        AddIngredientTo(targetParent);
        return true;
    }

    private void AddIngredientTo(PizzaComponentController targetParent)
    {
        owner.CombinedComponentsId.Add(targetParent.DataId);
        targetParent.transform.SetParent(owner.transform, false);
    }
}

