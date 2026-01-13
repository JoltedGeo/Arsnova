using UnityEngine;
using System.Collections;

public class ExplosionTrigger : MonoBehaviour
{
    public ParticleSystem firballParticleSystem;
    private ParticleSystem firballInstance;
    private Transform explosioncenter;
    // The damage value passed from the FireBallScript
    private int explosionDamage; 
    // The duration the explosion collider is active
    public float explosionDuration = 0.1f;

    void Start()
    {
        firballParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetupExplosion(int damage, float radius)
    {
        explosionDamage = damage;

        // Ensure the collider exists and is configured
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        circleCollider.radius = radius;
        circleCollider.isTrigger = true;

        if (firballParticleSystem != null)
        {
            firballInstance.transform.localPosition = Vector3.zero;
            firballInstance = Instantiate(firballParticleSystem, explosioncenter);
            firballInstance.Play();
        }
        // Start the process to destroy the temporary explosion object shortly
        StartCoroutine(DeactivateAfterDuration());
    }

    // This function runs when an object enters the *new* explosion collider
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the explosion radius is a "FlyingEnemy"
        if (other.CompareTag("FlyingEnemy"))
        {
            Debug.Log($"Explosion hit {other.gameObject.name}!");

            // Try to get the FEHealthManager component from the enemy object
            FEHealthManager enemyHealth = other.GetComponent<FEHealthManager>();

            if (enemyHealth != null)
            {
                // Call the TakeDamage function on the enemy script
                enemyHealth.TakeDamage(explosionDamage);
            }
        }
    }

    IEnumerator DeactivateAfterDuration()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(explosionDuration);
        
        // Destroy the temporary explosion GameObject
        Destroy(gameObject);
    }
}