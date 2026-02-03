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

        if (holdingItem)
            return;

        PickupIngredient(character);
    }

    private void PickupIngredient(PlayerCharacter character)
    {
        if (character.HoldingObject != null)
            return;

        // Spawn Network Object
        var instanceObject = Instantiate(ingredientPrefab);
        instanceObject.NetworkObject.Spawn();
        character.Pickup(instanceObject.gameObject);
    }
}
