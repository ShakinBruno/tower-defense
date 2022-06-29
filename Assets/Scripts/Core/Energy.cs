using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;

public class Energy : MonoBehaviour, ISaveable
{
    [SerializeField] private int maxEnergy;
    [SerializeField] private float totalChargeDuration;

    private GameDataParser gameDataParser;
    private Coroutine activeCoroutine;
    private DateTime nextRechargeTime;
    private Notifications notifications;
    public int PlayerEnergy { get; set; }

    [Serializable]
    private struct EnergyData
    {
        public int energyOnQuit;
        public string timeOnQuit;

        public EnergyData(int energyOnQuit, DateTime timeOnQuit)
        {
            this.energyOnQuit = energyOnQuit;
            this.timeOnQuit = timeOnQuit.ToString(CultureInfo.CurrentCulture);
        }
    }

    private void Awake()
    {
        gameDataParser = FindObjectOfType<GameDataParser>();
        notifications = GetComponent<Notifications>();

        if (File.Exists(gameDataParser.SavePath))
        {
            gameDataParser.Load();
            
            if (PlayerEnergy >= maxEnergy)
            {
                MenuHUD.SetAddEnergyActive(false);
                MenuHUD.SetTimerActive(false);
            }
        }
        else
        {
            PlayerEnergy = maxEnergy;
            MenuHUD.UpdateEnergy(PlayerEnergy, maxEnergy);
            MenuHUD.SetAddEnergyActive(false);
            MenuHUD.SetTimerActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        notifications.ScheduleDayBreakNotification();
    }

    public void GetBonusEnergyPoint()
    {
        PlayerEnergy++;
        MenuHUD.UpdateEnergy(PlayerEnergy, maxEnergy);
        gameDataParser.Save();
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(RestoreEnergy());
    }

    private IEnumerator RestoreEnergy()
    {
        for (var i = 0; i < maxEnergy - PlayerEnergy; i++)
        {
            DateTime rechargeTime = nextRechargeTime.AddMinutes(totalChargeDuration / maxEnergy);

            while (DateTime.Now < rechargeTime)
            {
                TimeSpan timeSpan = rechargeTime - DateTime.Now;
                MenuHUD.UpdateTimer(timeSpan.Minutes, timeSpan.Seconds);
                yield return null;
            }

            PlayerEnergy++;
            MenuHUD.UpdateEnergy(PlayerEnergy, maxEnergy);
            nextRechargeTime = DateTime.Now;
        }

        MenuHUD.SetAddEnergyActive(false);
        MenuHUD.SetTimerActive(false);
    }

    public object SaveState()
    {
        DateTime time = DateTime.Now;
        notifications.ScheduleOneEnergyNotification(time.AddMinutes(totalChargeDuration / maxEnergy));
        notifications.ScheduleFullEnergyNotification(
            time.AddMinutes((maxEnergy - PlayerEnergy) * totalChargeDuration / maxEnergy));
        return new EnergyData(PlayerEnergy, time);
    }

    public void LoadState(object state)
    {
        var saveData = (EnergyData)state;
        PlayerEnergy = saveData.energyOnQuit;
        nextRechargeTime = DateTime.Parse(saveData.timeOnQuit);

        for (var i = 0; i < maxEnergy - PlayerEnergy; i++)
        {
            float minutesToRecharge = (i + 1) * totalChargeDuration / maxEnergy;
            DateTime rechargeTime = nextRechargeTime.AddMinutes(minutesToRecharge);

            if (DateTime.Now >= rechargeTime)
            {
                nextRechargeTime = nextRechargeTime.AddMinutes(minutesToRecharge);
                PlayerEnergy++;
            }
        }

        MenuHUD.UpdateEnergy(PlayerEnergy, maxEnergy);
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(RestoreEnergy());
        MenuHUD.SetAddEnergyActive(true);
        MenuHUD.SetTimerActive(true);
    }
}