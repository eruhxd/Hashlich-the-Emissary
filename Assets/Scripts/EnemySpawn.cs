using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    public GameObject[] smallEnemies; // Lista de enemigos peque�os a generar
    public float spawnInterval = 5f; // Intervalo de tiempo entre generaci�n de enemigos
    public Transform spawnPoint; // Punto donde se generar�n los enemigos (puede ser un objeto vac�o)

    private void Start()
    {
        // Comienza a generar enemigos en intervalos regulares
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine para generar enemigos cada cierto tiempo
    private IEnumerator SpawnEnemies()
    {
        while (true) // Mientras el juego est� corriendo, el spawner continuar� generando enemigos
        {
            // Espera el tiempo definido en el Inspector
            yield return new WaitForSeconds(spawnInterval);

            // Verifica si hay enemigos peque�os asignados
            if (smallEnemies.Length > 0 && spawnPoint != null)
            {
                // Selecciona un enemigo aleatorio de la lista
                int randomIndex = Random.Range(0, smallEnemies.Length);

                // Instancia el enemigo seleccionado en el punto de spawn
                Instantiate(smallEnemies[randomIndex], spawnPoint.position, Quaternion.identity);

                // Opcionalmente, puedes hacer algo despu�s de instanciar (como loguear el evento)
                Debug.Log("Enemy spawned: " + smallEnemies[randomIndex].name);
            }
            else
            {
                Debug.LogWarning("No small enemies assigned or spawn point is missing!");
            }
        }
    }
}