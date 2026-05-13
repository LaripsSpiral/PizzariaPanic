using Main.Ingredient;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Station
{
    public class Station_Oven : BaseStation
    {
        private void FixedUpdate()
        {
            if (NetworkManager.IsListening)
                UpdateDoughCookRPC();
        }

        public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
        {
            base.HandleInteract(inputCtx, character);

            PlayerPickPlaceInteract(character);
        }

        [Rpc(SendTo.Server)]
        private void UpdateDoughCookRPC()
        {
            if (ItemHolder.HoldingNetObj == default)
                return;

            if (!ItemHolder.HoldingNetObj.TryGetComponent(out IngredientController holdingIngredient))
                return;

            var holdingData = holdingIngredient.Data;
            var holdingProcess = holdingData.ProcessData;
            if (holdingData.Type != Type.Dough || holdingProcess.ProcessWith != Processor.Oven)
                return;

            Debug.Log("Processing");
            holdingIngredient.Model.DoProcess(Processor.Oven);
            ProcessPunchRpc();
        }
    }
}