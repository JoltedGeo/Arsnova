using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.UIElements;

public class MoneyHandler : MonoBehaviour
{
    public float purse = 0;               // Value that increases each press
    public float rechargeDelay = .5f;    // Time (seconds) before you can press again

    private float rechargeTimer = 0f;   // Internal timer tracking cooldown
    public float pressAmount = 1;


    void Update()
    {
        // Reduce timer over time
        if (rechargeTimer > 0f)
        {
            rechargeTimer -= Time.deltaTime;
        }

        // Only allow pressing space when timer is done
        if (Input.GetKeyDown(KeyCode.F) && rechargeTimer <= 0f)
        {
            AddMoney(); // Add to purse
            Debug.Log("Gained: " + pressAmount);

            // Reset timer
            rechargeTimer = rechargeDelay;
        }
    }

    public void AddMoney()
    {
        purse += pressAmount;
    }
}
