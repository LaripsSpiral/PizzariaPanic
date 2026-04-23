using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;

    [SerializeField]
    private LayerMask interactionMask;

    [SerializeField]
    private float range;

    [SerializeField]
    private PlayerCharacter character;
    public PlayerCharacter Character => character;

    private IInteractable currInteractable;

    private Collider[] allocateInteractions = new Collider[10];

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

        float nearestSqrDist = range * range;

        int count = Physics.OverlapSphereNonAlloc(pivot.position, range, allocateInteractions, interactionMask);

        for (int i = 0; i < count; i++)
        {
            if (!allocateInteractions[i].TryGetComponent(out IInteractable interactable))
                continue;

            var diff = interactable.Transform.position - pivot.position;
            var distSqr = diff.sqrMagnitude;

            if (distSqr < nearestSqrDist)
            {
                target = interactable;
                nearestSqrDist = distSqr;
            }
        }
        return target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.position, range);
    }
}
