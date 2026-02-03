using UnityEngine.InputSystem;

public class Station_Oven : BaseStation
{
    public override void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter character)
    {
        base.HandleInteract(inputCtx, character);

        PlayerPickPlaceInteract(character);
    }
}
