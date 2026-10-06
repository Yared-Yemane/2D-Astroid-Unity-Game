using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int m_score;
    [SerializeField] private int m_highScore;   
    [SerializeField] private int m_currentLives;   
    [SerializeField] private int m_maxLives;   
    [SerializeField] private SoundEffectHandler m_collectibleSound;   

    private void Awake(){
        ClearScore();
    }


    /// <summary>
    /// adds the given amount to the current score
    /// </summary>
    /// <param name="amount">the amount to add to the current score</param>
    public void AddScore(int amount = 1, bool playSound = false){
        m_score += amount;
        if(m_score > m_highScore){
            m_highScore = m_score;
        }
        if(playSound){
            m_collectibleSound.Play();
        }
    }

    public void DeductLives(int amount){
        m_currentLives -= amount;
    }

    public void OnEnable(){
        GameEvents.Instance.onAddToScore += AddScore;
        GameEvents.Instance.onDeductLives += DeductLives;
        GameEvents.Instance.onPlayerDied += PlayerDied;
        GameEvents.Instance.onRetry += OnRetry;
    }

    public void OnDisable(){
        GameEvents.Instance.onAddToScore -= AddScore;
        GameEvents.Instance.onDeductLives -= DeductLives;
        GameEvents.Instance.onPlayerDied -= PlayerDied;
        GameEvents.Instance.onRetry -= OnRetry;
    }

    private void OnRetry(){
        ClearScore();
    }

    /// <summary>
    /// clears the current score
    /// </summary>
    public void ClearScore(){
        m_score = 0;
        m_currentLives = m_maxLives;
    }

    public void PlayerDied(){
        m_currentLives--;
        if(m_currentLives <= 0){
            GameEvents.Instance.OnGameOver();
            Time.timeScale = 0;
        }
    }
}
