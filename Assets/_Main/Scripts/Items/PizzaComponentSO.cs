using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PizzaComponentSO", menuName = "Scriptable Objects/PizzaComponentSO")]
public class PizzaComponentSO : ScriptableObject
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

public enum Processor
{
    None, Counter, Oven
}
