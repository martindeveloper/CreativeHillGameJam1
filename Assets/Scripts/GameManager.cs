﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    [System.Serializable]
    public class GameSettings
    {
        public int PlayerHealth = 3;
        public PhysicsMaterial2D DeadEntityPhysicsMaterial;
        public GameObject PlayerBulletEntity;
        public GameObject EnemyBulletEntity;
    }

    public UnityEngine.UI.Text KeysCounterText;
    public UnityEngine.UI.Text GameStateText;
    public Sprite HeartSprite;
    public GameObject HeartsContainer;
    public GameObject RestartButton;

    public GameSettings Settings = new GameSettings();

    public static GameManager Instance;

    private Stack<GameObject> HeartsBuffer = new Stack<GameObject>();
    private int CurrentPlayerHealth;
    private int CurrentPlayerKeys = 0;
    private int MaximumKeys = 3;
    
    public int KeysCount { get { return MaximumKeys;} set {} }

    [System.Serializable]
    public enum GameState
    {
        NotRunnig,
        Playing,
        GameOver,
        Win
    }

    public GameState State = GameState.NotRunnig;

    public void Awake()
    {
        Instance = this;
        
        // Count keys on scene
        GameObject[] keyObjects = GameObject.FindGameObjectsWithTag("Key") as GameObject[];
        MaximumKeys = keyObjects.Length;
    }

    public void Start()
    {
        State = GameState.Playing;

        CurrentPlayerHealth = Settings.PlayerHealth;

        GenerateHearts();
        RedrawKeysCounter();
     }

    private void GenerateHearts()
    {
        GameObject heartPrototype = new GameObject();
        heartPrototype.isStatic = true;
        UnityEngine.UI.Image image = heartPrototype.AddComponent<UnityEngine.UI.Image>();
        image.sprite = HeartSprite;

        Transform heartsTransform = HeartsContainer.GetComponent<Transform>();

        for (int i = 0; i < Settings.PlayerHealth; i++)
        {
            GameObject heart = Instantiate(heartPrototype, heartsTransform.position, Quaternion.identity) as GameObject;

            if (heart != null)
            {
                RectTransform heartTransform = heart.GetComponent<RectTransform>();
                heartTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                heartTransform.SetParent(heartsTransform, false);

                Vector3 heartPosition = heartTransform.anchoredPosition;
                heartPosition.x = i * 60;

                heartTransform.anchoredPosition = heartPosition;
            }

            HeartsBuffer.Push(heart);
        }

        Destroy(heartPrototype);
    }

    public void OnPlayerDeath()
    {
        GameObject heart = HeartsBuffer.Pop();
        Destroy(heart);

        CurrentPlayerHealth--;

        if(CurrentPlayerHealth == 0)
        {
            OnGameOver();
        }
    }
    
    private void RedrawKeysCounter()
    {
        KeysCounterText.text = string.Format("Keys {0}/{1}", CurrentPlayerKeys, MaximumKeys);
    }
    
    public void OnKeyPickup()
    {
        CurrentPlayerKeys++;
        
        RedrawKeysCounter();
    }

    public void OnGameRestart()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }

    public void OnGameOver()
    {
        GameStateText.text = "Damn, try it again.";

        if(RestartButton != null)
        {
            RestartButton.SetActive(true);
        }

        State = GameState.GameOver;
    }

    public void OnWin()
    {
        GameStateText.text = "Nice, you won the game!";
        State = GameState.Win;
    }
}
