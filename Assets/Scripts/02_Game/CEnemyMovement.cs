/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemyMovement.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/11/01
 * 죽었을시, skill 인보크 모두 취소
 *  (죽었다가 일어나서 춤추거나, 호박을 던지려해서...)
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/31
 * CEnemyParams.cs로 attackRange옮겨 보냄
 * Dance관련 CEnemySkill로 옮겨 보냄
 * Rogue 스피드 너프 - 일반의 2배
 * King의 스피드업 버프 받기, 종료
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/26
 * King 댄스
 * 시간 랜덤으로 주기
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/24
 *  죽었을시
 *      이동 멈추고, 본 컴포넌트 무시
 *      
 * 로그가 공격시, 멧돼지 달리기 멈춤
 * 로그가 이동시, 멧돼지 달리기 애니메이션 ㄱㄱ
 * 
 * 멧돼지 죽으면, 걸어가기
 *  느림느림~ 2
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/18
 * 에너미가 목표지점에 도착했을시 하늘을 쳐다보는 버그 수정됨
 * 
 * 에너미 종류에 따른 공격 사정거리, 이동속도 변경
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 * 2018/10/16
 * 
 * CMonsterFSM에서 이동관려만 -> CEnemyFSM
 *  
 * 사용되지 않는 파츠 제거
 * 참조 변수 동적 연결로 변경
 * 
 * NavMeshAgent를 이용한 이동
 * 
 * 공격 사정거리 내에 들어오면 NavMeshAgent의 이동을 멈추고 공격
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.AI;

public class CEnemyMovement : MonoBehaviour
{
    private const float SPEED_UP_BUF_RATIO = 2f;
    private const float BOAR_SPEED_RATIO = 4f;
    private const float DISMOUNTED_SPEED = 8f;

    private CEnemyParams _params;
    private CEnemySkill _skills;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _target;

    private GameObject _boar; // 로그일시 사용할 참조 변수
    public float _origSpeed;
    private bool _isSpeedUp;

    private void Awake()
    {
        _params = GetComponent<CEnemyParams>();
        _skills = GetComponent<CEnemySkill>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(name.Contains("Rogue"))      // 로그는 돼지타고 뛰니까 빨라
        {
            _navMeshAgent.speed *= BOAR_SPEED_RATIO;
            _boar = transform.Find("Hips_jnt/Spine_jnt/Boar").gameObject;
        }
        _isSpeedUp = false;
    }

    private void Start()
    {
        _target = GameObject.FindGameObjectWithTag("EnemyGoal").transform;
        _navMeshAgent.speed *= (1 + (_params._battleManager._enemyLevel - 1) * 0.2f);
        _origSpeed = _navMeshAgent.speed;
    }

    private void Update()
    {
        if(_params.GetFSM() == CharFSM.DIE)
        {
            _navMeshAgent.enabled = false;
            CancelInvoke();
            if(_skills != null)
            {
                _skills.CancelInvoke();
            }
            return;
        }
        else if(_params.GetFSM() == CharFSM.STUNNED)
        {
            _navMeshAgent.isStopped = true;
            return;
        }

        // 어차피 navemeshagent를 꺼버림
        //if(_params.GetFSM() == CharFSM.DANCE || _params.GetFSM() == CharFSM.PUMPKIN)
        //{
        //    return;
        //}

        if(_navMeshAgent.enabled == false)
        {
            return;
        }

        if(_target == null)
        {
            _params.SetFSM(CharFSM.IDLE);
            return;
        }

        float distance = Vector3.Distance(_target.position, transform.position);
        if(distance <= _params._attackRange)
        {
            _params.SetFSM(CharFSM.ATTACK);
            _navMeshAgent.isStopped = true;
            transform.LookAt(new Vector3(_target.position.x, transform.position.y, _target.position.z));
            if(name.Contains("Rogue"))
            {
                _boar.GetComponent<Animator>().SetBool("Move", false);
            }
        }
        else
        {
            // 상태이상에 갇힌게 아닐시, TRACE 상태로 변경
            if(!_params.IsCC)
            {
                _params.SetFSM(CharFSM.TRACE);
            }
            _navMeshAgent.isStopped = false;
            if(_params.GetFSM() == CharFSM.TRACE)
            {
                _navMeshAgent.SetDestination(_target.position);
            }
            if(name.Contains("Rogue"))
            {
                if(_boar != null)
                {
                    _boar.GetComponent<Animator>().SetBool("Move", true);
                }
            }
        }
    }

    public void RideKilled()
    {
        _navMeshAgent.baseOffset = 0;           // 에너미 높이 변경
        _origSpeed /= DISMOUNTED_SPEED;
        _navMeshAgent.speed /= _origSpeed;       // 에너미 속도 감소
        _boar.transform.parent = null;          // 에너미에서 분리
        _boar.transform.Rotate(Vector3.forward * 90);   // 눕히고
        _boar.transform.position += Vector3.up;         // 들어올려서 땅에 놓고
        _boar.GetComponent<Animator>().SetBool("Dead", true);   // 죽은 애니메이션
        _boar.GetComponent<Collider>().enabled = false;         // 이제 안 맞게
        Destroy(_boar, 3f);                                     // 3초 후에 없애기
    }

    // CEnemyCollisionCheck에서 호출
    // 스피드 업 버프를 받지 않았다면, 2배 빨르게 버프 온
    public void SpeedUp()
    {
        if(!_isSpeedUp)
        {
            _navMeshAgent.speed *= SPEED_UP_BUF_RATIO;
            _isSpeedUp = true;
        }
    }

    // CEnemyCollisionCheck에서 호출
    // 스피드 업 버프 해제
    public void SpeedUpEnd()
    {
        _origSpeed = _navMeshAgent.speed;
        _isSpeedUp = false; ;
    }
}
