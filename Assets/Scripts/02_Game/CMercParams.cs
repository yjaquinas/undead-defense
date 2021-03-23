using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMercParams : CCharParams
{

    public enum MERC_TYPE
    {
        ICE,
        LIGHTNING,
        TORNADO,
        GAS
    }
    [SerializeField] private MERC_TYPE _type;       // 용병이 사용할 스킬 (용병 타입)

    public Transform curtarget;

    // 안쓰임!---
    private const float ICE_DELAY = 10f;
    private const float LIGHTNING_DELAY = 5f;
    private const float TORNADO_DELAY = 20f;
    private const float GAS_DELAY = 3f;
    // ---안쓰임!

    public const float ICE_DURATION = 5f;
    public const float LIGHTNING_DURATION = 3f;
    public const float TORNADO_DURATION = 10f;
    public const float GAS_DURATION = 3f;

    public float _currAttackDelay;             // 시전 속도
    public float attackTimer;                  // 스킬 시전 타이머

    public GameObject particle_skill1;  // 1번 스킬 이펙트 넣는곳
    public GameObject particle_skill2;  // 2번 스킬 이펙트 넣는곳
    public GameObject particle_skill3;  // 3번 스킬 이펙트 넣는곳
    public GameObject particle_skill4;  // 4번 스킬 이펙트 넣는곳
    public GameObject disappearCollider; // 스킬 콜라이더 사라지게 할 매개변수넣는

    [SerializeField] private SkinnedMeshRenderer _smr;      // 옷 렌더러
    [SerializeField] private Material[] _outfitMaterials;       // 용병 타입에 따른 색상
    [SerializeField] private AudioClip[] _magicACs = new AudioClip[4];
    [SerializeField] private AudioSource _magicAS;

    void Start()
    {
        setType((int)_type);
    }

    public void setType(int type)
    {
        _type = (CMercParams.MERC_TYPE)type;
        _magicAS.clip = _magicACs[(int)_type];
        //_smr.material = _outfitMaterials[(int)_type];       // 용병 색상 변경
        switch(_type)
        {
            case MERC_TYPE.ICE:
                _currAttackDelay = ICE_DELAY;
                break;
            case MERC_TYPE.LIGHTNING:
                _currAttackDelay = LIGHTNING_DELAY;
                break;
            case MERC_TYPE.TORNADO:
                _currAttackDelay = TORNADO_DELAY;
                break;
            case MERC_TYPE.GAS:
                _currAttackDelay = GAS_DELAY;
                break;
            default:
                break;
        }
        attackTimer = _currAttackDelay;     // 첫 공격은 딜레이 없이
    }

    // 본 용병의 스킬 애니메이션 플레이
    public void UseSkill(Vector3 targetPos)
    {
        _animator.SetTrigger("UseSkill");
        _magicAS.Play();
        // 애니메이션 하고 0.6초 뒤에 마법 나가게 (sync)
        Invoke("MagicEffectOn", 0.6f);
        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
    }

    private void MagicEffectOn()
    {
        switch(_type)
        {
            case MERC_TYPE.ICE:
                particle_skill1.transform.position = new Vector3(curtarget.position.x, 0, curtarget.position.z);
                particle_skill1.GetComponent<ParticleSystem>().Play();
                particle_skill1.GetComponent<Collider>().enabled = true;
                disappearCollider = particle_skill1;
                Invoke("DisappearCollider", ICE_DURATION);
                break;
            case MERC_TYPE.LIGHTNING:
                particle_skill2.transform.position = new Vector3(curtarget.position.x, 0, curtarget.position.z);
                particle_skill2.GetComponent<ParticleSystem>().Play();
                particle_skill2.GetComponent<Collider>().enabled = true;
                disappearCollider = particle_skill2;
                Invoke("DisappearCollider", LIGHTNING_DURATION);
                break;
            case MERC_TYPE.TORNADO:
                particle_skill3.transform.position = new Vector3(curtarget.position.x, 0, curtarget.position.z);
                particle_skill3.GetComponent<ParticleSystem>().Play();
                particle_skill3.GetComponent<Collider>().enabled = true;
                disappearCollider = particle_skill3;
                Invoke("DisappearCollider", TORNADO_DURATION);
                break;
            case MERC_TYPE.GAS:
                particle_skill4.transform.position = new Vector3(curtarget.position.x, 0, curtarget.position.z);
                particle_skill4.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                particle_skill4.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                particle_skill4.GetComponent<Collider>().enabled = true;
                disappearCollider = particle_skill4;
                //Invoke("DisappearCollider", GAS_DURATION);
                Invoke("StopGas", GAS_DURATION);
                break;
        }
    }

    void DisappearCollider()
    {
        disappearCollider.GetComponent<Collider>().transform.position += new Vector3(0, 1000, 0);
        disappearCollider.GetComponent<ParticleSystem>().Stop();
    }
    void DisappearColliderLightning()
    {
        disappearCollider.GetComponentInChildren<Collider>().transform.position += new Vector3(0, 1000, 0);
        disappearCollider.GetComponentInChildren<ParticleSystem>().Stop();
    }
    void StopGas()
    {
        disappearCollider.GetComponent<Collider>().transform.position += new Vector3(0, 1000, 0);
        particle_skill4.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        particle_skill4.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
    }
}
