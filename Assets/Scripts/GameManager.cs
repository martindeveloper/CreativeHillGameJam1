using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public UnityEngine.UI.Text GameStateText;

    public static GameManager Instance; 

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
    }

    public void Start()
    {
        State = GameState.Playing;
    }

    public void OnGameOver()
    {
        GameStateText.text = "Damn, try it again.";
        State = GameState.GameOver;
    }

    public void OnWin()
    {
        GameStateText.text = "Nice, you won the game!";
        State = GameState.Win;
    }
}
