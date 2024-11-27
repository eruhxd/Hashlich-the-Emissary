using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("General")]
    public float gravityScale = -20f;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Rotation")]
    public float rotationSensibility = 10f;

    [Header("Jump")]
    public float jumpHeight = 1.9f;

    private float cameraVerticalAngle;
    private Vector3 moveInput = Vector3.zero;
    private Vector3 rotationInput = Vector3.zero;
    private CharacterController characterController;

    // Variables para doble salto
    private bool isGrounded = false;
    private bool canDoubleJump = false;

    // Referencia a la plataforma móvil
    private Transform platformParent;

    public Transform playerGround;
    public float detectionDistance = 1.2f;
    public LayerMask lm;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.minMoveDistance = 0;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void Move()
    {
        
        isGrounded = characterController.isGrounded;

       

        if (isGrounded)
        {
            // Reset doble salto al estar en el suelo
            canDoubleJump = false;

            // Movimiento horizontal basado en la entrada del jugador
            moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            moveInput = Vector3.ClampMagnitude(moveInput, 1f);

            // Movimiento en la dirección de la cámara
            moveInput = transform.TransformDirection(moveInput);

            // Aplicar velocidad de caminata o carrera
            if (Input.GetButton("Sprint"))
            {
                moveInput *= runSpeed;
            }
            else
            {
                moveInput *= walkSpeed;
            }

            // Primer salto
            if (Input.GetButtonDown("Jump"))
            {
                moveInput.y = Mathf.Sqrt(jumpHeight * -2f * gravityScale);
            }
        }
        else
        {
            // Movimiento aéreo (control limitado)
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Movimiento horizontal basado en la dirección de la cámara
            Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction = transform.TransformDirection(direction); // Aplicamos la dirección de la cámara

            // Aplica control de aire
            moveInput.x = Mathf.Lerp(moveInput.x, direction.x * walkSpeed, 0.5f);
            moveInput.z = Mathf.Lerp(moveInput.z, direction.z * walkSpeed, 0.5f);

            // Doble salto en el aire
            if (Input.GetButtonDown("Jump") && !canDoubleJump)
            {
                transform.SetParent(null);
                moveInput.y = Mathf.Sqrt(jumpHeight * -2f * gravityScale);
                canDoubleJump = true; // Activamos el doble salto
            }
        }

        // Aplicamos la gravedad
        moveInput.y += gravityScale * Time.deltaTime;

        // Movimiento del CharacterController
        characterController.Move(moveInput * Time.deltaTime);

        
    }

    private void FixedUpdate()
    {

        RaycastHit rci;
        bool hit = Physics.Raycast(playerGround.position, Vector3.down, out rci, detectionDistance, lm);

        // Si estamos sobre una plataforma, movemos el jugador con la plataforma
        if (rci.collider != null)
        {
            transform.SetParent(rci.collider.transform); // Hacemos que el jugador sea hijo de la plataforma
        }
        else
        {
            transform.SetParent(null); // Si no estamos en una plataforma, quitamos al jugador como hijo
        }
    }

    private void Look()
    {
        rotationInput.x = Input.GetAxis("Mouse X") * rotationSensibility * Time.deltaTime;
        rotationInput.y = Input.GetAxis("Mouse Y") * rotationSensibility * Time.deltaTime;

        cameraVerticalAngle = cameraVerticalAngle + rotationInput.y;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -70, 70);

        transform.Rotate(Vector3.up * rotationInput.x);
        playerCamera.transform.localRotation = Quaternion.Euler(-cameraVerticalAngle, 0f, 0f);
    }


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("C enter " + collision.gameObject.name);
    }

}