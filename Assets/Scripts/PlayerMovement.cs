using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public ParticleSystem rpsPrefab;
    private ParticleSystem rpsInstance;
    public ParticleSystem lpsPrefab;
    private ParticleSystem lpsInstance;
    public float moveSpeed = 5f;
    public float baseMoveSpeed = 5f;
    public float focusMoveSpeed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded;
    private Rigidbody2D rb;
    private float jumpTime;
    public float maxJumpTime = 1f;
    public bool isJumping;
    public float bouncePadMod = 4;

    // Falling speed increse varriables
    [Header("Fast Fall")]
    public float defaultGravity = 1f;
    public float maxGravity = 2.5f;
    public float gravityIncreaseSpeed = 5f;
    public float gravityResetSpeed = 5f;
    public float fastFallThreshold = .5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = defaultGravity;
        rpsInstance = Instantiate(rpsPrefab, transform);
        rpsInstance.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        lpsInstance = Instantiate(lpsPrefab, transform);
        lpsInstance.transform.localPosition = new Vector3(0f, -0.5f, 0f);
    }

    void Update()
    {
        HandleRunningAudio();

        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        //Runnin particles handling
        if (rb.velocity.x > 0 && isGrounded)
        {
            if (!rpsInstance.isPlaying)
            {
                Debug.Log("Moving Right And Play rps");
                rpsInstance.Play();
                FindObjectOfType<AudioManager>().Play("PlayerRunning");
            }
        }
        else
        {
            if (rpsInstance.isPlaying)
            {
                rpsInstance.Stop();
                FindObjectOfType<AudioManager>().Stop("PlayerRunning");
            }
        }

        if (rb.velocity.x < 0 && isGrounded)
        {
            if (!lpsInstance.isPlaying)
            {
                Debug.Log("Moving Left And Play lps");
                lpsInstance.Play();
                FindObjectOfType<AudioManager>().Play("PlayerRunning");
            }
        }
        else
        {
            if (lpsInstance.isPlaying)
            {
                lpsInstance.Stop();
                FindObjectOfType<AudioManager>().Stop("PlayerRunning");
            }
        }

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            jumpTime = maxJumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            FindObjectOfType<AudioManager>().Play("JumpAudio");
        }

        // Varriable Jump
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTime > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Continue applying upward force
                jumpTime -= Time.deltaTime; //This will count down to 0 because jumpTime is set to maxJumpTime
            }
            else
            {
                isJumping = false; // Stop applying force if jump duration is exceeded
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

    }


    void FixedUpdate()
    {
        HandleFastFall();
    }

    void HandleFastFall()
    {
        bool isFalling = rb.velocity.y < fastFallThreshold;
        bool holdingDown = Input.GetKey(KeyCode.S);

        if (!isGrounded && isFalling && holdingDown)
        {
            //Increase Gravity
            rb.gravityScale = Mathf.MoveTowards(rb.gravityScale, maxGravity, gravityIncreaseSpeed * Time.fixedDeltaTime);
        }
        else
        {
            //Smootly return grav to normal
            rb.gravityScale = Mathf.MoveTowards(rb.gravityScale, defaultGravity, gravityResetSpeed * Time.fixedDeltaTime);
        }
    }


    void HandleRunningAudio()
    {
        bool isMovingRight = rb.velocity.x > 0.01f && isGrounded;
        bool isMovingLeft = rb.velocity.x < -0.01f && isGrounded;
        bool isMoving = isMovingRight || isMovingLeft;

        // Handle right particle
        if (isMovingRight)
        {
            if (!rpsInstance.isPlaying) rpsInstance.Play();
        }
        else
        {
            if (rpsInstance.isPlaying) rpsInstance.Stop();
        }

        // Handle left particle
        if (isMovingLeft)
        {
            if (!lpsInstance.isPlaying) lpsInstance.Play();
        }
        else
        {
            if (lpsInstance.isPlaying) lpsInstance.Stop();
        }

        // Handle running audio (only one AudioSource)
        if (isMoving)
        {
            if (!FindObjectOfType<AudioManager>().IsPlaying("PlayerRunning"))
                FindObjectOfType<AudioManager>().Play("PlayerRunning");
        }
        else
        {
            FindObjectOfType<AudioManager>().Stop("PlayerRunning");
        }
    }

    //Regular Jumping
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player is no longer on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    //Bounce Pad Logic
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("BouncePad"))
        {
            jumpForce += bouncePadMod;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("BouncePad"))
        {
            jumpForce -= bouncePadMod;
        }
    }
}