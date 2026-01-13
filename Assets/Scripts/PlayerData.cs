using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int totalExperience;

    public PlayerData (LevelingManager player)
    {
        level = player.level;
        totalExperience = player.totalExperience;

    }

}
