using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pursecounter : MonoBehaviour
{

    [SerializeField] Text pointText;

    int points = 0;

    private void Awake()
    {
        if (pointText == null)
        {
            GameObject hud = GameObject.FindGameObjectWithTag("HUDManager");
            if (hud != null)
                pointText = hud.GetComponentInChildren<Text>();
        }

        UpdateHUD();   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public int Points
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        pointText.text = points.ToString();
    } 
}
