using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        // 메인 카메라와 같은 시선으로 UI를 회전함
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
