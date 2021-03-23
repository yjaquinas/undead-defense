/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemyKill.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/31
 * Dance관련 CEnemyMovement에서 가져옴
 * King이 Dance하는 동안 sphere collider 켜서, sphere내에 있는 에너미들
 *  둘중 하나
 *  1. 공격속도 업
 *  2. 방어력 업
 *  
 * Skeleton은 플레이어에게 호박을 던집니다
 *  호박던지는 애니메이션 동안 state를 호박으로!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.AI;

public class CEnemySkill : MonoBehaviour
{
    public enum BUF_TYPE
    {
        SPEED_UP,
        DEFENSE_UP
    }
    public BUF_TYPE _bufType;

    private const float DANCE_DURATION = 5f;
    private const float DANCE_RATE_MIN = 5;
    private const float DANCE_RATE_MAX = 20;

    private const float PUMPKIN_DURATION = 1.3f;
    private const float PUMPKIN_RATE_MIN = 30;
    private const float PUMPKIN_RATE_MAX = 100;

    private CEnemyParams _params;
    private NavMeshAgent _navMeshAgent;
    private SphereCollider _collider;
    private Animator _animator;

    [SerializeField] GameObject _pumpkin;


    private void Awake()
    {
        _params = GetComponent<CEnemyParams>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if(name.Contains("King"))
        {
            _collider = transform.Find("KingBufArea").GetComponent<SphereCollider>();
            Invoke("DanceStart", Random.Range(DANCE_RATE_MIN, DANCE_RATE_MAX));
        }
        else if(name.Contains("Skeleton"))
        {
            //Invoke("PumpkinThrowStart", Random.Range(PUMPKIN_RATE_MIN, PUMPKIN_RATE_MAX));
        }
    }

    //----------------------------------------
    // 킹 스킬
    //댄스애니메이션재생, 네브메쉬정지, 인보크시간
    private void DanceStart()
    {
        _bufType = (BUF_TYPE)Random.Range(0, 2);

        _params.SetFSM(CharFSM.DANCE);
        transform.LookAt(Camera.main.transform.position);
        _navMeshAgent.enabled = false;
        _collider.enabled = true;
        Invoke("DanceQuit", DANCE_DURATION);
    }
    //인보크시간 후에 Idle=>Trace자동 변경
    private void DanceQuit()
    {
        _navMeshAgent.enabled = true;
        _collider.enabled = false;
        _params.SetFSM(CharFSM.IDLE);
        Invoke("DanceStart", Random.Range(DANCE_RATE_MIN, DANCE_RATE_MAX));
    }

    //----------------------------------------
    // 스켈레톤 스킬
    private void PumpkinThrowStart()
    {
        _params.SetFSM(CharFSM.PUMPKIN);
        transform.LookAt(Camera.main.transform.position);
        _navMeshAgent.enabled = false;
        Invoke("PumpkinThrowQuit", PUMPKIN_DURATION);
    }
    // 호박을 생성! - 스켈레톤의 ThrowGrenade 애니메이션에서 이벤트로 호출
    private void ThrowPumpkin()
    {
        //_pumpkin.transform.position;
        GameObject pumpkin = Instantiate(_pumpkin, _pumpkin.transform.position, Quaternion.identity);
        pumpkin.GetComponent<CEnemyProjectile>().Shoot();
    }
    private void PumpkinThrowQuit()
    {
        _navMeshAgent.enabled = true;
        _params.SetFSM(CharFSM.IDLE);
    }

}
