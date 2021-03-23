/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CMercSkill.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/29
 * 이름변경: Skill.cs -> CMercSkill
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;

public class CMercSkill : MonoBehaviour
{
    private const int ICE_DAMAGE = 1;
    private const int LIGHTNING_DAMAGE = 3;
    private const int TORNADO_DAMAGE = 1;
    private const int GAS_DAMAGE = 3;

    private string _type;
    private int _damage;

    private void Awake()
    {
        _type = name;
        switch(_type)
        {
            case "Ice":
                _damage = ICE_DAMAGE;
                break;
            case "Lightning":
                _damage = LIGHTNING_DAMAGE;
                break;
            case "Tornado":
                _damage = TORNADO_DAMAGE;
                break;
            case "Gas":
                _damage = GAS_DAMAGE;
                break;
        }
    }


    // 에너미 콜라이더는 머리에
    // 머리의 root는 에너미 본체
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "EnemyPart")
        {
            col.transform.root.GetComponent<CEnemyParams>().tornadoPos = transform.position;
            col.transform.root.GetComponent<CEnemyParams>().GetHit(_type, _damage);
        }
    }

}
