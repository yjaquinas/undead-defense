/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * CScope.cs
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/24
 * 게임오버시
 *  스코프, 크로스헤어 비활성화
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/17
 * 스크린 좌/우 터치 분할
 *  우: 줌인 스코프
 * 스코프는 전투중에만 활성화
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2018/10/16
 * 
 * 스코프의 컴포넌트형 스크립트
 * 
 * 스코프를 통한 줌인 기능
 * 
 * 줌인 됐을시 애니메이션 호출,
 *  애니메이션에서 스코프 모드 호출
 *      크로스보우와 스코프 렌더러 숨기기
 *      스코프를 통한 view (overlay) 보이기
 *      카메라를 SCOPED_ZOOM_RATIO 배율로 줌인
 *      감도를 SCOPED_SENSITIVITY_RATIO 배율로 낮춤
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CScope : MonoBehaviour
{
    private const float SCOPED_SENSITIVITY_RATIO = 4;
    private const float NORMAL_FOV = 60f;
    private const float SCOPED_ZOOM_RATIO = 4;

    private CBattle _battleManager;

    private Animator _animator;
    private MeshRenderer _crossbowRenderer;

    private CInputAim _inputAim;

    private int _screenWidthHalf;             // 해상도 가로 반

    private bool _isScoped = false;             // 현재 줌인 됐는지 플래그


    [SerializeField] private Button _zoomButton; //줌인 버튼

    private bool _zoomIn; //줌인 토글

    private void Start()
    {
        _screenWidthHalf = Screen.width / 2;

        _battleManager = GameObject.Find("BattleManager").GetComponent<CBattle>();

        _animator = GetComponent<Animator>();
        _crossbowRenderer = GetComponent<MeshRenderer>();
        _inputAim = transform.parent.GetComponent<CInputAim>();

        _zoomIn = false;
    }

    private void Update()
    {
        // 전투중에만 스코프 사용
        if(_battleManager._FSM != CBattle.BattleFSM.BATTLE)
        {
            _animator.SetBool("Scoped", false);
            return;
        }
    }

    public void ButtonClick()
    {
        _zoomIn = !_zoomIn;
        ScopeToggle();
    }

    public void ScopeToggle()
    {
        if(_zoomIn)
        {
            _isScoped = true;
            _animator.SetBool("Scoped", _isScoped);
        }
        else
        {
            _isScoped = false;
            _animator.SetBool("Scoped", _isScoped);
        }
    }


    // on=true : 스코프 줌인 모드
    // CScopeIdleEvent에서 호출
    public void ScopeToggle(bool on)
    {

        _animator.SetBool("Scoped", on);

        if(!on)
        {
            Camera.main.fieldOfView = NORMAL_FOV;
            _inputAim.SetSensitivity(CInputAim.DEFAULT_SENSITIVITY);
        }
        else
        {
            Camera.main.fieldOfView = NORMAL_FOV / SCOPED_ZOOM_RATIO;
            _inputAim.SetSensitivity(CInputAim.DEFAULT_SENSITIVITY / SCOPED_SENSITIVITY_RATIO);
        }
    }

}
