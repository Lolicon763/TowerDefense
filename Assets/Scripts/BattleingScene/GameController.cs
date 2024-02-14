using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController GameControllerInstance { get; private set; }
    private int health;
    public GameObject CombatObjects;
    public GameObject HealthBarsParent;
    private void Awake()
    {
        
        if (GameControllerInstance == null)
        {
            GameControllerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void MinusCastleHealth(int amount)
    {
        if (health < amount)
        {
            health = 0;
            OnGameOver();
        }
        else
        {
            health -= amount;
        }
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
    }
    public void Resume()
    {
        Time.timeScale = 1.0f;
    }
    private void OnGameOver()
    {
        Debug.Log("GameOver");
    }
}
