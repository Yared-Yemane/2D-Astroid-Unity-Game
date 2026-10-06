using UnityEngine;
using UnityEngine.UIElements;


public class HUD : MonoBehaviour
{

    [SerializeField] private SoundEffectHandler m_focusSound;
    [SerializeField] private SoundEffectHandler m_submitSound;
    [SerializeField] private ScoreManager m_scoreManager;

    private VisualElement m_gameOverScreen;

    private UIDocument m_uiDocument;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable(){
        m_uiDocument = gameObject.GetComponent<UIDocument>();
        VisualElement root = m_uiDocument.rootVisualElement;

        VisualElement container = root.Q<VisualElement>("Container");
        container.dataSource = m_scoreManager;

        VisualElement retryButton = root.Q<VisualElement>("RetryButtonLabel");
        VisualElement exitButton = root.Q<VisualElement>("ExitButtonLabel");

        Clickable retryButtonClickable = new Clickable(()=>{
            HandleRetryButtonClick();
        });

        Clickable exitButtonClickable = new Clickable(()=>{
            HandleExitButtonClick();
        });

        retryButton.AddManipulator(retryButtonClickable);
        retryButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));

        exitButton.AddManipulator(exitButtonClickable);
        exitButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));

        VisualElement topBar = root.Q<VisualElement>("TopBar"); 
        topBar.dataSource = m_scoreManager;

        m_gameOverScreen = root.Q<VisualElement>("GameOver");
        GameEvents.Instance.onGameOver += OnGameOver;
        GameEvents.Instance.onRetry += OnRetry;
    }

    private void HandleExitButtonClick(){
        Application.Quit();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

    private void HandleRetryButtonClick(){
        GameEvents.Instance.OnRetry();
    }

    private void OnDisable(){
        GameEvents.Instance.onGameOver -= OnGameOver;
        GameEvents.Instance.onRetry -= OnRetry;
    }

    private void OnGameOver(){
        m_gameOverScreen.RemoveFromClassList("hidden");
    }

    private void OnRetry(){
        m_gameOverScreen.AddToClassList("hidden");
        Time.timeScale = 1;
    }
}
