using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField] pursecounter pointHUD;
    public float passivePointDelay =2.5f;

    private void Start()
    {
        if (pointHUD == null)
        {
            pointHUD = FindObjectOfType<pursecounter>();
            if (pointHUD == null)
            {
                Debug.LogError("❌ PointCounter: No pursecounter found in scene!");
                return;
            }
        }

        Debug.Log("✅ PointCounter: Found pursecounter " + pointHUD.name);
        StartCoroutine(CountPoints());
    }

    private IEnumerator CountPoints()
    {
        while (true)
        {
            pointHUD.Points += 5;

            yield return new WaitForSeconds(passivePointDelay);
        }
    }
}
