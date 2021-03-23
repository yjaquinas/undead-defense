/*
 * 
 * 화면에 리젠된 몹이 안보이면 아이콘 띄우기!
 * 리젠된 몬스터가 카메라 기준 왼쪽과 오른쪽을 구분해서 아이콘을 띄우기
 */
using UnityEngine;
using UnityEngine.UI;

public class CEnemySpawnVisibility : MonoBehaviour
{
    [SerializeField] Renderer _renderer;

    private Image _UIIndicatorIconImageLeft;
    private Image _UIIndicatorIconImageRight;
    private bool _isFirstRendering = true;
    private float _angleFromEnemy = 0;

    private void Start()
    {
        _UIIndicatorIconImageLeft = GameObject.Find("LeftSpawnIcon").GetComponent<Image>();
        _UIIndicatorIconImageRight = GameObject.Find("RightSpawnIcon").GetComponent<Image>();
    }

    private void Update()
    {
        if (_isFirstRendering)
        {
            _isFirstRendering = false;
            if(!_renderer.isVisible)
            {
                if (Camera.main.WorldToScreenPoint(transform.position).x < Screen.width / 2)
                {
                    _UIIndicatorIconImageLeft.enabled = true;
                    CancelInvoke("HideUIIcon");
                    Invoke("HideUIIcon", 2f);
                }
                else
                {
                    _UIIndicatorIconImageRight.enabled = true;
                    CancelInvoke("HideUIIcon");
                    Invoke("HideUIIcon", 2f);
                }
            }
        }
        else if(!_UIIndicatorIconImageLeft.enabled && !_UIIndicatorIconImageRight.enabled)
        {
            //Destroy(this);      // 한번만 체크하고 이 스크립트는 빠이빠이영
        }
    }

    private void HideUIIcon()
    {
        _UIIndicatorIconImageLeft.enabled = false;
        _UIIndicatorIconImageRight.enabled = false;
    }
}
