using Unity.Netcode;
using UnityEngine;

public abstract class BaseStation : NetworkBehaviour, IInteractable
{
    public Transform Transform => transform;

    protected virtual void Start()
    {
        InteractManager.Instance.Subscribe(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        InteractManager.Instance.Unsubscribe(this);
    }

    public virtual void HandleInteract(PlayerCharacter character)
    {
        Debug.Log($"{character} Interacted {this}");
    }

}
