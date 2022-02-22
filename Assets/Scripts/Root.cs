﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField]
    private RewardView _rewardView;

    private DailyRewardController _controller;

    void Start()
    {
        _controller = new DailyRewardController(_rewardView);
    }
}
