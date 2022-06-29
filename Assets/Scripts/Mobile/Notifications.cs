using System;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    private const string energyChannelID = "energyChannel";
    private const string reminderChannelID = "reminderChannel";
    private const int oneEnergyID = 1;
    private const int fullEnergyID = 2;
    private const int dayBreakID = 3;
    private AndroidNotificationChannel energyNotificationChannel;
    private AndroidNotificationChannel reminderNotificationChannel;
    private AndroidNotification oneEnergyNotification;
    private AndroidNotification fullEnergyNotification;
    private AndroidNotification dayBreakReminderNotification;
    
    private void Awake()
    {
        energyNotificationChannel = new AndroidNotificationChannel
        {
            Id = energyChannelID,
            Name = "Energy Recharge Channel",
            Description = "Notify user when energy is recharged",
            Importance = Importance.Default
        };

        reminderNotificationChannel = new AndroidNotificationChannel
        {
            Id = reminderChannelID,
            Name = "Play Reminder Channel",
            Description = "Remind user to play the game",
            Importance = Importance.High
        };

        oneEnergyNotification = new AndroidNotification
        {
            Title = "You received one energy point!",
            Text = "Let's slay some opponents!",
            SmallIcon = "default",
            LargeIcon = "default"
        };

        fullEnergyNotification = new AndroidNotification
        {
            Title = "Energy is fully recharged!",
            Text = "Play the game before opponents destroy our base!",
            SmallIcon = "default",
            LargeIcon = "default",
        };

        dayBreakReminderNotification = new AndroidNotification
        {
            Title = "We miss you!",
            Text = "You haven't played the game for 24 hours!",
            SmallIcon = "default",
            LargeIcon = "default"
        };
        
        AndroidNotificationCenter.RegisterNotificationChannel(energyNotificationChannel);
        AndroidNotificationCenter.RegisterNotificationChannel(reminderNotificationChannel);
    }

    public void ScheduleOneEnergyNotification(DateTime time)
    {
        oneEnergyNotification.FireTime = time;
        NotificationStatus notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(oneEnergyID);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(
                oneEnergyID, 
                oneEnergyNotification, 
                energyChannelID);
        }
        else
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(
                oneEnergyNotification, 
                energyChannelID, 
                oneEnergyID);
        }
    }

    public void ScheduleFullEnergyNotification(DateTime time)
    {
        fullEnergyNotification.FireTime = time;
        NotificationStatus notificationStatusOneEnergy = 
            AndroidNotificationCenter.CheckScheduledNotificationStatus(oneEnergyID);
        NotificationStatus notificationStatusFullEnergy =
            AndroidNotificationCenter.CheckScheduledNotificationStatus(fullEnergyID);

        if (notificationStatusOneEnergy == NotificationStatus.Delivered)
        {
            AndroidNotificationCenter.CancelNotification(oneEnergyID);
        }

        if (notificationStatusFullEnergy == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(
                fullEnergyID,
                fullEnergyNotification,
                energyChannelID);
        }
        else
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(
                fullEnergyNotification, 
                energyChannelID, 
                fullEnergyID);
        }
    }
    
    public void ScheduleDayBreakNotification()
    {
        dayBreakReminderNotification.FireTime = DateTime.Now.AddDays(1);
        NotificationStatus notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(dayBreakID);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(
                dayBreakID, 
                dayBreakReminderNotification, 
                reminderChannelID);
        }
        else
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(
                dayBreakReminderNotification, 
                reminderChannelID, 
                dayBreakID);
        }
    }
}