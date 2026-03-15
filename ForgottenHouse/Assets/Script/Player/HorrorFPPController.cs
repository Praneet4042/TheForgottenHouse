using UnityEngine;

public class HorrorFPPController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -20f;

    [Header("Mouse")]
    public float mouseSensitivity = 100f;
    public Transform camHolder;

    [Header("Sprint Settings")]
    public float maxSprintTime = 5f;
    public float sprintCooldown = 12f;

    // private
    CharacterController cc;
    float xRot = 0f;
    Vector3 velocity;

    private float _sprintTimeRemaining;
    private float _sprintCooldownRemaining;
    private bool _isSprinting = false;
    private bool _onCooldown = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _sprintTimeRemaining = maxSprintTime;
        _sprintCooldownRemaining = 0f;
    }

    void Update()
    {
        MouseLook();
        Move();
    }

    void MouseLook()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRot -= my;
        xRot = Mathf.Clamp(xRot, -70f, 70f);
        camHolder.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mx);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        HandleSprint(wantsToSprint);

        float speed = _isSprinting ? runSpeed : walkSpeed;
        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(move * speed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    void HandleSprint(bool wantsToSprint)
    {
        if (_onCooldown)
        {
            _isSprinting = false;
            _sprintCooldownRemaining -= Time.deltaTime;

            if (_sprintCooldownRemaining <= 0f)
            {
                _onCooldown = false;
                _sprintTimeRemaining = maxSprintTime;
                Debug.Log("Sprint ready!");
            }
            return;
        }

        if (wantsToSprint && _sprintTimeRemaining > 0f)
        {
            _isSprinting = true;
            _sprintTimeRemaining -= Time.deltaTime;

            if (_sprintTimeRemaining <= 0f)
            {
                _isSprinting = false;
                _onCooldown = true;
                _sprintCooldownRemaining = sprintCooldown;
                Debug.Log("Sprint exhausted! Cooling down...");
            }
        }
        else
        {
            _isSprinting = false;
            if (_sprintTimeRemaining < maxSprintTime)
                _sprintTimeRemaining += Time.deltaTime * 0.5f;
        }
    }
}