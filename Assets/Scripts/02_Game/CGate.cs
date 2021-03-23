using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGate : MonoBehaviour
{
    [SerializeField] CBattle _battleManager;
    [SerializeField] private Image _hpBar;

    [SerializeField] private AudioSource _as;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EnemyPart"))
        {
            _as.Play();
            _hpBar.rectTransform.localScale = _battleManager.EnemyPassedDoor();
            Destroy(other.transform.root.gameObject, 1f);
            _battleManager._enemyCount--;
        }
    }
}
