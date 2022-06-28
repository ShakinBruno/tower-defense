using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Core HUD")] 
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI bankText;
    
    [Header("Wave HUD")] 
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Button promptButton;

    [Header("World HUD")]
    [SerializeField] private TextMeshPro waveText;
    [SerializeField] private TextMeshPro healthText;
    
    [Header("Content/Edit Layouts")] 
    [SerializeField] private GameObject contentLayout;
    [SerializeField] private GameObject editLayout;
    [SerializeField] private Button sellButton;
    [SerializeField] private ContentConfig[] contentConfigs;

    [Header("Pause Screen")] 
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    
    [Header("Colors")]
    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectedColor;

    [Serializable]
    private struct ContentConfig
    {
        public Button button;
        public Image image;
    }
    
    private Game game;
    private static GameHUD instance;
    public static void UpdateWaveInfo(string info) => instance.waveInfoText.text = info;
    public static void UpdateHealth(int currentHealth) => instance.healthText.text = $"{currentHealth} LIVES";
    public static void UpdatePrompt(string prompt) => instance.promptText.text = prompt;
    public static void UpdateBank(int amount) => instance.bankText.text = $"{amount}$";
    public static void SetPromptLabelActive(bool active) => instance.promptPanel.SetActive(active);
    public static void SetContentLayoutActive(bool active) => instance.contentLayout.gameObject.SetActive(active);
    public static void SetManageLayoutActive(bool active) => instance.editLayout.gameObject.SetActive(active);
    public static void UpdateWaveText(int currentWave, int totalWaves) =>
        instance.waveText.text = $"WAVE {currentWave}/{totalWaves}";
    public static void UpdateTimer(int minutes, int seconds) =>
        instance.waveTimerText.text = $"{minutes:D2}:{seconds:D2}";

    private void Awake()
    {
        instance = this;
        game = GetComponent<Game>();
    }

    private void OnEnable()
    {
        pauseButton.onClick.AddListener(() => PauseGame(true));
        resumeButton.onClick.AddListener(() => PauseGame(false));
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        speedButton.onClick.AddListener(() => ChangeGameSpeed(speedText));
        promptButton.onClick.AddListener(() => game.StartOrSkipWave(game.HasGameStarted));
        sellButton.onClick.AddListener(() => game.SellContent());
        
        contentConfigs[0].button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Obstacle, TowerType.None, contentConfigs[0].image));
        contentConfigs[1].button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Laser, contentConfigs[1].image));
        contentConfigs[2].button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Mortar, contentConfigs[2].image));
        contentConfigs[3].button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.RocketLauncher, contentConfigs[3].image));
    }

    private void OnDisable()
    {
        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        speedButton.onClick.RemoveAllListeners();
        promptButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();

        foreach (ContentConfig config in contentConfigs)
        {
            config.button.onClick.RemoveAllListeners();
        }
    }

    private void PauseGame(bool pause)
    {
        game.PauseGame(pause);
        pausePanel.SetActive(pause);
    }

    private void RestartGame()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelIndex);
    }

    private void ChangeGameSpeed(TextMeshProUGUI text)
    {
        float gameSpeed = game.ChangeGameSpeed();
        text.text = $"{gameSpeed}x";
    }

    private void ChangeSelectedType(TileContentType type, TowerType towerType, Image image)
    {
        if (type == TileContentType.Tower)
        {
            game.SelectedType = TileContentType.Tower;

            if (game.SelectedTowerType == towerType)
            {
                game.SelectedType = TileContentType.None;
                game.SelectedTowerType = TowerType.None;
                image.color = baseColor;
            }
            else
            {
                game.SelectedTowerType = towerType;
                image.color = selectedColor;
                ResetButtonColorsExcept(image);
            }
        }
        else
        {
            game.SelectedTowerType = TowerType.None;

            if (game.SelectedType == type)
            {
                game.SelectedType = TileContentType.None;
                image.color = baseColor;
            }
            else
            {
                game.SelectedType = type;
                image.color = selectedColor;
                ResetButtonColorsExcept(image);
            }
        }
    }

    private void ResetButtonColorsExcept(Image image)
    {
        foreach (ContentConfig config in contentConfigs)
        {
            if (config.image != image) config.image.color = baseColor;
        }
    }
}