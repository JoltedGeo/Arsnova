using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingManager : MonoBehaviour
{
    public int level;
    public int totalExperience;
    private int FEXpGain = 15;

    public void Start()
    {
        if(level == 0)
        {
            level = 1;
        }
    }

    public void FlyingEnemyXp()
    {
        Debug.Log("Got " + FEXpGain + " Exp");
        totalExperience += FEXpGain;
    }
}
