using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public GameObject gameEndpanel;
    private bool isGameStart = false;
    public void Start()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        CheckPlayers();
    }
    public void CheckPlayers() 
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length >= 2 && !isGameStart)
        {
            StartGame();
        }
        if (GameObject.FindGameObjectsWithTag("Player").Length < 2 && isGameStart) 
        {
            EndGame();
        }
        
    }


    private void StartGame() 
    {
        GameObject.FindGameObjectWithTag("waiting_label").SetActive(false);
        Time.timeScale = 1f;
        isGameStart = true;
    }

    private void EndGame() 
    {
        Time.timeScale = 0.1f;
        gameEndpanel.SetActive(true);
    }
}
