/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CArrow.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 생성되면 직진!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;

public class CArrow : MonoBehaviour
{

    private const float MERC_SKILL_RADIUS = 35;
    [SerializeField] AudioSource _as;
    [SerializeField] AudioClip[] _collisionSoundACs;

    [SerializeField] private float _speed;                  //화살 속도
    [SerializeField] private float _destroyTime;            //화살 제거 시간
    [SerializeField] private int _damage;                   // 데미지
    private float timer; //화살 제거 타이머

    private float _spinSpeed = 360 * 5;


    //화살을 생성시의 각도로 보내기 
    private void Awake()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * _speed;
        timer = 0f;
        Invoke("DestroyIfNotStuck", _destroyTime);  // cancel if stuck
    }

    private void Update()
    {
        transform.Rotate(transform.forward, _spinSpeed * Time.deltaTime, Space.World);
    }

    private void DestroyIfNotStuck()
    {
        Destroy(gameObject);
    }


    // 다른오브젝트와 부딛히면
    // 화살 오브젝트 꽂히게 - StickArrow()
    private void OnTriggerEnter(Collider other)
    {
        _as.clip = _collisionSoundACs[0];

        string colliderTag = other.gameObject.tag;
        // 박히면 안되는거 스킵
        if(colliderTag == "Mercenary" || colliderTag == "EnemyBuf" || colliderTag == "MercSkill" || colliderTag == "Player" || colliderTag == "EnemyProjectile" || colliderTag == "HELL" || colliderTag =="Gate")
        {
            return;
        }

        StickArrow(other.gameObject, other.transform.position, transform.rotation);    // 피격된 부위에 화살 박기

        if(colliderTag == "EnemyHead")         // 머리 쏨
        {
            SkillShot(other.gameObject);
            int headShotDamage = Random.Range(1, 9) * 1000 + Random.Range(0, 10) * 100 + Random.Range(0, 10) * 10 + Random.Range(0, 10);
            other.gameObject.transform.Find("Hit_06").GetComponent<ParticleSystem>().Play();     // 파티클 이펙트
            other.transform.root.gameObject.GetComponent<CEnemyParams>().PlayHeadShotEffect();     // 헤드샷이펙트
            other.transform.root.gameObject.GetComponent<CEnemyParams>().HeadShot();                    // 헤드샷 했다고 알려줍니다
            other.transform.root.gameObject.GetComponent<CEnemyParams>().GetHit("Arrow", headShotDamage); // 큰 데미지 - 원샷킬을 노립니다
            _as.clip = _collisionSoundACs[4]; // 헤드샷 소리
        }
        else if(colliderTag == "Boar")     // 멧돼지 쏨
        {
            SkillShot(other.gameObject);
            other.transform.root.gameObject.GetComponent<CEnemyParams>().FallFromRide();
            other.transform.root.gameObject.GetComponent<CEnemyMovement>().RideKilled(); // 죽이기
            other.gameObject.GetComponent<Collider>().enabled = false;                   // 멧돼지 콜라이더 비활성화
            _as.clip = _collisionSoundACs[3]; // 멧돼지 소리
        }
        else if(colliderTag == "EnemyPart")
        {
            SkillShot(other.gameObject);
            other.transform.root.gameObject.GetComponent<CEnemyParams>().GetHit("Arrow", _damage);
            string enemyType = other.transform.root.name;
            switch(enemyType)
            {
                case "UndeadSkeleton(Clone)":
                    _as.clip = _collisionSoundACs[1];
                    break;
                case "UndeadHeavy(Clone)":
                    _as.clip = _collisionSoundACs[2];
                    break;
                case "UndeadRogue(Clone)":
                    _as.clip = _collisionSoundACs[1];
                    break;
            }
        }
        _as.Play();
    }


    void SkillShot(GameObject other)
    {
        // 타워들 4개를 찾은다음에
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Mercenary"))
        {
            // 화살을 맞은 유닛이 타워의 범위 안에 있다면 스킬공격하기.
            if(Vector3.Distance(transform.position, obj.transform.position) <= MERC_SKILL_RADIUS)
            {
                obj.GetComponentInChildren<CMercParams>().curtarget = other.gameObject.transform;
                obj.GetComponentInChildren<CMercParams>().UseSkill(other.gameObject.transform.position);
                return;
            }
        }
        gameObject.GetComponent<Collider>().enabled = false;
    }

    // 화살이 에너미에게
    // 빈 더미 화살 꺼내고, 자신 파괴
    private void StickArrow(GameObject objectHit, Vector3 hitPosition, Quaternion arrowDirection)
    {
        transform.Find("DummyArrow").localScale = Vector3.one * 2;
        if(objectHit.name == "Boar")
        {
            transform.Find("DummyArrow").localScale = Vector3.one * 10;
        }
        //transform.Find("DummyArrow").gameObject.SetActive(true);
        transform.Find("DummyArrow").parent = objectHit.transform;
        transform.Find("BasicArrowRotationTweaked").gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }

}
