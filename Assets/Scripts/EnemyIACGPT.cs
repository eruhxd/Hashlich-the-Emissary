using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;  // Referencia al jugador
    public float detectionRange = 10f;  // Rango de detecci�n para seguir al jugador

    private NavMeshAgent agent;  // Componente NavMeshAgent para mover al enemigo

    void Start()
    {
        // Obtiene el componente NavMeshAgent del enemigo
        agent = GetComponent<NavMeshAgent>();

        // Aseg�rate de que el jugador est� asignado en el Inspector
        if (player == null)
        {
            Debug.LogError("El jugador no est� asignado en el inspector");
        }
    }

    void Update()
    {
        // Verifica si el jugador est� dentro del rango de detecci�n
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Haz que el enemigo se mueva hacia la posici�n del jugador
            agent.SetDestination(player.position);
        }
        else
        {
            // Si el jugador est� fuera del rango de detecci�n, det�n al enemigo
            agent.ResetPath();
        }
    }
}