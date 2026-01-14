using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Remeber if you add anymore level musics, add them to each .stop()In each level musicstart gameObjects
        AudioManager.instance.Stop("FirstLevelMusic");
        AudioManager.instance.Play("MainMenuMusic");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
