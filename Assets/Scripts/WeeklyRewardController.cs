using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyRewardController
{
    private readonly RewardView _rewardView;
    private List<SlotRewardWeeklyView> _slots;

    private bool _rewardReceived = false;

    public WeeklyRewardController(RewardView rewardView)
    {
        _rewardView = rewardView;
        InitSlots();
        RefreshUi();
        _rewardView.StartCoroutine(UpdateCoroutine());
        SubscribeButtons();
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            Update();
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        RefreshRewardState();
        RefreshUi();
    }

    private void RefreshRewardState()
    {
        _rewardReceived = false;
        if (_rewardView.LastRewardTimeWeekly.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _rewardView.LastRewardTimeWeekly.Value;
            if (timeSpan.Seconds > _rewardView.TimeDeadlineWeekly)
            {
                _rewardView.LastRewardTimeWeekly = null;
                _rewardView.CurrentActiveSlotWeekly = 0;
            }
            else if (timeSpan.Seconds < _rewardView.TimeCooldownWeekly)
            {
                _rewardReceived = true;
            }
        }
    }

    private void RefreshUi()
    {
        _rewardView.GetRewardButtonWeekly.interactable = !_rewardReceived;

        for (var i = 0; i < _rewardView.WeeklyRewards.Count; i++)
        {
            _slots[i].SetData(_rewardView.WeeklyRewards[i], i + 1, i <= _rewardView.CurrentActiveSlotWeekly);
        }

        DateTime nextDailyBonusTime =
            !_rewardView.LastRewardTimeWeekly.HasValue
                ? DateTime.MinValue
                : _rewardView.LastRewardTimeWeekly.Value.AddSeconds(_rewardView.TimeCooldownWeekly);
        var delta = nextDailyBonusTime - DateTime.UtcNow;
        if (delta.TotalSeconds < 0)
            delta = new TimeSpan(0);

        _rewardView.RewardTimerWeekly.text = delta.ToString();

        _rewardView.TimeLineWeekly.fillAmount = (_rewardView.TimeCooldownWeekly - (float)delta.TotalSeconds) / _rewardView.TimeCooldownWeekly;
    }

    private void InitSlots()
    {
        _slots = new List<SlotRewardWeeklyView>();
        for (int i = 0; i < _rewardView.WeeklyRewards.Count; i++)
        {
            var reward = _rewardView.WeeklyRewards[i];
            var slotInstance = GameObject.Instantiate(_rewardView.SlotPrefabWeekly, _rewardView.SlotsParent, false);
            slotInstance.SetData(reward, i + 1, false);
            _slots.Add(slotInstance);
        }
    }

    private void SubscribeButtons()
    {
        _rewardView.GetRewardButtonWeekly.onClick.AddListener(ClaimReward);
        _rewardView.ResetButton.onClick.AddListener(ResetReward);
    }

    private void ResetReward()
    {
        _rewardView.LastRewardTimeWeekly = null;
        _rewardView.CurrentActiveSlotWeekly = 0;
    }

    private void ClaimReward()
    {
        if (_rewardReceived)
            return;
        var reward = _rewardView.WeeklyRewards[_rewardView.CurrentActiveSlotWeekly];
        switch (reward.Type)
        {
            case RewardType.None:
                break;
            case RewardType.Wood:
                CurrencyWindow.Instance.AddWood(reward.Count);
                break;
            case RewardType.Diamond:
                CurrencyWindow.Instance.AddDiamond(reward.Count);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _rewardView.LastRewardTimeWeekly = DateTime.UtcNow;
        _rewardView.CurrentActiveSlotWeekly = (_rewardView.CurrentActiveSlotWeekly + 1) % _rewardView.WeeklyRewards.Count;
        RefreshRewardState();
    }
}



