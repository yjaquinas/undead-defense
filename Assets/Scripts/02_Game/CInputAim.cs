/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CInputAim.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/22
 * Transform.rotation + slerp 방식에서 Transform.Rotate()로 변경
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/17
 * 스크린 좌/우 터치 분할
 * 터치 - 마우스클릭
 *  좌: 각도변경
 *  
 *  키보드 터치 없앰
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/16
 * CInputAim.cs
 *  조준 관리 관련
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/02
 * Battle 씬의 Player에 추가된 컴포넌트형 스크립트
 * 
 *  UpdateAim(): 유저의 플레이어 조작
 *      키보드로 조준점 조절: 카메라 rotation조절
 *      - Y축에대한 rotation이 transform.x를 변경
 *      - X축에대한 rotation이 transform.y를 변경
 *      * 터치 스와잎 모션이 크면 (한번에 많이 움직이면),
 *  xxxxxxxxxxxxxxxxxxxx 수정됨 2018/10/02 xxxxxxxxxx
 *       각도 조정 최대치를 넘어가는 경우가 있어서, ANGLE_LIMIT_BUFFER를 걸어둠
 *  xxxxxxxxxx 수정됨 2018/10/02 xxxxxxxxxxxxxxxxxxxx
 *       
 *  최대치 수정 - 없앰
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using UnityEngine;

public class CInputAim : MonoBehaviour
{
    [SerializeField] private CBattle _battleManager;

    public const float DEFAULT_SENSITIVITY = 60.0f;
    public const float TOUCH_SENSITIVITY = 0.0015f;

    private float _lookRotationX, _lookRotationY; // 카메라 회전도
    private float _aimSensitivity;              // 플레이어 터치 조준 조작 속도

    private bool trackMouse;
    private Vector3 lastPosition;

    private bool _isTouchAiming;
    private int _leftTouchIndex;
    private int _screenWidthHalf;             // 해상도 가로 반

    private void Start()
    {
        SetSensitivity(DEFAULT_SENSITIVITY);
        _leftTouchIndex = 0;
        _screenWidthHalf = Screen.width / 2;
        _isTouchAiming = false;
    }

    void Update()
    {
        if(_battleManager._FSM == CBattle.BattleFSM.BATTLE && !CMobileBackButton.IsPaused)
        {
            UpdateAim();
        }
    }

    // 유저의 스와이프 input을 읽어서,
    // 플레이어의 조준점을 조정
    // 현재 조준점을 읽어서, 그위에 유저의 input을 더한후, 최대치를 넘지 않는 각도에 한해서 카메라 플레이어의 rotation변경
    // 공격은 플레이어의 crossbow에서 책임
    void UpdateAim()
    {
        // 조준점 변경 = 플레이어 rotation조정 -> 카메라 + 석궁 앵글 조정
        _lookRotationX = transform.rotation.eulerAngles.x;     // 현재 플레이어 의 조준 x각도
        _lookRotationY = transform.rotation.eulerAngles.y;     // 현재 플레이어 의 조준 y각도

        //// 모바일 좌우 화면 터치 관련 주석 처리 부분--------------------------------------------------------------------------------------
        // 터치 컨트롤
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(Input.touchCount - 1);
            if(touch.position.x < _screenWidthHalf)
            {
                _leftTouchIndex = Input.touchCount - 1;
                _isTouchAiming = true;
                switch(touch.phase)
                {
                    case TouchPhase.Ended:
                        _isTouchAiming = false;
                        break;
                }
            }
            if(_isTouchAiming)
            {
                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(_leftTouchIndex).deltaPosition * _aimSensitivity * TOUCH_SENSITIVITY;
                _lookRotationX -= touchDeltaPosition.y;
                _lookRotationY += touchDeltaPosition.x;
            }
            transform.rotation = Quaternion.Euler(_lookRotationX, _lookRotationY, transform.rotation.z);
        }
        //// 모바일 좌우 화면 터치 관련 주석 처리 부분--------------------------------------------------------------------------------------

        // 키보드
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // 플레이어 rotation 조정
        transform.Rotate(Vector3.up * horizontal * _aimSensitivity * Time.deltaTime, Space.World);
        transform.Rotate(transform.right * -vertical * _aimSensitivity * Time.deltaTime, Space.World);

        //// 마우스 컨트롤 - 터치연동됨
        //if(Input.GetButtonDown("Fire1"))
        //{
        //    trackMouse = true;
        //    lastPosition = Input.mousePosition;
        //}
        //else if(Input.GetButtonUp("Fire1"))
        //{
        //    trackMouse = false;
        //    //_newXAngle = 0;
        //    //_newYAngle = 0;
        //}
        //if(trackMouse)
        //{
        //    Vector3 newPosition = Input.mousePosition;
        //    _lookRotationX -= (newPosition.y - lastPosition.y) * _aimSensitivity * TOUCH_SENSITIVITY;
        //    _lookRotationY += (newPosition.x - lastPosition.x) * _aimSensitivity * TOUCH_SENSITIVITY;
        //    lastPosition = newPosition;
        //}
        //transform.rotation = Quaternion.Euler(_lookRotationX, _lookRotationY, transform.rotation.z);

    }

    // 감도 조절
    public void SetSensitivity(float newSensitivity)
    {
        _aimSensitivity = newSensitivity;
    }
}

