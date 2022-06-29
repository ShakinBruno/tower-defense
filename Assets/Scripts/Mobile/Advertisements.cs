using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class Advertisements : MonoBehaviour, IUnityAdsListener
{
    private const string rewardedID = "Rewarded_Android";
    private const string interstitialID = "Interstitial_Android";
    private const string gameID = "4818489";
    private static Advertisements instance;
    private Energy energy;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        energy = FindObjectOfType<Energy>();
        Advertisement.Initialize(gameID, false);
    }

    private void OnEnable()
    {
        Advertisement.AddListener(this);
    }

    private void OnDisable()
    {
        Advertisement.RemoveListener(this);
    }

    public static void ShowRewarded()
    {
        Advertisement.Show(rewardedID);
    }
    
    public static void ShowInterstitial()
    {
        Advertisement.Show(interstitialID);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished || showResult == ShowResult.Skipped)
        {
            switch (placementId)
            {
                case rewardedID:
                    energy.GetBonusEnergyPoint();
                    break;
                case interstitialID:
                    SceneManager.LoadScene(0);
                    break;
            }
        }
        else
        {
            GameHUD.SetErrorActive(true);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }
}