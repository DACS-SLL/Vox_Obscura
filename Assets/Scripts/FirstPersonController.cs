using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 6f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;

    [Header("Cámara")]
    public Transform cameraRoot;
    public float sensitivity = 150f;
    public float verticalLookLimit = 85f;

    [Header("Headbob")]
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 18f;
    public float runBobAmount = 0.1f;
    public float crouchBobSpeed = 8f;
    public float crouchBobAmount = 0.03f;

    private float defaultCameraY;
    private float bobTimer = 0f;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 1.8f;
    public float crouchTransitionSpeed = 8f;

    private CharacterController controller;
    private float xRotation = 0f;
    private float velocityY = 0f;

    private bool isCrouching = false;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip woodStep;
    public AudioClip concreteStep;
    public AudioClip grassStep;

    private float footstepTimer = 0f;
    private float stepInterval = 0.5f;


    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraY = cameraRoot.localPosition.y;
    }

    void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleCrouch();
        HandleHeadBob(controller.velocity.magnitude);
        HandleFootsteps(controller.velocity.magnitude);

    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
            speed = runSpeed;

        if (isCrouching)
            speed = crouchSpeed;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveDir = transform.TransformDirection(input);

        if (controller.isGrounded)
            velocityY = -1f;
        else
            velocityY += gravity * Time.deltaTime;

        Vector3 movement = moveDir * speed + Vector3.up * velocityY;

        controller.Move(movement * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : standHeight;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        cameraRoot.localPosition = new Vector3(
            0,
            controller.height - 0.2f,
            0
        );
    }

    void HandleHeadBob(float currentSpeed)
    {
        if (!controller.isGrounded || controller.velocity.magnitude < 0.1f)
        {
            bobTimer = 0f;
            cameraRoot.localPosition = new Vector3(0, defaultCameraY, 0);
            return;
        }

        float bobSpeed = walkBobSpeed;
        float bobAmount = walkBobAmount;

        if (isCrouching)
        {
            bobSpeed = crouchBobSpeed;
            bobAmount = crouchBobAmount;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            bobSpeed = runBobSpeed;
            bobAmount = runBobAmount;
        }

        bobTimer += Time.deltaTime * bobSpeed;

        float yOffset = Mathf.Sin(bobTimer) * bobAmount;

        cameraRoot.localPosition = new Vector3(
            0,
            defaultCameraY + yOffset,
            0
        );
    }

    void HandleFootsteps(float currentSpeed)
    {
        if (!controller.isGrounded || controller.velocity.magnitude < 0.2f)
            return;

        if (isCrouching) stepInterval = 0.55f;
        else if (Input.GetKey(KeyCode.LeftShift)) stepInterval = 0.3f;
        else stepInterval = 0.45f;

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= stepInterval)
        {
            footstepTimer = 0f;

            AudioClip clip = DetectSurface();

            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            footstepSource.PlayOneShot(clip);
        }

    }

    AudioClip DetectSurface()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            string tag = hit.collider.tag;

            switch (tag)
            {
                case "Wood": return woodStep;
                case "Grass": return grassStep;
                default: return concreteStep;
            }
        }

        return concreteStep;
    }

}
