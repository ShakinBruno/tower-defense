using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Color baseColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private ButtonMenuConfig pause;
    [SerializeField] private ButtonMenuConfig speed;
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private PromptConfig interactionPrompt;
    [SerializeField] private ButtonContentConfig obstacle;
    [SerializeField] private ButtonContentConfig laserTower;
    [SerializeField] private ButtonContentConfig mortar;
    [SerializeField] private TextMeshPro waveText;
    [SerializeField] private TextMeshPro healthText;

    [Serializable]
    private struct PromptConfig
    {
        public GameObject prompt;
        public TextMeshProUGUI text;
        public Button button;
    }

    [Serializable]
    private struct ButtonMenuConfig
    {
        public Button button;
        public TextMeshProUGUI text;
    }

    [Serializable]
    private struct ButtonContentConfig
    {
        public Button button;
        public Image image;
    }
    
    private Game game;
    private static GameHUD instance;
    public static void UpdateWaveInfo(string info) => instance.waveInfoText.text = info;
    public static void UpdateHealth(int currentHealth) => instance.healthText.text = $"{currentHealth} LIVES";
    public static void UpdatePrompt(string prompt) => instance.interactionPrompt.text.text = prompt;
    public static void SetPromptLabelActive(bool active) => instance.interactionPrompt.prompt.SetActive(active);
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
        pause.button.onClick.AddListener(() => PauseOrResumeGame(pause.text));
        speed.button.onClick.AddListener(() => ChangeGameSpeed(speed.text));
        interactionPrompt.button.onClick.AddListener(() => game.StartOrSkipWave(game.HasGameStarted));
        obstacle.button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Obstacle, TowerType.None, obstacle.image));
        laserTower.button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Laser, laserTower.image));
        mortar.button.onClick.AddListener(() =>
            ChangeSelectedType(TileContentType.Tower, TowerType.Mortar, mortar.image));
    }

    private void OnDisable()
    {
        pause.button.onClick.RemoveAllListeners();
        speed.button.onClick.RemoveAllListeners();
        interactionPrompt.button.onClick.RemoveAllListeners();
        obstacle.button.onClick.RemoveAllListeners();
        laserTower.button.onClick.RemoveAllListeners();
        mortar.button.onClick.RemoveAllListeners();
    }
    
    private void PauseOrResumeGame(TextMeshProUGUI text)
    {
        bool isGamePaused = game.PauseOrResumeGame();
        text.text = isGamePaused ? "I>" : "II";
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
        if (obstacle.image == image)
        {
            laserTower.image.color = baseColor;
            mortar.image.color = baseColor;
        }
        else if (laserTower.image == image)
        {
            obstacle.image.color = baseColor;
            mortar.image.color = baseColor;
        }
        else if (mortar.image == image)
        {
            obstacle.image.color = baseColor;
            laserTower.image.color = baseColor;
        }
    }
}