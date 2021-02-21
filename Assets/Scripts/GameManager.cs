using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject gameOverCanvas;
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    public AudioSource mainAudioSource;

    bool gameIsOver;

    // Start is called before the first frame update
    void Start()
    {
        mainCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);

        Guard.OnGuardSpotPlayer += ShowGameLoseUI;
        FindObjectOfType<PlayerControl>().OnReachedEndOfLevel += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                RestartLevel();
            }
        }
    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverCanvas.SetActive(true);
        mainCanvas.SetActive(false);
        gameOverUI.SetActive(true);
        gameIsOver = true;
        mainAudioSource.Stop();
        Guard.OnGuardSpotPlayer -= ShowGameLoseUI;
        FindObjectOfType<PlayerControl>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
    
    public  void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
