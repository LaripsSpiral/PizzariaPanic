using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public Transform Transform { get; }
    public void HandleInteract(InputAction.CallbackContext inputCtx, PlayerCharacter interactor);
    public void HandleCancelInteract(InputAction.CallbackContext inputCtx, PlayerCharacter interactor);
}
