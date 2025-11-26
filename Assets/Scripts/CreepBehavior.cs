using UnityEngine;

public class CreepBehavior : MonoBehaviour
{
    public float walkSpeed = 0.7f;
    public float detectionDistance = 1f;
    public float turnAngle = 180f;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.isKinematic = false;

        animator.Play("Creep|Crouch_Action");
    }

    void StartWalking()
    {
        animator.Play("Creep|Crouch_Action");
    }

    void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Creep|Crouch_Action"))
        {
            PatrolMovement();
        }
    }

    void PatrolMovement()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.7f, transform.forward, detectionDistance))
        {
            Debug.Log("Pared detectada");
            TurnAround();
            return;
        }

        Vector3 newPos = rb.position + transform.forward * walkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    void TurnAround()
    {
        transform.Rotate(0f, turnAngle, 0f);
    }
}
