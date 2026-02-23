using System.Collections;
using Unity.Netcode;
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
            HandleProcessRPC();
    }

    public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleInteract(inputCtx, character);

        // QoL - Quick access to place down, any of interaction
        if (character.ItemHolder && !ItemHolder.HoldingNetObj)
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

    [Rpc(SendTo.Server)]
    private void HandleProcessRPC()
    {
        Debug.Log("Try Processing");

        // No item to process
        var item = ItemHolder.HoldingNetObj;
        if (!item || !item.TryGetComponent(out PizzaComponentController pizzaComponents))
            return;

        Debug.Log("Processing");
        pizzaComponents.Model.DoProcess(Processor.Counter);
    }

}
