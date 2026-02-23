using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class Station_IngredientContainer : BaseStation
{
    [SerializeField]
    private PizzaComponentSO ingredientData;

    [SerializeField]
    private PizzaComponentController pizzaComponentPrefab;

    public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleInteract(inputCtx, character);

        PlayerPickPlaceInteract(character);

        if (ItemHolder.HoldingNetObj)
            return;

        if (character.ItemHolder.HoldingNetObj != null)
            return;

        SpawnNetworkIngredientRPC();
        character.ItemHolder.SwapItemFromHolder(ItemHolder);
    }

    [Rpc(SendTo.Server)]
    private void SpawnNetworkIngredientRPC()
    {
        var instanceNetworkObj = Instantiate(pizzaComponentPrefab).NetworkObject;
        instanceNetworkObj.Spawn();

        if (instanceNetworkObj.TryGetComponent(out PizzaComponentController pizzaComponent))
        {
            pizzaComponent.SetDataRPC(ingredientData.Name);
            ItemHolder.HoldItemRPC(instanceNetworkObj);
        }
    }
}
