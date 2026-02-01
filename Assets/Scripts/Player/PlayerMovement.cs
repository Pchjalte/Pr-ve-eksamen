using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun {
    
    public Transform playerCam;
    public Transform orientation;

    private Rigidbody rb;

    private InputSystem_Actions input;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool jumping;
    private bool crouching;

    private float xRotation;
    public float sensitivity = 50f;
    public float sensMultiplier = 1f;

    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    private Vector3 crouchScale = new Vector3(0.25f, 0.1875f, 0.25f);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    private Vector3 normalVector = Vector3.up;

    private void Awake() {

        rb = GetComponent<Rigidbody>();
        input = new InputSystem_Actions();

        if (!photonView.IsMine) {

            Destroy(playerCam.gameObject);
            Destroy(this);
            return;
        }
    }

    private void OnEnable() {

        if (!photonView.IsMine) return;

        input.Enable();

        input.Player.Jump.performed += _ => jumping = true;
        input.Player.Jump.canceled += _ => jumping = false;

        input.Player.Crouch.performed += _ => StartCrouch();
        input.Player.Crouch.canceled += _ => StopCrouch();
    }

    private void OnDisable() {

        if (!photonView.IsMine) return;

        input.Disable();
    }

    private void Start() {

        if (!photonView.IsMine) return;

        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {

        if (!photonView.IsMine) return;

        moveInput = input.Player.Move.ReadValue<Vector2>();
        lookInput = input.Player.Look.ReadValue<Vector2>();

        Look();
    }

    private void FixedUpdate() {

        if (!photonView.IsMine) return;

        Movement();
    }

    private void Movement() {

        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        CounterMovement(moveInput.x, moveInput.y, mag);

        if (readyToJump && jumping)
            Jump();

        if (crouching && grounded && readyToJump) {

            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        if (moveInput.x > 0 && xMag > maxSpeed) moveInput.x = 0;
        if (moveInput.x < 0 && xMag < -maxSpeed) moveInput.x = 0;
        if (moveInput.y > 0 && yMag > maxSpeed) moveInput.y = 0;
        if (moveInput.y < 0 && yMag < -maxSpeed) moveInput.y = 0;

        float multiplier = grounded ? 1f : 0.5f;
        float multiplierV = grounded && crouching ? 0f : multiplier;

        rb.AddForce(orientation.forward * moveInput.y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.right * moveInput.x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Look() {

        float mouseX = lookInput.x * sensitivity * Time.deltaTime * sensMultiplier;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime * sensMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.localRotation = Quaternion.Euler(xRotation, playerCam.localRotation.eulerAngles.y + mouseX, 0);
        orientation.localRotation = Quaternion.Euler(0, playerCam.localRotation.eulerAngles.y, 0);
    }

    private void StartCrouch() {

        crouching = true;

        float heightDiff = playerScale.y - crouchScale.y;
        transform.localScale = crouchScale;
        transform.position -= Vector3.up * (heightDiff * 0.5f);

        if (rb.linearVelocity.magnitude > 0.5f && grounded)
            rb.AddForce(orientation.forward * slideForce);
    }

    private void StopCrouch() {

        crouching = false;

        float heightDiff = playerScale.y - crouchScale.y;
        transform.localScale = playerScale;
        transform.position += Vector3.up * (heightDiff * 0.5f);
    }

    private void Jump() {

        if (!grounded) return;

        readyToJump = false;

        rb.AddForce(Vector2.up * jumpForce * 1.5f);
        rb.AddForce(normalVector * jumpForce * 0.5f);

        Vector3 vel = rb.linearVelocity;
        rb.linearVelocity = new Vector3(vel.x, Mathf.Max(0, vel.y), vel.z);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump() {

        readyToJump = true;
    }

    private void CounterMovement(float x, float y, Vector2 mag) {

        if (!grounded || jumping) return;

        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f)
            rb.AddForce(moveSpeed * orientation.right * Time.deltaTime * -mag.x * counterMovement);

        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f)
            rb.AddForce(moveSpeed * orientation.forward * Time.deltaTime * -mag.y * counterMovement);

        if (rb.linearVelocity.magnitude > maxSpeed) {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }
    }

    public Vector2 FindVelRelativeToLook() {

        float lookAngle = orientation.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;
        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float magnitude = rb.linearVelocity.magnitude;

        return new Vector2(
            magnitude * Mathf.Cos((90 - u) * Mathf.Deg2Rad),
            magnitude * Mathf.Cos(u * Mathf.Deg2Rad)
        );
    }

    private bool IsFloor(Vector3 v) {

        return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other) {

        if ((whatIsGround & (1 << other.gameObject.layer)) == 0) return;

        foreach (var contact in other.contacts) {

            if (IsFloor(contact.normal)) {

                grounded = true;
                normalVector = contact.normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        Invoke(nameof(StopGrounded), Time.deltaTime * 3f);
    }

    private void StopGrounded() {

        grounded = false;
    }
}