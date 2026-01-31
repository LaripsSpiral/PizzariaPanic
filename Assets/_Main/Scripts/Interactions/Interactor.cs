using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter character;
    public PlayerCharacter Character => character;

    [SerializeField]
    private float range;

    [Rpc(SendTo.Server)]
    public void InteractRPC() => Interact(FindNearest());
    public void Interact(IInteractable interactable)
    {
        if (interactable == null)
            return;

        //Debug.Log($"{this} Interacted with {interactable}.");
        interactable.HandleInteract(character);
    }

    private IInteractable FindNearest()
    {
        IInteractable target = null;
        var nearest = float.MaxValue;

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
