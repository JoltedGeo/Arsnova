using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private ParticleSystem particleSysteminstance;
    public int maxHealth = 100;
    public int currentHealth;
    private Rigidbody2D rb;
    private bool isDead;
    public int healthRechargeRate = 10;
    public float healthDelay = 2f;
    private float healthDelayTimer;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 0 && !isDead)
        {
            DestroyPlayer();
        }

        // Health Regeneration
        // Mana regeneration
        healthDelayTimer += Time.deltaTime;
        if(currentHealth < maxHealth && healthDelayTimer > healthDelay)
        {
            healthDelayTimer = 0;
            currentHealth += healthRechargeRate;

            healthBar.SetHealth(currentHealth);

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }
    
    void DestroyPlayer()
    {
        isDead = true;
        FindObjectOfType<AudioManager>().Play("FlyingEnemyDeath");
        particleSysteminstance = Instantiate(_particleSystem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        rb.AddForce(transform.up * 1f, ForceMode2D.Impulse);

        healthBar.SetHealth(currentHealth);
    }
}
