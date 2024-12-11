using UnityEngine;

public class Enemy: MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 100; // Vida del enemigo
    public int damageAmount = 10; // Cantidad de daño por disparo
    public GameObject deathEffect; // Efecto visual de muerte (opcional)

    private Animator animator;

    private void Start()
    {
        // Si el enemigo tiene un Animator, lo conseguimos para animaciones de muerte, si lo deseas
        animator = GetComponent<Animator>();
    }

    // Método que reduce la vida del enemigo cuando recibe daño
    public void TakeDamage(int damage)
    {
        // Reducir la salud del enemigo
        health -= damage;

        // Mostrar un mensaje en la consola (opcional)
        Debug.Log("Enemy Health: " + health);

        // Verificar si la salud ha llegado a cero o menos
        if (health <= 0)
        {
            Die();
        }
    }

    // Método que maneja la muerte del enemigo
    private void Die()
    {
        // Si el enemigo tiene un Animator y un trigger para muerte, activarlo (opcional)
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Instanciar un efecto de muerte, si existe
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Desactivar al enemigo (puedes agregar lógica adicional aquí)
        gameObject.SetActive(false); // O puedes destruirlo si lo prefieres: Destroy(gameObject);

        // Mostrar un mensaje opcional cuando muere el enemigo
        Debug.Log("Enemy died!");
    }
}