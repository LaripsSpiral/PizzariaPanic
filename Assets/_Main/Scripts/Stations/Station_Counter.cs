using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Station_Counter : BaseStation
{
    [SerializeField]
    private bool holdingInteract;

    private void FixedUpdate()
    {
        if (holdingInteract)
            HandleProcess();
    }

    public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleInteract(inputCtx, character);

        // QoL - Quick access to place down, any of interaction
        if (character.ItemHolder.CurrItem && !ItemHolder.CurrItem)
        {
            PlayerPickPlaceInteract(character);
            return;
        }

        switch (inputCtx.interaction)
        {
            case HoldInteraction:
                holdingInteract = true;
                break;

            default:
                PlayerPickPlaceInteract(character);
                break;
        }
    }

    public override void HandleCancelInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleCancelInteract(inputCtx, character);

        holdingInteract = false;
    }

    private void HandleProcess()
    {
        Debug.Log("Try Processing");

        // No item to process
        var item = ItemHolder.CurrItem;
        if (!item || !item.TryGetComponent(out PizzaComponents pizzaComponents))
            return;

        Debug.Log("Processing");
        pizzaComponents.IngredientController.DoProcess(Processor.Counter);
    }

}
