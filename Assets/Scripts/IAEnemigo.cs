using System.Collections;
using UnityEngine;

public class IAEnemigo : MonoBehaviour
{
    public enum EstadoEnemigo { Patrullando, Persiguiendo, Atacando, Inactivo }

    [Header("Configuración General")]
    public float velocidad = 2f;
    public bool puedePatrullar = false;
    public bool puedeAtacarEnMovimiento = true;

    [Header("Configuración de Patrulla")]
    public Transform[] puntosPatrulla;
    public float tiempoEsperaEnPunto = 2f;

    [Header("Configuración de Persecución")]
    public Transform objetivo;
    public float rangoPersecucion = 10f;
    public float rangoAtaque = 2f;

    [Header("Configuración de Ataque")]
    public float tiempoEntreAtaques = 1.5f;
    public int daño = 10;

    private int indicePuntoActual = 0;
    private EstadoEnemigo estadoActual = EstadoEnemigo.Inactivo;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 direccionActual = Vector3.zero;
    private Coroutine rutinaActual;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Buscar automáticamente al jugador si no está asignado
        if (objetivo == null)
        {
            GameObject jugador = GameObject.FindWithTag("Player");
            if (jugador != null)
            {
                objetivo = jugador.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró un jugador con el tag 'Player'.");
            }
        }

        // Forzar estado inicial
        if (puedePatrullar && puntosPatrulla.Length > 0)
        {
            CambiarEstado(EstadoEnemigo.Patrullando);
        }
        else
        {
            CambiarEstado(EstadoEnemigo.Inactivo);
        }
    }

    private void Update()
    {
        if (estadoActual == EstadoEnemigo.Inactivo) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, objetivo.position);

        if (distanciaAlJugador <= rangoAtaque)
        {
            CambiarEstado(EstadoEnemigo.Atacando);
        }
        else if (distanciaAlJugador <= rangoPersecucion)
        {
            CambiarEstado(EstadoEnemigo.Persiguiendo);
        }
        else if (estadoActual != EstadoEnemigo.Patrullando && puedePatrullar)
        {
            CambiarEstado(EstadoEnemigo.Patrullando);
        }

        ActualizarAnimator();
    }

    private void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;

        estadoActual = nuevoEstado;

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
        }

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                Debug.Log("Enemigo inicia patrullaje.");
                rutinaActual = StartCoroutine(Patrullar());
                break;
            case EstadoEnemigo.Persiguiendo:
                Debug.Log("Enemigo inicia persecución.");
                rutinaActual = StartCoroutine(PerseguirJugador());
                break;
            case EstadoEnemigo.Atacando:
                Debug.Log("Enemigo ataca.");
                rutinaActual = StartCoroutine(Atacar());
                break;
            case EstadoEnemigo.Inactivo:
                Debug.Log("Enemigo está inactivo.");
                break;
        }
    }

    private IEnumerator Patrullar()
    {
        while (estadoActual == EstadoEnemigo.Patrullando)
        {
            if (puntosPatrulla.Length == 0)
            {
                Debug.LogWarning("No hay puntos de patrulla configurados.");
                yield break;
            }

            Vector3 puntoDestino = puntosPatrulla[indicePuntoActual].position;

            while (Vector3.Distance(transform.position, puntoDestino) > 0.1f)
            {
                direccionActual = (puntoDestino - transform.position).normalized;
                Mover(direccionActual);
                yield return null;
            }

            Debug.Log($"Enemigo llegó al punto {indicePuntoActual}.");
            yield return new WaitForSeconds(tiempoEsperaEnPunto);

            indicePuntoActual = (indicePuntoActual + 1) % puntosPatrulla.Length;
        }
    }

    private IEnumerator PerseguirJugador()
    {
        while (estadoActual == EstadoEnemigo.Persiguiendo)
        {
            if (objetivo == null) yield break;

            direccionActual = (objetivo.position - transform.position).normalized;
            Mover(direccionActual);
            yield return null;
        }
    }

    private IEnumerator Atacar()
    {
        while (estadoActual == EstadoEnemigo.Atacando)
        {
            direccionActual = Vector3.zero; // Detener el movimiento durante el ataque
            animator.SetTrigger("Attack");

            Debug.Log($"Enemigo ataca al jugador causando {daño} de daño.");

            yield return new WaitForSeconds(tiempoEntreAtaques);

            if (Vector3.Distance(transform.position, objetivo.position) <= rangoPersecucion)
            {
                CambiarEstado(EstadoEnemigo.Persiguiendo);
            }
            else if (puedePatrullar)
            {
                CambiarEstado(EstadoEnemigo.Patrullando);
            }
        }
    }

    private void Mover(Vector3 direccion)
    {
        if (direccion == Vector3.zero) return;

        rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);
    }

    private void ActualizarAnimator()
    {
        animator.SetFloat("MoveX", direccionActual.x);
        animator.SetFloat("MoveZ", direccionActual.z);
        animator.SetFloat("Speed", direccionActual.magnitude);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoPersecucion);

        if (puntosPatrulla != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform punto in puntosPatrulla)
            {
                Gizmos.DrawSphere(punto.position, 0.2f);
            }
        }
    }
}
