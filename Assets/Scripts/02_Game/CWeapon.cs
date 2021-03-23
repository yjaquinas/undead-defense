/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CWeapon.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/29
 * 용병 선택 추가
 * 레이캐스트가 레이 선상의 모든 콜라이더 체크
 *  에너미가 용병/스킬의 콜라이더 뒤에 있을시 용병 쏠 수 있게
 * 레이캐스트가 에너미와 충동했을시, 거기까지만
 *  (여러마리 에너미가 줄줄이 맞으면 안돼니까)
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/24
 * 
 * 멧돼지 머리쏘면 멧돼지 죽이기
 * 멧돼지 몸 쏘면 그냥 화살만 박히게
 * 에너미 머리쏘면 더블점수!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/23
 *  화살 쏜곳에 화살 박기
 *  화살 스폰 포인트를 Vector3 -> Transform
 *      Vector3는 Struct라서, 참조가 아님, 처음 초기화된 위치만 사용되서 안좋음
 * 머리 쏘면 머리 뜯어내기 - 렌더러가 통짜여서 불가능합니다!!!
 *  머리 파괴시키고, 바닥에 새로운 머리 떨궈주기
 * 머리 쏘면 원샷 킬!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/16
 * 이름 변경
 *  CCrossbowController -> CWeapon
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/16
 *  크로스보우 슛 오디오 추가
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/09/24
 * 
 *  Battle 씬의 Crossbow에 추가된 컴포넌트형 스크립트
 * 
 *  화면 가운데의 조준점에 에너미가 들어오면, 자동 슛
 //*  화살 궤도를 그려주고, BattleSceneManager의 Killed() 호출
 *  피격당한 에너미에게 데미지
 *      CEnemyParams.GetHit()
 *  공격속도는 1초에 한번으로 일단
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;

public class CWeapon : MonoBehaviour
{
    private const int BASIC_ARROW_DAMAGE = 5;       // 기본 화살 데미지
    private const float ARROW_DURATION = 0.1f;      // 화살 공격 라인 표시 시간

    private float _arrowDurationTimer;              // 화살 공격 라인 표시 타이머
    private bool _isCrossbowReady;                  // 화살 장전 됨? 공격시 애니메이션 시작에서 false로, 애니메이션 끝에서 true
    [SerializeField] private float timeBetweenArrows; // 화살 주기 (인스펙터에서 세팅하세요)
    private float timer; //화살 주기 체크


    private int _damage;                   // 데미지
    private const float _attSpeed = 1;
    private GameObject _basicArrowStuckPrefab;

    private CBattle _battleManager;

    [SerializeField] private AudioSource _crossbowReleaseAS;
    [SerializeField] private AudioSource _crossbowReleaseAS2;
    [SerializeField] private AudioSource _crossbowReloadAS;

    private Transform _arrowSpawnPoint;

    private bool _isLoaded;


    [SerializeField] private int mercType;
    [SerializeField] private Animator _shotAni; //반동 애니메이터
    [SerializeField] private GameObject _arrow; //화살 프리팹
    [SerializeField] private GameObject _player;
    [SerializeField] private Button ShotButton;

    private void Start()
    {
        _battleManager = GameObject.Find("BattleManager").GetComponent<CBattle>();
        _basicArrowStuckPrefab = Resources.Load("Prefabs/BasicArrowStuck") as GameObject;    // 일반 화살 프리팹 참조

        _arrowSpawnPoint = transform.Find("ArrowSpawnPoint");
        _arrowDurationTimer = 0;                            // 화살 궤도 지속시간
        _isCrossbowReady = true;                                      // 공격 속도 타이머
        _isLoaded = true;
        _damage = BASIC_ARROW_DAMAGE;
    }


    private void Update()
    {
        timer += Time.deltaTime;

        // 전투중에만 공격가능
        if(_battleManager._FSM != CBattle.BattleFSM.BATTLE)
        {
            return;
        }

        // 화살 궤도를 ARROW_DURATION초간만 보이고 숨기기
        _arrowDurationTimer += Time.deltaTime;
        if(_arrowDurationTimer >= ARROW_DURATION)
        {
            _arrowDurationTimer = 0;                              // 화살 궤도 지속시간 초기화
        }
    }

    //빵 버튼 누르면 실행
    public void Fire()
    {
        if (timer > timeBetweenArrows)
        {
            // 전투중에만 공격가능
            if (_battleManager._FSM != CBattle.BattleFSM.BATTLE)
            {
                return;
            }
            ShootArrow();
            int randnum = Random.Range(0, 5);
            switch (randnum)
            {
                case 0:
                    _shotAni.SetTrigger("Shot1");
                    break;
                case 1:
                    _shotAni.SetTrigger("Shot2");
                    break;
                case 2:
                    _shotAni.SetTrigger("Shot3");
                    break;
                case 3:
                    _shotAni.SetTrigger("Shot4");
                    break;
                case 4:
                    _shotAni.SetTrigger("Shot5");
                    break;
            }
            timer = 0;
        }
    }

    // 화면 중앙이 조준하고있는 방향으로 슛!!!
    // 화살 생성 -> CArrow.cs
    // 화살 슛 사운드 플레이
    // 딜레이 리셋
    public void ShootArrow()
    {
        _crossbowReleaseAS.Play();
        _crossbowReleaseAS2.Play();
        Invoke("PlayReloadAS", 0.5f);
        _arrowDurationTimer = 0;                                                    // 화살 궤도 지속시간 초기화
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));  // 화면 가운데에서 부터의 ray

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for(int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            string colliderTag = hit.collider.gameObject.tag;

            // 화살이 박히면 안되는 거라면 스킵
            if(colliderTag == "Mercenary" || colliderTag == "EnemyBuf" || colliderTag == "MercSkill" || colliderTag == "Player" || colliderTag == "EnemyProjectile")
            {
                continue;
            }
            // 날아가는 화살 방향
            Vector3 enemyTransform = hit.point - transform.position;
            Quaternion arrowLookRotation = Quaternion.LookRotation(enemyTransform);
            Instantiate(_arrow, _arrowSpawnPoint.position, arrowLookRotation);
            break;      // 화살은 하나만 날아가게
        }
    }
    private void PlayReloadAS()
    {
        _crossbowReloadAS.Play();
    }

    // 용병 타입 변경
    public void ChangeMercType()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));  // 화면 가운데에서 부터의 ray
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for(int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            string colliderTag = hit.collider.gameObject.tag;

            // 용병 아니면 무시
            if(colliderTag != "Mercenary")
            {
                continue;
            }

            if(hit.collider.gameObject.name == "ICE")
            {
                mercType = 0;
                return;
            }
            else if(hit.collider.gameObject.name == "LIGHTNING")
            {
                mercType = 1;
                return;
            }
            else if(hit.collider.gameObject.name == "TORNADO")
            {
                mercType = 2;
                return;
            }
            else if(hit.collider.gameObject.name == "GAS")
            {
                mercType = 3;
                return;
            }

            // 반대로 타워에 맞췄을때
            if(hit.collider.gameObject.CompareTag("Mercenary")) // 타워랑 위에 용병 전부 태그 똑같이 맞춤.
            {
                hit.collider.gameObject.transform.Find("Mercenary").GetComponent<CMercParams>().setType(mercType);
            }
        }
    }

    // 장전 됐는지 체크
    // 현재 _isLoaded는 수정되지 않아서 상시 true
    public bool IsLoaded()
    {
        return _isLoaded;
    }
}
