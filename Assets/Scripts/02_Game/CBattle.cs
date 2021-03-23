/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CBattle.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/11/26
 * BGM
 * 초반 웨이브 heavy, the duel
 * 중반 웨이브 chase
 * 파이널 웨이브 (밤) Epic
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/15
 * 
 * 이름변경: CBattleScene.cs -> CBattle.cs
 * 
 * 전투의 상태 기계 BattleFSM
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/10
 * 
 * 전체적인 설계 재정비
 *  이름변경: CBattleSceneManager.cs -> CBattleScene.cs
 *  
 *  전투의 웨이브 관리
 *      총 웨이브 갯수
 *      현재 카운트
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/04
 *  크로스보우 합침
 *  
 *  게임클리어조건
 *      _enemyCount가 0일시
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/02
 *  레벨디자인 합침
 *  IDLE시 카메라 각도 위에서 지도 전체 보여주기 -> 방어선 설정시 better ux
 *      (카메라의 local position: 13, 70, 26; local rotation: 90, 0, 292)
 *  UI관련은 CBattleSceneUIManager.cs로 이동
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  2018/09/25
 *  
 *  xxxxxxxxxxxxxxxxxxxx 수정됨 2018/10/02 xxxxxxxxxx
 *  IDLE시 카메라 각도 위에서 지도 전체 보여주기 -> 방어선 설정시 better ux
 *      (카메라의 local position: 0, 70, 40; local rotation: 90, 0, 0) --------------------- projection도 orthographics으로?
 *  xxxxxxxxxx 수정됨 2018/10/02 xxxxxxxxxxxxxxxxxxxx
 *  전투 시작시 (StageStart()),
 *      카메라를 플레이어의 뒤로 위치해주기
 *          (카메라의 local position: 0, 0.5, 1; local rotation: 0, 0, 0)
 *      화면에 crosshair보여주기
 *  
 *  KilledEnemy()에서 에너미 destroy
 *  ***** 추후에 에너미 생성은 EnemySpawn 게임오브젝트 생성후에에 위임해주기....
 *      (나 왜 또 이걸 여기에다가 만들었니 ㅠ)
 *  
 *  ***** 추후에 현재 게임의 상태를 다른 class/object에서 확인할 수 있는 FSM설정
 *      IDLE: 전투 시작 전 -> 유저의 타워배치
 *      BATTLE: 전투 중 -> 유저의 조준 컨트롤
 *      LOST: 게임 오버 -> x
 *      WIN: 게임 클리어 -> x 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/09/24
 *  에너미가 화살에 맞았을때, CrossbowController가 KilledEnemy([enemy gameobject])를
 *   호출함
 *  모든 웨이브를 성공적으로 방어했을시 승리
 *  에너미가 플레이어 위치에 도달했을시 실패
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/09/23
 *  에너미 웨이브 생성
 *      게임 시작 후 FIRST_ENEMY_DELAY(초) 대기
 *      매 ENEMY_SPAWN_RATE(초) 마다 에너미 생성
 *      웨이브 마다 ENEMY_PER_WAVE(수)의 에너미 생성
 *  총 TOTAL_WAVE_COUNT(수)의 에너미 웨이브
 *  웨이브 사이 WAVE_DELAY(초)의 대기시간
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/09/22
 * 
 *  Battle 씬의 BattleManager에 추가된 컴포넌트형 스크립트
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CBattle : MonoBehaviour
{
    public enum BattleFSM
    {
        READY,
        BATTLE,
        PAUSE,
        GAMEOVER,
        CLEAR
    }
    public BattleFSM _FSM;


    private const float SPAWN_RATE = 3f;
    private float _waveDelayCounter;

    private const int TOTAL_ENEMIES_PASSED_UNTIL_GAMEOVER = 10;     // 게임오버 조건의 놓친 에너미 수
    public int GAMEOVER_CONTINUE_COUNT = 5;

    [SerializeField] private CBattleUI _battleUI;
    [SerializeField] private Text _enemyCountText;
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _BGMAS;
    [SerializeField] private GameObject _crossbow;
    [SerializeField] private GameObject _crosshair;

    public CEnemySpawn _enemySpawn;

    public int _gamePoint;                   // 플레이어의 게임포인트
    public int _enemyCount;                 // enemyspawn에서 증가, doorcheck에서 감소, enemyparams에서 감소

    [SerializeField] private int _numEnemiesPassed = 0;
    public int _waveNumber;
    public int _enemyLevel;

    [SerializeField] private Text _ContinueCountText;
    [SerializeField] private Text _waveWarningText;
    [SerializeField] private Material _daySkyBoxMaterial;
    [SerializeField] private Material _nightSkyBoxMaterial;
    [SerializeField] private Light[] _directionalLights;
    [SerializeField] private GameObject _nightCreek;

    private void Awake()
    {
        // 전투씬 입장시 (전투 시작시) 초기화 되야 할 변수들
        _gamePoint = 0;                 // 플레이어의 게임포인트 초기화

        _FSM = BattleFSM.READY;         // 전투 상태머신 초기화: 준비

    }

    private void Start()
    {
        _enemyCount = 0;
        _waveNumber = 1;
        _battleUI.UpdateWaveNumber(_waveNumber);
        _enemyLevel = 1;
        InvokeRepeating("TimerScoreUp", 11, 1);
        InitWave();
        _battleUI.enabled = true;

        _waveWarningText.enabled = true;
        _waveWarningText.text = ".....PREPARE YOURSELF";

        Invoke("EnterBattle", 5f);  // 배틀 전엔 유저 인풋 무시
    }


    private void Update()
    {
        _enemyCountText.text = _enemyCount.ToString();
    }

    public void ContinueAfterAds()
    {

        _battleUI.UpdateWaveNumber(_waveNumber);
        InitWave();
        _battleUI.enabled = true;

        _waveWarningText.enabled = true;
        _waveWarningText.text = ".....PREPARE YOURSELF";

        Invoke("EnterBattle", 5f);  // 배틀 전엔 유저 인풋 무시
    }


    private void EnterBattle()
    {
        _numEnemiesPassed = 0;
        _FSM = BattleFSM.BATTLE;
        _crossbow.SetActive(true);
        _crosshair.SetActive(true);
        _battleUI.ShowUIButtons();
    }

    void TimerScoreUp()
    {
        _gamePoint = _gamePoint + 10;
        CBattleUI.UpdateGamePointUI(_gamePoint.ToString());
    }

    public void GameOver()
    {
        _FSM = BattleFSM.GAMEOVER;
        CancelInvoke();
        _battleUI.ShowBattleOver("lost");
        Invoke("GoToEndScene", GAMEOVER_CONTINUE_COUNT);

        //_ContinueCountText.text = GAMEOVER_CONTINUE_COUNT.ToString();
        //InvokeRepeating("ContinueCountDown", 1f, 1f);
    }

    public void ContinueCountDown()
    {
        _ContinueCountText.text = (int.Parse(_ContinueCountText.text) - 1).ToString();
    }

    private void GoToEndScene()
    {
        PlayerPrefs.SetString("GAME_SCORE", _gamePoint.ToString());
        PlayerPrefs.Save();
        SceneManager.LoadScene("03_End");
    }

    // 보상 씬으로 이동
    private void EnterBattleWinGachaScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);  // 빌드 인덱스상의 다음 씬으로 이동
    }

    public void InitWave()
    {
        _enemySpawn.StartWave();
        InvokeRepeating("UpdateWaveWarning", 0f, 1f);
        _waveDelayCounter = _enemySpawn.INITIAL_DELAY;
        _BGMAS.clip = _audioClips[0];
        _BGMAS.Play();
        for(int i = 0; i < _directionalLights.Length; i++)
        {
            _directionalLights[i].intensity = 0.5f;
        }
    }

    // 에너미 사망
    // 에너미 카운트 감소
    // CEnemyParams.cs -> CCharacterParams.cs가 호출
    public void EnemyKilled(int point)
    {
        _gamePoint += point;
        CBattleUI.UpdateGamePointUI(_gamePoint.ToString());
    }

    public Vector3 EnemyPassedDoor()
    {
        _numEnemiesPassed++;
        if(_numEnemiesPassed >= TOTAL_ENEMIES_PASSED_UNTIL_GAMEOVER)
        {
            _FSM = BattleFSM.GAMEOVER;
            GameOver();
            return Vector3.zero;
        }

        return new Vector3((float)(TOTAL_ENEMIES_PASSED_UNTIL_GAMEOVER - _numEnemiesPassed) / TOTAL_ENEMIES_PASSED_UNTIL_GAMEOVER, 1f, 1f);
    }

    public void IncrememtWaveNumber()
    {
        // 웨이브 _5, _9는 특별하게 조명 바꾸고 하늘 바꾸고 문자도 빠바빨간색 궁구메허니
        switch(_enemySpawn._waveNumber % 10)
        {
            case 1:
                RenderSettings.skybox = _daySkyBoxMaterial;
                _nightCreek.SetActive(false);
                break;
            case 5:
                RenderSettings.skybox = _nightSkyBoxMaterial;
                for(int i = 0; i < _directionalLights.Length; i++)
                {
                    _directionalLights[i].intensity = 0.3f;
                }
                _BGMAS.clip = _audioClips[1];
                _BGMAS.Play();

                InvokeRepeating("UpdateWaveWarning", 0f, 1f);
                _waveDelayCounter = _enemySpawn.DELAY_BETWEEN_WAVES;
                break;
            case 9:
                _nightCreek.SetActive(true);
                for(int i = 0; i < _directionalLights.Length; i++)
                {
                    _directionalLights[i].intensity = 0.1f;
                }
                _BGMAS.clip = _audioClips[2];
                _BGMAS.Play();
                break;
        }
        _waveNumber++;
        _battleUI.UpdateWaveNumber(_waveNumber);
    }

    // 새 웨이브 시작전 카운트다운
    private void UpdateWaveWarning()
    {
        // 카운트는 최대 5부터 활성화
        if(_waveDelayCounter < 6)
        {
            _waveWarningText.enabled = true;
            // 카운트 다운 끝 - 인보크 끝. 카운트 비활성화
            if(_waveDelayCounter == 0)
            {
                _waveWarningText.enabled = false;
                CancelInvoke("UpdateWaveWarning");
                return;
            }

            if(_waveNumber > 8)
            {
                _waveWarningText.color = Color.red;
                if(_waveDelayCounter > 1) _waveWarningText.text = "DEADLY WAVE COMING\nIN " + _waveDelayCounter + " SECONDS";
                else _waveWarningText.text = "DEADLY WAVE COMING\nIN " + _waveDelayCounter + " SECOND";
                _enemySpawn._spawnRate = SPAWN_RATE * 0.5f;
            }
            else
            {
                _waveWarningText.color = Color.white;
                // 게임의 젤 처음 웨이브라면
                if(_waveNumber == 1 && _enemyLevel == 1)
                {
                    if(_waveDelayCounter > 1) _waveWarningText.text = "THE UNDEAD ARMY IS\nCOMING IN " + _waveDelayCounter + " SECONDS";
                    else _waveWarningText.text = "THE UNDEAD ARMY IS\nCOMING IN " + _waveDelayCounter + " SECOND";
                }
                else
                {
                    if(_waveDelayCounter > 1) _waveWarningText.text = "NEXT WAVE COMING\nIN " + _waveDelayCounter + " SECONDS";
                    else _waveWarningText.text = "NEXT WAVE COMING\nIN " + _waveDelayCounter + " SECOND";
                }
                _enemySpawn._spawnRate = SPAWN_RATE;
            }
        }
        _waveDelayCounter--;

    }
}
