using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelingManager : MonoBehaviour
{
    public int level = 1;
    public int currentXp;
    private int FEXpGain = 15;
    public int[] xpPerLevel = {0, 100, 250, 500, 900, 1400, 1900, 3550, 4100, 4900};//10 levels. lvl 1 is index 0
    private int adminXpGain= 50;

    public delegate void OnXpChanged();
    public event OnXpChanged onXpChanged;

    public void Start()
    {
        if(level == 0)
        {
            level = 1;
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            GainXp(adminXpGain);
        }
    }

    public void FlyingEnemyXp()
    {
        GainXp(FEXpGain);
    }

    public void GainXp (int amount)
    {
        currentXp += amount;
        Debug.Log("Got " + FEXpGain + " Exp");

        while(currentXp >= xpPerLevel[level])
        {
            currentXp -= xpPerLevel[level];
            LevelUp();
        }
        
        onXpChanged?.Invoke();
    }

    void LevelUp()
    {
        level++;

        Debug.Log("Leveled up! New level: " + level);

        if (level >= xpPerLevel.Length)
        {
            level = xpPerLevel.Length - 1;
            currentXp = xpPerLevel[level];
        }
    }

    public int GetXpForNextLevel()
    {
        
    if (level < 0 || level >= xpPerLevel.Length)
        return 1; // never return 0

        return xpPerLevel[level];   
    }
}
