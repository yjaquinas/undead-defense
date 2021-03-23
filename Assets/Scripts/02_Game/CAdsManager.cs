using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Advertisements;

public class CAdsManager : MonoBehaviour
{
    [SerializeField] private CBattle _battle;
    [SerializeField] private CBattleUI _battleUI;

    public string _androidGameId;       // 안드로이드 App ID

    public bool _testMode;
    public Button _adsButton;           // 광고 버튼
    public Text _msgText;

    private void Start()
    {
        _adsButton.interactable = false;    // 버튼을 비활성화 함
        // 유니티 광고 시스템이 제공되는 상태이며 아직 초기화가 되어 있지 않다면
        if(Advertisement.isSupported && !Advertisement.isInitialized)
        {
            // 광고 시스템을 초기화함
            Advertisement.Initialize(_androidGameId, _testMode);
        }
    }

    private void Update()
    {
        // 광고 시스템이 초기화가 완료되었고 광고를 볼 준비가 되었다면
        _adsButton.interactable = (Advertisement.isInitialized && Advertisement.IsReady(null));
    }

    // 광고 보기 완료 이벤트 메소드
    public void UnityAdsShowCallback(ShowResult result)
    {
        switch(result)
        {
            case ShowResult.Finished:   // 광고 보기 완료
                _msgText.text = "광고 시청을 완료함";
                _battleUI.ShowBattleOver("continue");
                _battle.ContinueAfterAds();
                break;
            case ShowResult.Skipped:    // 광고를 스킵함
                _msgText.text = "광고 시청을 중단함";
                break;
            case ShowResult.Failed:     // 광고 보기 실패
                _msgText.text = "광고 시청을 실패함";
                break;
        }
    }

    // 광고 보기 버튼 클릭
    public void OnShowAdsButtonClick()
    {
        // 광고 보기 옵션 설정
        ShowOptions option = new ShowOptions();
        // 광고 보기 완료 이벤트 메소드 연결
        option.resultCallback = UnityAdsShowCallback;

        // 광고 띄우기
        Advertisement.Show(option);
    }
}
