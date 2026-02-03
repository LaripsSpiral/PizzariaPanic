using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientController
{
    [SerializeField, ReadOnly, AllowNesting]
    private float currProcessTime;

    [SerializeField, ReadOnly, AllowNesting]
    private ProcessIngredientData currentProcessData;

    private LinkedList<ProcessIngredientData> processesLinkedList;

    [SerializeField]
    private ProcessIngredientData[] processesDataArray;

    public void Init()
    {
        processesLinkedList = new(processesDataArray);
        ChangeStage(processesLinkedList.First.Value);
    }

    public void DoProcess(Processor processBy)
    {
        // Must match of processor
        if (currentProcessData.processWith != processBy)
        {
            Debug.Log($"{this} process not match;");
            return;
        }

        // Change to next process
        if (currProcessTime <= 0)
        {
            // Last process
            if (currentProcessData == processesLinkedList.Last.Value)
                return;

            NextStage();
            return;
        }

        // Decrease process timer;
        Debug.Log($"{this} is Processing");
        currProcessTime -= Time.fixedDeltaTime;
    }

    private void NextStage()
    {
        Debug.Log($"{this} changed to next stage;");

        var newProcess = processesLinkedList.Find(currentProcessData).Next.Value;
        if (currentProcessData != default)
        {
            currentProcessData.ProcessedObject.gameObject.SetActive(false);
        }

        ChangeStage(newProcess);
    }

    private void ChangeStage(ProcessIngredientData newProcessData)
    {
        currentProcessData = newProcessData;
        currentProcessData.ProcessedObject.gameObject.SetActive(true);
        currProcessTime = currentProcessData.ProcessTime;
    }
}

[Serializable]
class ProcessIngredientData
{
    public Processor processWith;
    public float ProcessTime;
    public Transform ProcessedObject;
}

public enum Processor
{
    None, Counter, Oven
}
