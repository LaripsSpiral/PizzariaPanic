using Main.Ingredient;
using Main.Order;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Station
{
    public class Station_Trash : BaseStation
    {
        public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
        {
            base.HandleInteract(inputCtx, character);
            PlayerPickPlaceInteract(character);

            if (ItemHolder.HoldingNetObj)
            {
                Trash();
            }
        }

        private void Trash()
        {
            if (!ItemHolder.HoldingNetObj.gameObject)
                return;

            Destroy(ItemHolder.HoldingNetObj.gameObject);
        }
    }
}