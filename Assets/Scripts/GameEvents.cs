using UnityEngine;
using System;

public class GameEvents
{
    public static GameEvents m_instance;

    // public static GameEvents Instance...
    public static GameEvents Instance{
        get{
            if(m_instance == null){
                m_instance = new GameEvents();
            }
            return m_instance;
        }
    }
    
    public event Action onRetry;
    
    public event Action<int, bool> onAddToScore;
    public event Action<int> onDeductLives;
    public event Action onPlayerDied;

     public event Action onGameOver;
     public event Action onInitialAstroidsSpawned;


    public void OnRetry(){
        onRetry?.Invoke();
    }
     public void OnGameOver(){
        onGameOver?.Invoke();
     }

     public void InitialAstroidsSpawned(){
        onInitialAstroidsSpawned?.Invoke();
     }


    /// <summary>
    /// dispatches the onAddToScore event to increment the score by the given amount
    /// </summary>
    /// <param name="amount">the amount to increment the score by</param>
     public void AddToScore(int amount, bool playSound = false){
        onAddToScore?.Invoke(amount, playSound);
     }

    /// <summary>
    /// dispatches the onDeductLives event to decrement the lives by the given amount
    /// </summary>
    /// <param name="amount">the amount to decrement the lives by</param>
     public void DeductFromLives(int amount){
        onDeductLives?.Invoke(amount);
     }

     public void PlayerDied(){
        onPlayerDied?.Invoke();
     }
}
