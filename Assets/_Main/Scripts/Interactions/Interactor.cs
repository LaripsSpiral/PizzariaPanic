using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private float range;

    [SerializeField]
    private PlayerCharacter character;
    public PlayerCharacter Character => character;

    private IInteractable currInteractable;

    [Rpc(SendTo.Server)]
    public void InteractRPC(InputAction.CallbackContext ctx) => Interact(ctx);
    public void Interact(InputAction.CallbackContext ctx)
    {
        currInteractable = FindNearest();
        if (currInteractable == null)
            return;

        //Debug.Log($"{this} Interacted with {interactable}.");
        currInteractable.HandleInteract(ctx, character);
    }

    [Rpc(SendTo.Server)]
    public void CancelInteractRPC(InputAction.CallbackContext ctx)
    {
        if (currInteractable == null)
            return;

        currInteractable.HandleCancelInteract(ctx, character);
        currInteractable = null;
    }

    private IInteractable FindNearest()
    {
        IInteractable target = null;
        var nearest = range;

        var interactManager = InteractManager.Instance;
        var interactables = interactManager.Interactables;

        if (interactables.Count == 0)
            return null;

        for (int i = 0; i < interactables.Count; i++)
        {
            // In Range
            var distance = Vector3.Distance(
                transform.position,
                interactables[i].Transform.position);

            if (distance < nearest)
            {
                target = interactables[i];
                nearest = distance;
            }
        }

        //Debug.Log($"{this} Found {target}");

        return target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
