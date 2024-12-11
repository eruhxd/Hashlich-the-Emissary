using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Variables para la vida y muerte
    private int health = 3; // El jugador puede recibir hasta 3 golpes
    private bool isDead = false; // Estado de muerte del jugador

    // Posición inicial del jugador para el respawn
    private Vector3 initialPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterController.minMoveDistance = 0;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Guardamos la posición inicial del jugador para el respawn
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (isDead) return; // Si el jugador está muerto, no puede moverse ni hacer nada

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

    // Esta función se activa cuando el jugador está tocando a un enemigo
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Reducir la vida del jugador
            health--;

            // Mostrar mensaje de vida restante (opcional)
            Debug.Log("Player Health: " + health);

            // Si el jugador ha recibido 3 golpes, muere
            if (health <= 0)
            {
                Die();
            }
        }
    }

    // Método para manejar la muerte del jugador
    private void Die()
    {
        isDead = true; // El jugador está muerto
        Debug.Log("Player died!");

        // Aquí puedes agregar la lógica para la muerte, como reproducir una animación
        // o desactivar al jugador
        // Por ejemplo, desactivar al jugador:
        // gameObject.SetActive(false); // Desactiva al jugador
        // Time.timeScale = 0;

        // Aquí también podrías añadir una animación de muerte o pantalla de Game Over
        // Iniciar el respawn
        Invoke("Respawn",1);
    }

    // Corutina que maneja el respawn
    private void Respawn()
    {
        // Esperar unos segundos antes de respawnear al jugador (puedes ajustar este tiempo)
        // yield return new WaitForSeconds(3f);

        //Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Reposicionar al jugador en la posición inicial
        transform.position = initialPosition;

        // Restaurar salud
        health = 3;

        // Reactivar al jugador
        gameObject.SetActive(true);

        // Opcionalmente, reiniciar otras propiedades del jugador como su animación, si es necesario
        isDead = false; // El jugador ya está vivo nuevamente
        Debug.Log("Player respawned!");
    }
}
