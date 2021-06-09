using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour {
    /// <summary>
    /// Can the player move.
    /// </summary>
    [SerializeField] private bool canMove = true;
    
    /// <summary>
    /// Is the player moving?
    /// </summary>
    public bool isMoving { get; set; }

    // Is the player touching a ground surface.
    [Header("Player Variables")]
    [SerializeField] private bool isGrounded;

    // How fast can the player move?
    [SerializeField] [Range(0f, 20f)] private float playerSpeed = 5f;

    // How long should the walking sound interval between steps.
    [SerializeField] [Range(0f, 2f)] private float walkingSpeedSound = 0.75f;

    // Time for when to play the next footstep.
    private float nextTimeToPlayFootstep = 0f;

    [Header("Camera Variables")]   
    // Camera vertical rotation bounds.
    [SerializeField] private float[] verticalCameraRotationLimit = { -110f, 60f };

    // Camera rotation sensitivity.
    [SerializeField] private float cameraXRotationSensitivity = 3f;
    [SerializeField] private float cameraYRotationSensitivity = 3f;

    // Mouse Axis inversion
    [SerializeField] private bool invertMouseX;
    [SerializeField] private bool invertMouseY;

    // Vector to hold camera rotation values.
    private Vector3 cameraRotation;

    [Header("Ground Detection Variables")]

    // How far under the player should we check for the ground.
    [SerializeField] private Vector3 groundDistance = new Vector3(0f, -2f, 0f);
    [SerializeField] private Vector3 playerOffset = new Vector3(0f, -1f, 0f);

    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform bulletSpawn;
    [SerializeField, Range(1, 350)] private int rateOfFire = 30;
    private float nextTimeToFire;
    
    // reference for the player camera.
    private GameObject playerCamera;
    
    // Gun animator.
    private Animator gunAnim;
    private VisualEffect muzzleFlash;
    [SerializeField] private AudioClip gunshotClip;
    private AudioSource aSource;

    // Player components.
    // Player Rigidbody
    private Rigidbody rb;
    private static readonly int GunFireAnim = Animator.StringToHash("Fire");

    // Gets needed references.
    private void Start() {
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>().transform.gameObject;
        rb = GetComponent<Rigidbody>();
        gunAnim = GetComponentInChildren<Animator>();
        muzzleFlash = GetComponentInChildren<VisualEffect>();
        aSource = GetComponentInChildren<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.L)) {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked ?
                CursorLockMode.None : CursorLockMode.Locked);
        }

        if(!canMove) return;
        Rotation();
        Pistol();
    }

    // Used to call the footstep sounds if the player is moving.
    private void FixedUpdate() {
        if(!canMove) return;
        CheckGround();
        Movement();
        if(!isMoving) return;
        if(!(Time.time > nextTimeToPlayFootstep)) return;
        nextTimeToPlayFootstep = Time.time + walkingSpeedSound;
        if(!isGrounded) return;
        // PlayFootstep();
    }
    

    /// <summary>
    /// Moves player.
    /// </summary>
    private void Movement() {
        var xAxis = Input.GetAxisRaw("Horizontal");
        var zAxis = Input.GetAxisRaw("Vertical");

        var horizontal = transform.right * xAxis;
        var vertical = transform.forward * zAxis;
        var movement = (horizontal + vertical).normalized * playerSpeed;

        if(movement != Vector3.zero) {
            rb.MovePosition(rb.position + movement * Time.deltaTime);
            isMoving = true;
        } else {
            isMoving = false;
        }
    }

    /// <summary>
    /// Rotates the player camera.
    /// </summary>
    private void Rotation() {
        var yAxis = 0f;
        var xAxis = 0f;

        if(invertMouseX) {
            xAxis = Input.GetAxis($"Mouse X") * -1f;
        } else {
            xAxis = Input.GetAxis($"Mouse X");
        }

        if(invertMouseY) {
            yAxis = Input.GetAxisRaw($"Mouse Y");
        } else {
            yAxis = Input.GetAxisRaw($"Mouse Y") * -1f;
        }        

        var rotation = new Vector3(0f, xAxis, 0f) * cameraXRotationSensitivity;
        cameraRotation += new Vector3(yAxis, 0f, 0f) * cameraYRotationSensitivity;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, verticalCameraRotationLimit[0], verticalCameraRotationLimit[1]);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        if(playerCamera == null) return;
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation.x, transform.rotation.y, transform.rotation.z);
    }

    /// <summary>
    /// Checks if the player is on air.
    /// </summary>
    private void CheckGround() {
        var offsetPosition = (transform.position - playerOffset);
        Physics.Linecast(offsetPosition, offsetPosition - groundDistance, out var hitInfo);

        if(hitInfo.collider == null) return;
        isGrounded = true;
    }

    /// <summary>
    /// Controls the pistol.
    /// </summary>
    private void Pistol() {
        if(Time.time < nextTimeToFire) return;
        if(!Input.GetKey(KeyCode.Mouse0)) return;
        nextTimeToFire = Time.time + 60f / rateOfFire;
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        gunAnim.SetTrigger(GunFireAnim);
        muzzleFlash.Play();
        aSource.PlayOneShot(gunshotClip);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        var offsetPosition = (transform.position - playerOffset);
        Gizmos.DrawLine(offsetPosition, offsetPosition - groundDistance);
    }
    #endif
}
