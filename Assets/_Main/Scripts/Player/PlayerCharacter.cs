using Main.Items.ItemHolder;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    public NetworkItemHolder ItemHolder;

    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float dashMultiplier = 5;

    [SerializeField]
    private float rotateSpeed = 5;

    [SerializeField]
    private Rigidbody rb;

    private Vector3 moveDir;

    private void OnValidate()
    {
        rb ??= GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ItemHolder.InitUpdateParentTransform();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    [Rpc(SendTo.Server)]
    public void MoveRPC(Vector2 moveInput)
    {
        moveDir = new Vector3(moveInput.x, 0, moveInput.y);
        rb.AddForce(moveDir * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    private void UpdateRotation()
    {
        if (moveDir.sqrMagnitude < 0.001f)
            return;

        // rot dir
        var rotDir = Quaternion.LookRotation(moveDir);

        // Interpolate
        float currentYaw = transform.eulerAngles.y;
        float targetYaw = rotDir.eulerAngles.y;
        float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.fixedDeltaTime * rotateSpeed);

        transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
    }

    [Rpc(SendTo.Server)]
    public void DashRPC(Vector2 moveInput)
    {
        var dir = new Vector3(moveInput.x, 0, moveInput.y);

        if (dir == Vector3.zero)
            dir = transform.forward;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * moveSpeed * dashMultiplier, ForceMode.Impulse);
    }
}
