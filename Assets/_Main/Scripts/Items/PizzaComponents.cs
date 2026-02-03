using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PizzaComponents : NetworkBehaviour
{
    [SerializeField]
    private IngredientController ingredientController;
    public IngredientController IngredientController => ingredientController;

    public Transform Transform => Transform;

    protected virtual void Start()
    {
        ingredientController.Init();
    }

    public virtual void AddInto(PizzaBase pizza)
    {

    }
}
