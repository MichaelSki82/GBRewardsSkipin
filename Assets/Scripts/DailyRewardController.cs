using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardController
{
    private readonly RewardView _rewardView;
    private List<SlotRewardView> _slots;

    private bool _rewardReceived = false;

    public DailyRewardController(RewardView rewardView)
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
        if (_rewardView.LastRewardTimeDaily.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _rewardView.LastRewardTimeDaily.Value;
            if (timeSpan.Seconds > _rewardView.TimeDeadlineDaily)
            {
                _rewardView.LastRewardTimeDaily = null;
                _rewardView.CurrentActiveSlotDaily = 0;
            }
            else if(timeSpan.Seconds < _rewardView.TimeCooldownDaily)
            {
                _rewardReceived = true;
            }
        }
    }

    private void RefreshUi()
    {
        _rewardView.GetRewardButtonDaily.interactable = !_rewardReceived;

        for (var i = 0; i < _rewardView.DailyRewards.Count; i++)
        {
            _slots[i].SetData(_rewardView.DailyRewards[i], i+1, i <= _rewardView.CurrentActiveSlotDaily);
        }

        DateTime nextDailyBonusTime =
            !_rewardView.LastRewardTimeDaily.HasValue
                ? DateTime.MinValue
                : _rewardView.LastRewardTimeDaily.Value.AddSeconds(_rewardView.TimeCooldownDaily);
        var delta = nextDailyBonusTime - DateTime.UtcNow;
        if (delta.TotalSeconds < 0)
            delta = new TimeSpan(0);

        _rewardView.RewardTimerDaily.text = delta.ToString();

        _rewardView.TimeLineDaily.fillAmount = (_rewardView.TimeCooldownDaily - (float)delta.TotalSeconds)/_rewardView.TimeCooldownDaily;
    }

    private void InitSlots()
    {
        _slots = new List<SlotRewardView>();
        for (int i = 0; i < _rewardView.DailyRewards.Count; i++)
        {
            var reward = _rewardView.DailyRewards[i];
            var slotInstance = GameObject.Instantiate(_rewardView.SlotPrefab, _rewardView.SlotsParent, false);
            slotInstance.SetData(reward, i+1, false);
            _slots.Add(slotInstance);
        }
    }

    private void SubscribeButtons()
    {
        _rewardView.GetRewardButtonDaily.onClick.AddListener(ClaimReward);
        _rewardView.ResetButton.onClick.AddListener(ResetReward);
    }

    private void ResetReward()
    {
        _rewardView.LastRewardTimeDaily = null;
        _rewardView.CurrentActiveSlotDaily = 0;
    }

    private void ClaimReward()
    {
        if (_rewardReceived)
            return;
        var reward = _rewardView.DailyRewards[_rewardView.CurrentActiveSlotDaily];
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

        _rewardView.LastRewardTimeDaily = DateTime.UtcNow;
        _rewardView.CurrentActiveSlotDaily = (_rewardView.CurrentActiveSlotDaily + 1) % _rewardView.DailyRewards.Count;
        RefreshRewardState();
    }
}
