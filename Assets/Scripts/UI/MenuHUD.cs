using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button addEnergyButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener(LoadRandomLevel);
        levelsButton.onClick.AddListener(SwitchToLevelsLayout);
        exitButton.onClick.AddListener(Application.Quit);
        menuButton.onClick.AddListener(SwitchToMenuLayout);

        for (var i = 0; i < levelButtons.Length; i++)
        {
            int sceneIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(sceneIndex));
        }
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
        levelsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();

        foreach (Button button in levelButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void LoadRandomLevel()
    {
        int randomBuildIndex = Random.Range(1, SceneManager.sceneCountInBuildSettings);
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
