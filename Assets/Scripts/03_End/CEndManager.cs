using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameData; // MiniJSON
public class CEndManager : MonoBehaviour
{
    private const float ROW_PANEL_HEIGHT = 80;

    // 순위 정보 출력용 로우 패널 프리팹
    [SerializeField] private GameObject _rankRowPanelPrefab;

    // 스크롤뷰 출력 영역 기준 오브젝트 참조
    [SerializeField] private Transform _bestPointContentTrans;

    private string _bestPoint;
    private string _gamePoint;

    // 점수
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _infoMsgText;

    // 시작 페이지 번호 (0부터)
    [SerializeField] private int _startPage;
    // 리스트 항목 갯수
    [SerializeField] private int _listCount;
    // 전체 랭크 갯수
    [SerializeField] private int _totalRankCount;

    private void Start()
    {
        _infoMsgText.text = "";

        _bestPoint = PlayerPrefs.GetString("BEST_SCORE", "0");
        _gamePoint = PlayerPrefs.GetString("GAME_SCORE", "0");

        if(int.Parse(_gamePoint) > int.Parse(_bestPoint))
        {
            _bestPoint = _gamePoint;
            //StartCoroutine("UpdateBestScore");
        }
        else
        {
            //StartCoroutine("BestPointRankCoroutine");
        }
        _scoreText.text = "Best Score : " + _bestPoint + "\nScore : " + _gamePoint;

    }

    // 랭크 항목들을 클리어함
    void ClearRankList()
    {
        Image[] listPanels = _bestPointContentTrans.GetComponentsInChildren<Image>();
        foreach(Image listPanel in listPanels)
        {
            Destroy(listPanel.gameObject);
        }
    }


    // 현재 유저의 랭크 패널 추가
    private void CreateMyRankRowPanel(string myrank_count)
    {
        GameObject rowPanel = Instantiate(_rankRowPanelPrefab, Vector2.zero, Quaternion.identity, _bestPointContentTrans);
        rowPanel.GetComponent<CRankRowPanel>().SetRankInfo(myrank_count, PlayerPrefs.GetString("NICKNAME"), _bestPoint);
        rowPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0);
        rowPanel.transform.GetChild(0).GetComponent<Image>().color = Color.cyan;
    }

    public void OnPrevButtonClick()
    {
        if(_startPage <= 0)
        {
            Debug.Log("현재 페이지가 첫번째 페이지임");
            return;
        }
        _startPage--;

        ClearRankList();

        StartCoroutine("BestPointRankCoroutine");
    }

    public void OnNextButtonClick()
    {
        _startPage++;
        if(_totalRankCount <= (_listCount * _startPage))
        {
            _startPage--;
            Debug.Log("현제 페이지가 마지막 페이지임");
            return;
        }

        ClearRankList();

        StartCoroutine("BestPointRankCoroutine");
    }

    IEnumerator BestPointRankCoroutine()
    {
        string url = CCommon.BaseUrl + "best_point_rank_list";

        WWWForm form = new WWWForm();

        form.AddField("best_point", _bestPoint);
        form.AddField("start_page", _startPage);
        form.AddField("list_count", _listCount);

        WWW www = new WWW(url, form);

        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            int rank = 0;

            Dictionary<string, object> rankData = MiniJSON.jsonDecode(www.text.Trim()) as Dictionary<string, object>;

            string result = rankData["result"].ToString();

            if(result.Equals("RANK_SUCCESS"))
            {
                _totalRankCount = int.Parse(rankData["total_rank_count"].ToString());

                // 현재 유저의 랭크 카운트를 구함 + 생성함
                CreateMyRankRowPanel(rankData["myrank"].ToString());
                // 순위 리스트를 추출
                List<object> rankList = rankData["rank"] as List<object>;

                float posY = -ROW_PANEL_HEIGHT;

                for(int i = 0; i < rankList.Count; i++)
                {
                    rank++;
                    Dictionary<string, object> rankRowData = rankList[i] as Dictionary<string, object>;

                    // 생성한 로우 패널을 스크롤뷰의 Content 자식으로 등록함
                    GameObject rowPanel = Instantiate(_rankRowPanelPrefab, Vector2.zero, Quaternion.identity, _bestPointContentTrans);

                    string nickname = rankRowData["nickname"].ToString();
                    string bestPoint = rankRowData["best_point"].ToString();

                    // 내꺼 색상 변경
                    if(nickname == PlayerPrefs.GetString("NICKNAME"))
                    {
                        rowPanel.transform.GetChild(0).GetComponent<Image>().color = Color.cyan;
                    }
                    // 순위 정보 출력 패널에 순위 정보를 출력
                    rowPanel.GetComponent<CRankRowPanel>().SetRankInfo(((i + 1) + (_listCount * _startPage)).ToString(), nickname, bestPoint);
                    // 순위 정보 출력 패널의 위치를 설정함
                    rowPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posY);

                    // 패널 위치 설정
                    posY -= ROW_PANEL_HEIGHT;
                }
                // 스크롤 콘텐츠 뷰의 크기를 재 설정함
                RectTransform rt = _bestPointContentTrans.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Abs(posY));
            }
        }
        else
        {
            Debug.Log("서버 통신 에러 발생");
        }
    }

    IEnumerator UpdateBestScore()
    {
        string url = CCommon.BaseUrl + "update_user_best_point";

        WWWForm form = new WWWForm();

        form.AddField("nickname", PlayerPrefs.GetString("NICKNAME"));
        form.AddField("best_point", _bestPoint);

        WWW www = new WWW(url, form);

        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            int rank = 0;

            Dictionary<string, object> rankData = MiniJSON.jsonDecode(www.text.Trim()) as Dictionary<string, object>;

            string result = rankData["result"].ToString();
        }
        else
        {
            Debug.Log(www.error);
        }

        StartCoroutine("BestPointRankCoroutine");
    }

    public void OnResetbuttonClick()
    {
        //SceneManager.LoadScene("01_Enter");
        SceneManager.LoadScene("02_Game");
        _infoMsgText.text = "loading...";
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("유니티 에디터에선 안꺼지지롱00;");
        Application.Quit();
        _infoMsgText.text = "BYE BYE";
    }
}
