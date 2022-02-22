using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotRewardWeeklyView : MonoBehaviour
{
    [SerializeField]
    private Image _rewardIcon;
    [SerializeField]
    private Image _highlightImage;
    [SerializeField]
    private TMP_Text _textWeek;
    [SerializeField]
    private TMP_Text _countText;

    public void SetData(Reward reward, int weekNum, bool isSelect)
    {
        _rewardIcon.sprite = reward.Icon;
        _textWeek.text = $"Week {weekNum}";
        _countText.text = reward.Count.ToString();
        _highlightImage.gameObject.SetActive(isSelect);
    }
}
