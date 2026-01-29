using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float dashMultiplier = 5;

    [SerializeField]
    private float rotateSpeed = 5;

    [SerializeField]
    private Rigidbody rb;

    private void OnValidate()
    {
        rb ??= GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    [Rpc(SendTo.Server)]
    public void MoveRPC(Vector2 moveInput)
    {
        var moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        rb.AddForce(moveDir * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    [Rpc(SendTo.Server)]
    public void DashRPC(Vector2 moveInput)
    {
        var dir = new Vector3(moveInput.x, 0, moveInput.y);

        if (dir == Vector3.zero)
            dir = transform.forward;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * dashMultiplier, ForceMode.Impulse);
    }

    private void UpdateRotation()
    {
        var moveDir = rb.linearVelocity.normalized/10;
        if (moveDir == Vector3.zero)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.LookRotation(moveDir), 
            Time.fixedDeltaTime * rotateSpeed);
    }

}
