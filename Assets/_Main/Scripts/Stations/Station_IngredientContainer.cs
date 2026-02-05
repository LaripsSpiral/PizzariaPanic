using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Station_IngredientContainer : BaseStation
{
    [SerializeField]
    private PizzaComponents ingredientPrefab;

    public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleInteract(inputCtx, character);

        PlayerPickPlaceInteract(character);

        if (ItemHolder.CurrItem)
            return;

        if (character.ItemHolder.CurrItem != null)
            return;

        SpawnNetworkIngredientRPC();
        character.ItemHolder.SwapItemFromHolder(ItemHolder);
    }

    [Rpc(SendTo.Server)]
    private void SpawnNetworkIngredientRPC()
    {
        var instanceItem = Instantiate(ingredientPrefab);

        var networkInstance = instanceItem.NetworkObject;
        networkInstance.Spawn();

        ItemHolder.HoldItemRPC(networkInstance);
    }
}
