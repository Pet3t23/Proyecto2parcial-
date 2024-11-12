using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Aseg�rate de que la velocidad sea alta
    public float lifetime = 5f; // Aumenta este valor para permitir que las balas viajen m�s lejos

    void Start()
    {
        Destroy(gameObject, lifetime); // Destruye el proyectil despu�s de 'lifetime' segundos
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; // Movimiento continuo
    }
}
