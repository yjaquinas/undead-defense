using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CRankRowPanel : MonoBehaviour
{
    [SerializeField] private Text _rankText;
    [SerializeField] private Text _nicknameText;
    [SerializeField] private Text _bestPointText;

    // 순위 표시용 패널 셋팅
    public void SetRankInfo(string rank, string nickname, string bestPoint)
    {
        _rankText.text = rank;
        _nicknameText.text = nickname;
        _bestPointText.text = bestPoint;
    }
}
