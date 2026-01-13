using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusicOnScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        FindObjectOfType<AudioManager>().Play("FirstLevelMusic");
        FindObjectOfType<AudioManager>().Stop("MainMenuMusic");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
