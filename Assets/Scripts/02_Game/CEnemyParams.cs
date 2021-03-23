/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemyParams.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/31
 * CEnemyMovement.cs에서 attackRange옮겨움
 * 방어력 수치 추가
 *  방어력 계산 이후, 최소 공격력은 1로
 * 버프로인한 방어력 증가, 해제
 * 화살에 맞았을시 애니메이션
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/30
 * 헤드샷
 *  이전: 플레이어가 헤드샷 쏘면 무조건 원샷킬이어서, 직접 DoublePoint()호출
 *      -> 문제: heavy는 한방이 아니여서, 포인트 더블이 썋임 2^n
 *          - 해결: HeadShot()호출로, 머리에 맞았는지 플래스 설정,
 *              에너미 사망시 플래그 체크로 포인트 더블
 * 상태이상
 *  이전: 에너미가 상태이상 FSM에 유지되지 않고, TRACE로 바로 넘어감
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/29
 *  Heavy는 화살 데미지 1, 번개에 원샷킬
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/24
 *  보상점수 더블만들기 DoublePoint()
 *  로그는 보상점수 2배
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/17
 *  피격시 상태이상 마법이라면 FSM변경 호출하기
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/16
 * 
 * 에너미의 속성, 피격, 사망 관리
 * 
 * 피격시 오디오 플레이
 * 
 * 상태이상 FSM 호출,
 * 해당 상태에 상응하는 parameter변경
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CEnemyParams : CCharParams
{
    private const int DEFENSE_UP_BUF = 5;
    private const float FROZEN_DECELERATION_RATE = 0.25f;
    private const int BASIC_DEFENSE = 1;

    // 참조는 유니티 inspector에서
    [SerializeField] private GameObject _frozenCC;
    [SerializeField] private GameObject _stunnedCC;

    private CEnemyMovement _enemyMovemet;

    [SerializeField] private float KING_ATTACK_RANGE = 15f;
    public float _attackRange = 3f;
    private int _defense;
    private bool _isDefenseUp;
    private int _rewardPoint;

    float _distToTornado;   // 스킬과 몬스터 사이 거리
    float angle = 0f;       //각도
    private float _heightBeforeTornado;           // 하늘로 뜨기전에 높이.

    private bool _isTornado;
    public bool IsTornado { get { return _isTornado; } }
    public Vector3 tornadoPos;
    private bool _isHeadShot;
    [SerializeField] private bool _isCC;         // 상태 이상 상태 플래그
    public bool IsCC { get { return _isCC; } }
    [SerializeField] private ParticleSystem _headShotPS;

    [SerializeField] private Text _damageText;

    protected override void Awake()
    {
        base.Awake();
        //name = "Skeleton";

        _enemyMovemet = GetComponent<CEnemyMovement>();

        _maxHp = 10;
        _curHp = _maxHp;
        _rewardPoint = 300;
        if(name.Contains("Rogue"))
        {
            _rewardPoint = 400;
        }
        _isHeadShot = false;
        _isCC = false;

        if(name.Contains("King"))       // 킹은 활 들고 있으니까 멀리서 쏜다
        {
            _attackRange = KING_ATTACK_RANGE;
        }
        if(name.Contains("Heavy"))       // 킹은 활 들고 있으니까 멀리서 쏜다
        {
            _rewardPoint = 500;
        }
        _defense = BASIC_DEFENSE;
        _isDefenseUp = false;
    }

    void Update()
    {
        // 토네이도에 걸렸으면 빙글빙글
        if(_isTornado)
        {
            //스킬맞을때 스킬과 몬스터 사이 거리 구함.
            angle += 5f;
            transform.position = tornadoPos + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) - Mathf.Sin(Mathf.Deg2Rad * angle),
                                transform.position.y + Time.deltaTime, Mathf.Sin(Mathf.Deg2Rad * angle) + Mathf.Cos(Mathf.Deg2Rad * angle));
            //angle += 5f;
            //transform.Rotate(Vector3.up * 360f * Time.deltaTime);
        }
    }

    // 피격
    public override void GetHit(string attType, int attackDamage)
    {
        // 화살이 아닐시 상태이상
        // 가스 상태이상은 따로
        if(attType != "Arrow" || attType != "Gas")
        {
            _isCC = true;
        }

        // 토네이도 안에 있으면 무시
        if(_FSM != CharFSM.TORNADO)
        {
            switch(attType)
            {
                case "Arrow":
                    // 헤비한테는 화살 데미지 약하게, 헤드샷일시엔 한방에!
                    if(name.Contains("Heavy") && !_isHeadShot)
                    {
                        attackDamage = 1;
                    }
                    _isCC = false;
                    break;
                case "Ice":
                    Freeze();
                    break;
                case "Lightning":
                    Stun();
                    break;
                case "Tornado":
                    Tornado(tornadoPos);
                    break;
                case "Gas":
                    _isCC = false;
                    break;
            }
        }

        // 방어력 계산 이후, 최소 공격력은 1로
        attackDamage -= _defense;

        if(attackDamage < 1)
        {
            attackDamage = 1;
        }

        // 데미지 표시
        // 0.5초 후 가리기
        //_damageText.text = attackDamage.ToString(); // 데미지 업데이트
        //_damageText.enabled = true;     // 데미지 보이기
        CancelInvoke("HideDamage");     // 이전에 데미지 표시할때 Invoke해둔 데미지 가리기가 있다면 취소
        Invoke("HideDamage", 0.5f);     // 새로 데미지 가리기 Invoke

        _animator.SetTrigger("GetHit");
        _curHp -= attackDamage;
        if(_curHp <= 0)
        {
            _curHp = 0;
            transform.Find("Billboard/HpFront").gameObject.SetActive(false);
            transform.Find("Billboard/HpBack").gameObject.SetActive(false);
            //_hpBar.transform.parent.gameObject.SetActive(false);
            SetFSM(CharFSM.DIE);
            _headCollider.enabled = false;      // 사망시 콜라이더 바로 비활성화!
            _bodyCollider.enabled = false;      // 사망시 콜라이더 바로 비활성화!
            Destroy(gameObject, 5f);            // 수초뒤 삭제
            _battleManager._enemyCount--;
            if(_isHeadShot)                     // 헤드샷이 있었으면, 점수 더블
            {
                HeadShotPoint();
            }
            _battleManager.EnemyKilled(_rewardPoint);        // 죽음 배틀 메니저에 알리기

            // 로그 죽이면 보어  콜라이더 없애기
            if(name.Contains("Rogue"))
            {
                if(transform.Find("Hips_jnt/Spine_jnt/Boar"))
                {
                    GameObject _boar = transform.Find("Hips_jnt/Spine_jnt/Boar").gameObject;
                    _boar.GetComponent<Collider>().enabled = false;
                    _boar.transform.parent = null;          // 에너미에서 분리
                    Destroy(_boar, 3f);                                     // 3초 후에 없애기
                }
            }
        }
        _hpBar.rectTransform.localScale = new Vector3((float)_curHp / _maxHp, 1f, 1f);
    }

    private void HideDamage()
    {
        _damageText.enabled = false;
    }

    // 보상점수 2배로 올리기
    // CWeapon.cs에서 머리쏘면 점수 더블!
    private void HeadShotPoint()
    {
        _rewardPoint *= 2;
    }

    // 헤드샷 됨
    public void HeadShot()
    {
        _isHeadShot = true;
        _damageText.text = "HEADSHOT!";
        _damageText.enabled = true;
        _damageText.color = Color.red;
    }

    // 얼음 공격을 받았을시 호출됨
    // 얼음 상태이상 업데이트
    // 상태이상 그래픽 이펙트 활성화
    // 이동속도, 이동 애니메이션 속도 줄이기
    // FROZEN_DURATION초 이후에 상태이상 해제 호출
    public void Freeze()
    {
        SetFSM(CharFSM.FROZEN);
        _frozenCC.SetActive(true);
        _animator.speed *= FROZEN_DECELERATION_RATE;
        GetComponent<NavMeshAgent>().speed *= FROZEN_DECELERATION_RATE;
        Invoke("CCFinish", CMercParams.ICE_DURATION);
    }

    public void Stun()
    {
        SetFSM(CharFSM.STUNNED);
        _stunnedCC.SetActive(true);
        //_animator.speed = 0;
        //GetComponent<NavMeshAgent>().speed = 0;
        GetComponent<NavMeshAgent>().isStopped = true;
        Invoke("CCFinish", CMercParams.LIGHTNING_DURATION);
    }

    public void Tornado(Vector3 pos)
    {
        SetFSM(CharFSM.TORNADO);
        _stunnedCC.SetActive(true);
        _animator.speed = 0;
        GetComponent<NavMeshAgent>().speed = 0;
        //Invoke("CCFinish", CMercParams.TORNADO_DURATION);
        _isTornado = true;
        tornadoPos = pos;

        _distToTornado = Vector3.Distance(new Vector3(tornadoPos.x, 0, tornadoPos.z),
                    new Vector3(transform.position.x, 0, transform.position.z));
        _heightBeforeTornado = transform.position.y;
    }

    // 상태이상 해제
    // 상태이상 그래픽 이펙트 비활성화
    // 이동속도, 애니메이션 속도 되돌리기
    public void CCFinish() //추가함
    {
        if(_isTornado)
        {
            transform.position = new Vector3(transform.position.x, _heightBeforeTornado, transform.position.z);
            _isTornado = false;
        }

        _frozenCC.SetActive(false);
        _stunnedCC.SetActive(false);
        _isCC = false;      // 상태이상 종료
        _animator.speed = 1;
        if(GetFSM() != CharFSM.DIE)
        {
            SetFSM(CharFSM.IDLE);
            GetComponent<NavMeshAgent>().isStopped = false;
            GetComponent<NavMeshAgent>().speed = _enemyMovemet._origSpeed;
        }
    }

    // CEnemyCollisionCheck에서 호출
    // 스피드 업 버프를 받지 않았다면, 2배 빨르게 버프 온
    public void DefenseUp()
    {
        if(!_isDefenseUp)
        {
            _defense += DEFENSE_UP_BUF;
            _isDefenseUp = false;
        }
    }

    // CEnemyCollisionCheck에서 호출
    // 스피드 업 버프 해제
    public void DefenseUpEnd()
    {
        _defense = BASIC_DEFENSE;
        _isDefenseUp = true; ;
    }

    public void PlayHeadShotEffect()
    {
        _headShotPS.Play();
    }

    public void FallFromRide()
    {
        Stun();
    }
}
