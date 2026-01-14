using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public LevelingManager playerData;
    [SerializeField] GameObject playerDataHolder;

    public bool gameIsPaused = false;
    public GameObject gameOverScreenUI;
    public GameObject pauseMenuUI;
    public TMP_Text totalXpText;
    public TMP_Text TotalXpDisplayText;

    void Awake()
    {
        playerData = playerDataHolder.GetComponent<LevelingManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        TotalXpDisplayText.text = playerData.totalExperience.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void GameOver()
    {
        gameOverScreenUI.SetActive(true);
        totalXpText.text = playerData.totalExperience.ToString();
        
        Time.timeScale = 0.40f;
    }

    public void Restart()
    {
        //Change the scene you load into based off of gameplay, maybe a replay level and back to hub
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
}