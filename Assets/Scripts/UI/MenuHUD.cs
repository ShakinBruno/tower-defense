using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Energy))]
public class MenuHUD : MonoBehaviour
{
    [Header("Menu Layout")] 
    [SerializeField] private GameObject menuLayout;
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button exitButton;

    [Header("Levels Layout")]
    [SerializeField] private GameObject levelsLayout;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button[] levelButtons;

    [Header("Energy Bar")] 
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private GameObject timerLabel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button addEnergyButton;

    [Header("Error Pop-Up")] 
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Button closeButton;

    private static MenuHUD instance;
    private Energy energy;
    private GameDataParser gameDataParser;
    private Advertisements advertisements;
    public static void UpdateEnergy(int currentEnergy, int maxEnergy) =>
        instance.energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
    public static void UpdateTimer(int minutes, int seconds) => instance.timerText.text = $"{minutes:D2}:{seconds:D2}";
    public static void SetTimerActive(bool active) => instance.timerLabel.SetActive(active);
    public static void SetAddEnergyActive(bool active) => instance.addEnergyButton.gameObject.SetActive(active);
    public static void SetErrorActive(bool active) => instance.errorPanel.SetActive(active);

    private void Awake()
    {
        instance = this;
        energy = GetComponent<Energy>();
        gameDataParser = FindObjectOfType<GameDataParser>();
        advertisements = FindObjectOfType<Advertisements>();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(LoadRandomLevel);
        levelsButton.onClick.AddListener(SwitchToLevelsLayout);
        exitButton.onClick.AddListener(Application.Quit);
        menuButton.onClick.AddListener(SwitchToMenuLayout);
        addEnergyButton.onClick.AddListener(Advertisements.ShowRewarded);
        closeButton.onClick.AddListener(() => SetErrorActive(false));

        for (var i = 0; i < levelButtons.Length; i++)
        {
            int sceneIndex = i + 1;
            levelButtons[i].onClick.AddListener(() =>
            {
                if (energy.PlayerEnergy <= 0) return;
                energy.PlayerEnergy--;
                gameDataParser.Save();
                SceneManager.LoadScene(sceneIndex);
            });
        }
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
        levelsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        addEnergyButton.onClick.RemoveAllListeners();

        foreach (Button button in levelButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void LoadRandomLevel()
    {
        if (energy.PlayerEnergy <= 0) return;
        int randomBuildIndex = Random.Range(1, SceneManager.sceneCountInBuildSettings);
        energy.PlayerEnergy--;
        gameDataParser.Save();
        SceneManager.LoadScene(randomBuildIndex);
    }

    private void SwitchToLevelsLayout()
    {
        menuLayout.SetActive(false);
        levelsLayout.SetActive(true);
    }
    
    private void SwitchToMenuLayout()
    {
        levelsLayout.SetActive(false);
        menuLayout.SetActive(true);
    }
}
