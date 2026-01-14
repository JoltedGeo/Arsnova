using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Casting : MonoBehaviour
{
    public ManaBar manaBar;
    public FireballUITimer fireballUITimer;
    public PlayerMovement playerMovement;

    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject magicMisile;
    public GameObject fireBall;
    public Transform misileTransform;
    public bool canCast;
    private float timer;
    public float timeBetweenCasting;
    public int fireBallDelay = 5;
    public float fireBallDelayTimer;
    public bool canCastFireBall;
    public int fireBallCost = 30;

    public UIManagerScript uIManagerScript;
    //Resources
    public int currentMana;
    public int maxMana = 100;
    public int manaRechargeRate = 10;
    public float manaDelay = 1f;
    private float manaDelayTimer;
    public int misileCost = 10;

    //Focusing for mana
    public int manaFocusRate;
    public float manaFocusTimer;
    public float maxManaFocusTime = 7;
    public bool isFocusing;
    public bool canFocus;
    public float manaFocusDelay;
    private Coroutine manaFocusCoroutine;
    public ParticleSystem manaFocusingParticles;
    private ParticleSystem manaFocusingInstance;
    public Transform focusPoint;

    

    // Start is called before the first frame update
    void Start()
    {
        manaFocusTimer = maxManaFocusTime;
        manaFocusingInstance = Instantiate(manaFocusingParticles, focusPoint);
        manaFocusingInstance.transform.localPosition = Vector3.zero;
        manaFocusingInstance.Stop();

        canFocus = true;
        isFocusing = false;
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>(); 
        fireballUITimer.SetMaxFireValue(fireBallDelay);

        if (fireballUITimer == null)
            fireballUITimer = FindObjectOfType<FireballUITimer>();
        
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();

        currentMana = maxMana;
        manaBar.SetMaxMana(maxMana); 
        fireBallDelayTimer = fireBallDelay; // Start as ready
        canCastFireBall = true;     
    }

    //Check if game is paused

    // Update is called once per frame
    void Update()
    {

        if (!uIManagerScript.gameIsPaused)
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;

            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        else
        {
            return;
        }

        // Mana regeneration
        if (!isFocusing)
        {
            manaDelayTimer += Time.deltaTime;
            if (currentMana < maxMana && manaDelayTimer > manaDelay)
            {
                manaDelayTimer = 0;
                currentMana += manaRechargeRate;

                manaBar.SetMana(currentMana);

                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
            }
        }

        // Casting Fireball
        if (!canCastFireBall)
        {
            fireBallDelayTimer += Time.deltaTime;

            if (fireBallDelayTimer >= fireBallDelay)
            {
                canCastFireBall = true;
                fireBallDelayTimer = fireBallDelay;
            }
        }

        if (Input.GetMouseButton(1) && canCastFireBall && currentMana >= fireBallCost)
        {

            canCastFireBall = false;
            fireBallDelayTimer = 0f; // Reset here ONLY when you actually cast

            Instantiate(fireBall, misileTransform.position, Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Cast");

            currentMana -= fireBallCost;
            manaBar.SetMana(currentMana);
            manaDelayTimer = 0f;

        }

        // Casting Magic Misile
        if (!canCast)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenCasting && currentMana >= misileCost)
            {
                canCast = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canCast)
        {

            canCast = false;
            Instantiate(magicMisile, misileTransform.position, Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Cast");
            currentMana -= misileCost;
            manaBar.SetMana(currentMana);
            manaDelayTimer += Time.deltaTime;

        }

        // Mana Focusing
        if (Input.GetKey(KeyCode.E) && playerMovement.isGrounded && manaFocusTimer <= maxManaFocusTime && canFocus)
        {
            ManaFocus();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            StopFocusing();
        }
        if(isFocusing && !playerMovement.isGrounded)
        {
            StopFocusing();
        }
        // Mana regeneration
        if (!isFocusing)
        {
            manaFocusTimer += Time.deltaTime;
            if(manaFocusTimer >= 0.5f)
            {
                canFocus = true;
            }
        }
        if (manaFocusTimer > maxManaFocusTime)
        {
            manaFocusTimer = maxManaFocusTime;
            // manaBar.SetMana(currentMana); Set for the manafocus bar, Like fireball slider but its a gauge for how long you can focus.
        }
    }

    private void StopFocusing()
    {
        isFocusing = false;
        playerMovement.moveSpeed = playerMovement.baseMoveSpeed;

        if(manaFocusingInstance.isPlaying)
        {
            manaFocusingInstance.Stop();
        }

        if (manaFocusCoroutine != null)
        {
            StopCoroutine(manaFocusCoroutine);
            manaFocusCoroutine = null;
        }
    }

    public void ManaFocus()
    {
        isFocusing = true;
        manaFocusTimer -= Time.deltaTime;
        playerMovement.moveSpeed = playerMovement.focusMoveSpeed;

        if(!manaFocusingInstance.isPlaying)
        {
            manaFocusingInstance.Play();
        }

        if (manaFocusCoroutine == null)
        {
            manaFocusCoroutine = StartCoroutine(ManaFocusRecharge());
        }

        if (manaFocusTimer <= 0)
        {
            manaFocusTimer = 0;
            canFocus = false;
            StopFocusing();
        }
    }

    private IEnumerator ManaFocusRecharge()
    {
        while (isFocusing)
        {
            if (currentMana < maxMana)
            {
                currentMana += manaFocusRate;
                manaBar.SetMana(currentMana);
            }

            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
            yield return new WaitForSeconds(manaFocusDelay);
        }
        manaFocusCoroutine = null; // reset reference when done
    }
}
