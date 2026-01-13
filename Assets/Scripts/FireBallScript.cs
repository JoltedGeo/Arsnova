using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public AudioSource fireBallExplosion;
    [SerializeField] private ParticleSystem _particleSystem;
    private ParticleSystem particleSysteminstance;

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    private float timeAlive;
    public float maxLifeSpan = 5;

    public float explosionRadius = 2f; 
    public int fireBallDamage = 50;

    // Start is called before the first frame update
    void Start()
    {
        fireBallExplosion = GetComponent<AudioSource>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90); // The 90 degrees is to turn the misile to be vertical, if not its horizontal
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive >= maxLifeSpan)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("FlyingEnemy"))
        {
            Debug.Log("Projectile Hit. Creating AoE Explosion");

            FindObjectOfType<AudioManager>().Play("FireBallExplosion");
            SpawnImpactParticles();

            GameObject explosionArea = new GameObject("FireballExplosionArea");
            explosionArea.transform.position = transform.position;

            ExplosionTrigger explosionTrigger = explosionArea.AddComponent<ExplosionTrigger>();

            explosionTrigger.SetupExplosion(fireBallDamage, explosionRadius);

            Destroy(gameObject);
        }
    }

    private void SpawnImpactParticles()
    {
        particleSysteminstance = Instantiate(_particleSystem, transform.position, Quaternion.identity);
    }

}
