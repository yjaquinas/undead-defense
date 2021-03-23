/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemyProjectile.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/11/01
 * CEnemySkill.cs에서 생성됨
 * 플레이어와 충돌시, 터지고, 조금 후 destory
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;

public class CEnemyProjectile : MonoBehaviour
{
    private const float PUMPKIN_THROW_VELOCITY = 30;
    [SerializeField] private GameObject _explosionEffect;

    public void Shoot()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        GetComponent<Rigidbody>().velocity = directionToCamera.normalized * PUMPKIN_THROW_VELOCITY;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GetComponent<AudioSource>().Play();                     // 터지는소리
            GetComponent<Rigidbody>().velocity = Vector3.zero;             // 멈추고
            GetComponent<MeshRenderer>().enabled = false;       // 숨기고
            _explosionEffect.GetComponent<ParticleSystem>().Play(); // 터지고
            Destroy(gameObject, 1);                                 // 파괴
        }
    }
}
