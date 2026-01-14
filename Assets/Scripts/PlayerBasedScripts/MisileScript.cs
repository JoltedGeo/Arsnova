using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using Unity.Mathematics;

public class MisileScript : MonoBehaviour
{
    public AudioSource magicMisileHit;
    [SerializeField] private ParticleSystem _particleSystem;
    private ParticleSystem particleSysteminstance;

    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    private float timeAlive;
    public float maxLifeSpan = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        magicMisileHit = GetComponent<AudioSource>();
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
            Debug.Log("Projectile Hit");

            FindObjectOfType<AudioManager>().Play("MMExplosion");
            SpawnImpactParticles();

            Destroy(gameObject);
        }
    }

    private void SpawnImpactParticles()
    {
        particleSysteminstance = Instantiate(_particleSystem, transform.position, Quaternion.identity);
    }
}
