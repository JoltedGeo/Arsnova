using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class FlyingEnemyGraphic : MonoBehaviour
{
    public AIPath aIPath;
    
    void Update()
    {
        if (aIPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(5f, 5f, 5f);
        }
        else if (aIPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-5f, 5f, 5f);
        }
    }
}
