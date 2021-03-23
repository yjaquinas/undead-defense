/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemySpawn.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/11/01
 * 스테이지에 따라 에너미 종류 증가
 *  웨이브1: 스켈레톤만
 *  웨이브2: + 헤비
 *  웨이브3: + 킹
 *  웨이브4: + 로그
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/18
 * 에너미 종류 증가
 *  SkeletonEnemy ->
 *      UndeadHeavy
 *      UndeadKing
 *      UndeadRogue
 *      UndeadSkeleton
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/17
 * 
 * 동적 참조로 변환
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CEnemySpawn : MonoBehaviour
{
    public float INITIAL_DELAY = 10f;
    public float DELAY_BETWEEN_WAVES = 3f;
    private const float DELAY_BETWEEN_LEVELS = 20f;
    private const float SPAWN_RATE = 3f;

    [SerializeField] private Transform[] _spawnPointTransforms;     // 생성 위치 참조 배열 유니티에서 참조
    [SerializeField] private Animator[] _spawnPointIconAnimators;
    [SerializeField] private CBattle _battleManager;
    private GameObject _spawnVFXPrefab;         // 생성 이펙트 프리펩
    private GameObject[] _enemyPrefabs;          //생성할 에너미 프리팹 배열

    [SerializeField] private AudioSource _as;
    [SerializeField] private AudioClip[] _ac;


    // 패턴
    [HideInInspector]
    public int pattern0, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8, pattern9;
    private string[] _wavePatterns =
    {
        "000000",
        "000000",
        "010101",
        "010101111",
        "2222000",
        "010101",
        "01010202",
        "11111212",
        "222222",
        "2222222222"
    };

    public int _waveNumber;
    private int _enemyIndesInThisWave;
    private float _waveDelayCounter;
    [SerializeField] private string _currPattern;
    public float _spawnRate;  // 몇초에 한번씩 스폰하는가

    private bool _isWaveSpawnDone = false;
    private bool _isCurrLevelDone = false;

    private void Awake()
    {
        _spawnVFXPrefab = Resources.Load("Prefabs/SpawnEffect") as GameObject;
        _enemyPrefabs = new GameObject[4];
        _enemyPrefabs[0] = Resources.Load("Prefabs/UndeadSkeleton") as GameObject;
        _enemyPrefabs[1] = Resources.Load("Prefabs/UndeadHeavy") as GameObject;
        _enemyPrefabs[2] = Resources.Load("Prefabs/UndeadRogue") as GameObject;
        //_enemyPrefabs[3] = Resources.Load("Prefabs/UndeadKing") as GameObject;
        _enemyIndesInThisWave = 0;
        _spawnRate = SPAWN_RATE;
        _isWaveSpawnDone = false;
        _isCurrLevelDone = false;
        _as.clip = _ac[0];
    }

    private void Update()
    {
        // 웨이브가 끝났고, 해당 웨이브의 에너미가 모두 죽음
        if(_isWaveSpawnDone && (_battleManager._enemyCount == 0))
        {
            if(_isCurrLevelDone)
            {
                //Invoke("StartWave", DELAY_BETWEEN_LEVELS);
                _battleManager.InitWave();
                _isCurrLevelDone = false;
            }
            else
            {
                Invoke("SpawnEnemy", DELAY_BETWEEN_WAVES);
            }
            _isWaveSpawnDone = false;
            _battleManager.IncrememtWaveNumber();       // 매 웨이브의 첫 에너미 마다 배틀메니저와 UI의 웨이브 번호 업데이트
        }
    }

    // 에너미 웨이브 스타트 메소드
    // CBattle의 InitWave()에서 호출
    public void StartWave()
    {
        _waveNumber = 0;
        _currPattern = _wavePatterns[_waveNumber++];
        _enemyIndesInThisWave = 0;
        Invoke("SpawnEnemy", INITIAL_DELAY);

    }
    // 웨이브
    // 패턴끝날때까지 다시 인보크
    // 패턴 끝났을시, 다음 웨이브 세팅 후 인보크
    public void SpawnEnemy()
    {
        // 에너미 생성 위치
        int spawnPointIndex = Random.Range(0, 4);
        Vector3 spawnPoint = _spawnPointTransforms[spawnPointIndex].position;
        _spawnPointIconAnimators[spawnPointIndex].SetTrigger("Flash");
        //// 에너미생성이펙트
        //GameObject effect = Instantiate(_spawnVFXPrefab, spawnPoint, Quaternion.identity);
        //Destroy(effect, 2f);
        // 에너미 생성
        int enemyIndex = _currPattern[_enemyIndesInThisWave++] - '0';        // char에서 int로 변환
        GameObject newEnemy = Instantiate(_enemyPrefabs[enemyIndex], spawnPoint, Quaternion.identity);
        if(enemyIndex == 2)     // 멧돼지
        {
            _as.clip = _ac[1];
        }
        _as.Play();
        _battleManager._enemyCount++;       // 에너미 생성
        // 해당 웨이브의 다음 생성 에너미 업데이트
        if(_enemyIndesInThisWave < _currPattern.Length)
        {
            // 해당 웨이브의 다음 에너미 콜
            Invoke("SpawnEnemy", _spawnRate);
        }
        else
        {
            // 웨이브 끝
            if(_waveNumber < _wavePatterns.Length)
            {
                // 해당 레벨의 다음 웨이브 콜
                _currPattern = _wavePatterns[_waveNumber++];
                _enemyIndesInThisWave = 0;
                _isWaveSpawnDone = true;    // 이번 웨이브 끝 플래그
            }
            else
            {
                // 레벨 끝 (웨이브 그룹 10개 다 나옴)
                _isCurrLevelDone = true;
                _isWaveSpawnDone = true;    // 이번 웨이브 끝 플래그
                _battleManager._enemyLevel++;
            }
        }
    }

}
