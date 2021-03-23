/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CEnemyCollisionCheck.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/31
 * 킹의 speed up 버프를 받았을시, 해당 버프에 맞는 버프 주기
 *  속도 업, 방어력업
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/30
 * 에너미의 Hips_jnt에 붙임 - collider가 있는 GameObject
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;

public class CEnemyCollisionCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyBuf")
        {
            CEnemySkill.BUF_TYPE bufType = other.gameObject.transform.parent.gameObject.GetComponent<CEnemySkill>()._bufType;
            switch(bufType)
            {
                case CEnemySkill.BUF_TYPE.DEFENSE_UP:
                    transform.parent.GetComponent<CEnemyParams>().DefenseUp();
                    break;
                case CEnemySkill.BUF_TYPE.SPEED_UP:
                    transform.parent.GetComponent<CEnemyMovement>().SpeedUp();
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "MercSkill")
        {
            transform.parent.gameObject.GetComponent<CEnemyParams>().CCFinish();
        }
        if(other.gameObject.tag == "EnemyBuf")
        {
            CEnemySkill.BUF_TYPE bufType = other.gameObject.transform.parent.gameObject.GetComponent<CEnemySkill>()._bufType;
            switch(bufType)
            {
                case CEnemySkill.BUF_TYPE.DEFENSE_UP:
                    transform.parent.GetComponent<CEnemyParams>().DefenseUpEnd();
                    break;
                case CEnemySkill.BUF_TYPE.SPEED_UP:
                    transform.parent.GetComponent<CEnemyMovement>().SpeedUpEnd();
                    break;
            }
        }
    }
}
