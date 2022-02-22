using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    private const string LastTimeKeyDaily = "LastRewardTimeDaily";
    private const string ActiveSlotKeyDaily = "ActiveSlotDaily";

    private const string LastTimeKeyWeekly = "LastRewardTimeWeekly";
    private const string ActiveSlotKeyWeekly = "ActiveSlotWeekly";


    #region Fields
    [Header("Time settings")]
    [SerializeField]
    public int TimeCooldownDaily = 86400;
    [SerializeField]
    public int TimeDeadlineDaily = 172800;

    [SerializeField]
    public int TimeCooldownWeekly = 604800;
    [SerializeField]
    public int TimeDeadlineWeekly = 1209600;

    [Space]
    [Header("RewardSettings")]
    public List<Reward> DailyRewards;
    public List<Reward> WeeklyRewards;


    [Header("UI")]
    [SerializeField]
    public TMP_Text RewardTimerDaily;
    [SerializeField]
    public TMP_Text RewardTimerWeekly;

    [SerializeField]
    public Transform SlotsParent;
    [SerializeField]
    public SlotRewardView SlotPrefab;
    [SerializeField]
    public Button ResetButton;
    [SerializeField]
    public Button GetRewardButtonDaily;

    [SerializeField]
    public Button GetRewardButtonWeekly;

    [SerializeField]
    public Image TimeLineDaily;

    [SerializeField]
    public Image TimeLineWeekly;




    #endregion

    public int CurrentActiveSlotDaily
    {
        get => PlayerPrefs.GetInt(ActiveSlotKeyDaily);
        set => PlayerPrefs.SetInt(ActiveSlotKeyDaily, value);
    }



    public int CurrentActiveSlotWeekly
    {
        get => PlayerPrefs.GetInt(ActiveSlotKeyDaily);
        set => PlayerPrefs.SetInt(ActiveSlotKeyDaily, value);
    }






    public DateTime? LastRewardTimeDaily
    {
        get
        {
            var data = PlayerPrefs.GetString(LastTimeKeyDaily);
            if (string.IsNullOrEmpty(data))
                return null;
            return DateTime.Parse(data);
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString(LastTimeKeyDaily, value.ToString());
            else
                PlayerPrefs.DeleteKey(LastTimeKeyDaily);
        }
    }

    public DateTime? LastRewardTimeWeekly
    {
        get
        {
            var data = PlayerPrefs.GetString(LastTimeKeyWeekly);
            if (string.IsNullOrEmpty(data))
                return null;
            return DateTime.Parse(data);
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString(LastTimeKeyWeekly, value.ToString());
            else
                PlayerPrefs.DeleteKey(LastTimeKeyWeekly);
        }
    }


    private void OnDestroy()
    {
        GetRewardButtonDaily.onClick.RemoveAllListeners();
        ResetButton.onClick.RemoveAllListeners();
        GetRewardButtonWeekly.onClick.RemoveAllListeners();
    }

}
