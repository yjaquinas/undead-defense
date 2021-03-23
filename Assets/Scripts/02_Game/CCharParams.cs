/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CCharacterParams.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/24
 * 죽으면 HP바 숨기기
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/18
 *  CEnemyFSM -> CharFSM 변경 후 Params에 추가
 *  애니메이션은 FSM에 따라 FSM에서 자동 변경하게 관리
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;
using UnityEngine.UI;

public enum CharFSM
{
    IDLE,
    TRACE,
    ATTACK,
    DIE,
    DANCE,
    PUMPKIN,
    FROZEN,
    STUNNED,
    TORNADO
};

public class CCharParams : MonoBehaviour
{
    public CBattle _battleManager;
    protected Image _hpBar;

    [SerializeField]
    protected CharFSM _FSM = CharFSM.IDLE;

    protected Animator _animator;

    protected int _maxHp;
    protected int _curHp;

    [SerializeField] protected Collider _bodyCollider;     // 공격 맞는 콜라이더
    [SerializeField] protected Collider _headCollider;     // 헤드샷 콜라이더

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _battleManager = GameObject.Find("BattleManager").GetComponent<CBattle>();
        _hpBar = transform.Find("Billboard/HpFront").GetComponent<Image>();
        _hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);   // 체력바 초기화
        _headCollider = transform.Find("Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Neck_jnt/Head_jnt").gameObject.GetComponent<Collider>();
        _bodyCollider = transform.Find("Hips_jnt").gameObject.GetComponent<Collider>();
    }

    public CharFSM GetFSM()
    {
        return _FSM;
    }
    public void SetFSM(CharFSM state)
    {
        _FSM = state;
        _animator.SetInteger("State", (int)state);
    }


    // 피격
    public virtual void GetHit(string attType, int attackDamage)
    {
    }
}
