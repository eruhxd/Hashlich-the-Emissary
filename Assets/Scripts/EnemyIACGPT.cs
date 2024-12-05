using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;  // Referencia al jugador
    public float detectionRange = 10f;  // Rango de detección para seguir al jugador

    private NavMeshAgent agent;  // Componente NavMeshAgent para mover al enemigo

    void Start()
    {
        // Obtiene el componente NavMeshAgent del enemigo
        agent = GetComponent<NavMeshAgent>();

        // Asegúrate de que el jugador está asignado en el Inspector
        if (player == null)
        {
            Debug.LogError("El jugador no está asignado en el inspector");
        }
    }

    void Update()
    {
        // Verifica si el jugador está dentro del rango de detección
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Haz que el enemigo se mueva hacia la posición del jugador
            agent.SetDestination(player.position);
        }
        else
        {
            // Si el jugador está fuera del rango de detección, detén al enemigo
            agent.ResetPath();
        }
    }
}