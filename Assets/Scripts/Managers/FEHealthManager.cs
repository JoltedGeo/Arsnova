using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class FEHealthManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private ParticleSystem particleSysteminstance;
    public AIDestinationSetter aIDestinationSetter;
    public PlayerHealthManager playerHealthManager;
    public CircleCollider2D circleCollider2D;
    public Animator animator;
    public int maxHealth = 60;
    public int currentHealth;
    public int magicMisileDamage = 20;
    public int fireBallDamage = 50;
    private int flyingEnemyDamage = 20;
    private float hitDelayTimer;
    public float hitDelay = 0.4f;
    private bool isDead;

    public HealthBar eHealthBar;
    public LevelingManager levelingManager;

    // Start is called before the first frame update
    void Start()
    {
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
        // Debugging to determine if the script refrence isn't in the inspector
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            Debug.Log("Found Player GameObject: " + playerObject.name);

            // Check if the component exists on the found object
            playerHealthManager = playerObject.GetComponent<PlayerHealthManager>();

            if (playerHealthManager == null)
            {
                // This is likely the exact issue if the tag is right
                Debug.LogError("PlayerHealthManager script is NOT on the object named: " + playerObject.name);
            }
            else
            {
                Debug.Log("Successfully found the PlayerHealthManager script!");
            }
        }
        else
        {
            // This confirms your tag check was incorrect or the object is missing
            Debug.LogError("No GameObject found with the tag 'Player' in the scene!");
        }


        circleCollider2D = GetComponent<CircleCollider2D>();
        playerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
        levelingManager = FindObjectOfType<LevelingManager>();
        currentHealth = maxHealth;
        eHealthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20);
        }

        // Check if dead
        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        levelingManager.FlyingEnemyXp();
        aIDestinationSetter.enabled = false;
        animator.SetBool("isDead", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        DestroyEnemy();
    }

    //Method for destroying the gameobject after the death animation
    void DestroyEnemy()
    {
        FindObjectOfType<AudioManager>().Play("FlyingEnemyDeath");
        particleSysteminstance = Instantiate(_particleSystem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        eHealthBar.SetHealth(currentHealth);
        FindObjectOfType<AudioManager>().Play("Enemy Hurt");
        animator.SetTrigger("gotHit");
    }

    void ResetHitAnim()
    {
        animator.ResetTrigger("gotHit");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Triggered");
        if (collision.gameObject.CompareTag("Magic Misile"))
        {
            Debug.Log("Got Hit by MagicMisile");
            TakeDamage(magicMisileDamage);
        }
        Debug.Log("Collision Triggered");
        if (collision.gameObject.CompareTag("FireBall"))
        {
            Debug.Log("Got Hit by fireBall");
            TakeDamage(fireBallDamage);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Initial Hit on Player");
            playerHealthManager.TakeDamage(flyingEnemyDamage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hitDelayTimer += Time.deltaTime;
            if (hitDelayTimer > hitDelay)
            {
                Debug.Log("Retriggered Box collider");
                circleCollider2D.enabled = false;
                circleCollider2D.enabled = true;

                hitDelayTimer = 0;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        circleCollider2D.enabled = false;
        circleCollider2D.enabled = true;
    }
}
