using System.Collections;
using System.Collections.Generic;
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

    //Magic Ritual Casting
    /*
        Ideas for implementing

        First off when holding down RMB The player's going to be put into a state of casting where movement speed
        will be slowed down and will enable you to use a set ammount of elements like air, fire, ect. While in this state
        you will be able to input up to 6 spells with pre-determined spell combos. 

        Side note, for the animations make it so
        each time the player inputs a spell particles of the same element will burst out as vfx for visual feedback.
        When in the ritual state, have a magic circle apear in front of the wand to indicate casting.

        A list of each element and their property's 
            - Air: This element deals with movement and fast moving projectiles and can be mainly used to increase
                projectile speed, simple spells can be cast with great firing speed
            - Fire: This element deals with more explosive damage at the cost of casting speed and sometimes projectile speed.
                Fire spells mainly focus on AoE and effect size
            - Water: This element is used mainly for crowd control and multi-combo moves ina mid-range melee style like a water
                whip or like water elementa
            - Nature: Nature spells act as the games primary buffs where you can apply armor resistence or increased mana regeneration.
                This element should also include close range melee attacks by conjoring martial weapons
            - Shadow: This element should focus on self augmenting effects like increased speed, increased magic power for lower resistances.
                Shadow magic should focus on more long range attacks with quick attack speed and percision.

            A note before the list of spells:
                While in the ritual state after you input the first element you want, there will be a list of spells and subsequent elements you can
                use input so you are reminded of what to press to cast the spell. Eg. Like combo list while you're casting

        A list of simple spells to get started (2 minimum in each catagory)

            Air Based Attacks
                - 2 Air - this spell casts 3 quick shots of air and high speeds with a small recharge time and medium mana cost

                - 1 Air, 1 Fire, 1 Air - Increased Jump highed with the ability to slowly fall my holding space for a certain amount of time

                - 1 Water, 2 Air - This spell combo give the player increased movespeed for a duration of time. The spell works by letting the caster
                    run on produced water at his feet and using pressured air to glide the character.

            Fire Based Attacks
                - 1 Fire, 1 Water - This spell creates a burst of steam that propells the caster in the opposite direction

                - 3 fire - This spell casts a fireball which does higher damage with an aoe explosion at the cost of high mana and a longer cooldown
            
            Water Based Attacks
                - 1 Water, 1 Shadow, 1 Water - this spell releases a 1 water whip attack in the direction aimed at with icreased range.
                    It deals moderate to high damage at the price of higher mana mana

                - 2 water - This casts a water whip with a 2 attack combo at medium range offering a lower cooldown rate and consistent damage

            Shadow Based Attacks
                - 2 Shadow, 1 Nature - This spell imbuses the caster with increased magic power for draining mana

                - 1 Shadow, 1 Air, 1 Shadow -

            Nature Based Augments
                - 2 Nature - this spell increases armor resistence for a duration of time

                - 1 Nature, 1 Shadow, 1 Water - This spell increases mana regeneration and increases the rate which the syphon bar fills up
    */
}
