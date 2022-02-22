using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField]
    private RewardView _rewardView;

    private DailyRewardController _controller;
    private WeeklyRewardController _weeklyRewardController;

    void Start()
    {
        _controller = new DailyRewardController(_rewardView);
        _weeklyRewardController = new WeeklyRewardController(_rewardView);
    }
}
