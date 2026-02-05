using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public static InteractManager Instance;

    private List<IInteractable> interactables;
    public List<IInteractable> Interactables => interactables;

    private void Awake()
    {
        Instance = this;
        interactables = new List<IInteractable>();
    }
    public void Subscribe(IInteractable interactable)
    {
        interactables.Add(interactable);
        Debug.Log($"InteractManager Added {interactable}");
    }
    public void Unsubscribe(IInteractable interactable)
    {
        interactables.Remove(interactable);
        Debug.Log($"InteractManager Removed {interactable}");
    }
}
