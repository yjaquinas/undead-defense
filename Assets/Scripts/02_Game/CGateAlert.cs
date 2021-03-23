using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGateAlert : MonoBehaviour {

    public Image alertImage;
    public int count = 0;

    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.tag == "EnemyPart")
        {
            count = 0;
            CancelInvoke("Alert");
            InvokeRepeating("Alert", 0f,0.3f);
        }
    }

    private void Alert()
    {
        alertImage.enabled = !alertImage.enabled;
        count++;
        if(count >= 10)
        {
            CancelInvoke("Alert");
            alertImage.enabled = false;
        }
    }

}
