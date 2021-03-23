/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CBattleUI.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 에너미 생싱 위치 아이콘 표시
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/15
 * 이름 변경
 *  CBattleSceneUIManager -> CBattleUI
 *  
 * TopViewCamera 제거됨. 
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/02
 * 
 *  Battle 씬의 BattleManager에 추가된 컴포넌트형 스크립트
 *  Canvas - UI 관련 담당
 *  
 *  UI내에선 Text사용 안하고 Text Mesh Pro만 사용하기
 *  
 *  유저의 조준점 crosshair도 UI에서 담당!
 *  
 *  TopViewCamera -> PlayerCamera
 *  
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;

public class CBattleUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawnPointIndicatorIcons;
    private static Text _gamePointText;             // 게임포인트 UI.Text 참조변수
    private static Text _youWinText;                // 승리 UI 참조변수
    [SerializeField] private GameObject _failPanel;
    [SerializeField] private Text _waveWarningText;
    [SerializeField] private Image _crossHairImage;

    private Button _startButton;                    // 스테이지 스타트 참조변수
    [SerializeField] private Button _shootButton;                    // 화살 슛 참조변수
    [SerializeField] private Button _scopeButton;                    // 스코프 줌 참조변수
    private Button _mercChangeButton;               // 용병 변경 참조변수

    private CBattle _battle;

    [SerializeField] private Text _timeText;
    private int _playTimeInSeconds = 0;
    [SerializeField] Text _waveNumberText;

    private void Start()
    {
        _battle = GetComponent<CBattle>();
        // 전투 관리차 참조

        _gamePointText = GameObject.Find("GamePointText").GetComponent<Text>();         // 게임포인트 UI 참조
        _gamePointText.text = _battle._gamePoint.ToString();                       // 게임포인트 UI 초기화

        //_topViewCamera = GameObject.Find("TopViewCamera").GetComponent<Camera>();
        InvokeRepeating("UpdateTime", 1f, 1f);
    }

    public void ShowUIButtons()
    {
        _shootButton.gameObject.SetActive(true);
        _scopeButton.gameObject.SetActive(true);
    }

    // 게임 오버 문구 표시
    // 매개변수 condition:
    //  "lost": 패배문구
    //  "win": 승리 문구
    public void ShowBattleOver(string condition)
    {
        switch(condition)
        {
            case "lost":
                _failPanel.SetActive(true);
                _shootButton.gameObject.SetActive(false);
                _scopeButton.gameObject.SetActive(false);
                _waveWarningText.gameObject.SetActive(false);
                _crossHairImage.gameObject.SetActive(false);

                break;
            case "continue":

                _failPanel.SetActive(false);
                _shootButton.gameObject.SetActive(true);
                _scopeButton.gameObject.SetActive(true);
                _waveWarningText.gameObject.SetActive(true);
                _crossHairImage.gameObject.SetActive(true);
                break;
            case "win":
                _youWinText.GetComponent<Text>().enabled = true;        // 승리 문구 보이기
                break;

        }
    }

    // 게임포인트 UI 업데이트
    // 매개변수 gamePoint:
    //  게임보인트 UI에 표시할 점수 string
    public static void UpdateGamePointUI(string gamePoint)
    {
        _gamePointText.text = gamePoint;
    }

    private void UpdateTime()
    {
        _playTimeInSeconds++;
        _timeText.text = (_playTimeInSeconds / 60).ToString("00") + ":" + (_playTimeInSeconds % 60).ToString("00");
    }

    public void UpdateWaveNumber(int waveNumber)
    {
        _waveNumberText.text = waveNumber.ToString();
    }

}
