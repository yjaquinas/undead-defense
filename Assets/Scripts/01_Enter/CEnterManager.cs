using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;
using UnityEngine.SceneManagement;

public class CEnterManager : MonoBehaviour
{
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private InputField _nicknameIF;
    [SerializeField] private Text _infoMsgText;

    private void Start()
    {
        _infoMsgText.text = "";
        _nicknameIF.text = PlayerPrefs.GetString("NICKNAME", "default");
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("02_Game");
        _infoMsgText.text = "loading...";

        //if(string.IsNullOrEmpty(_nicknameIF.text.ToLower().Trim()))
        //{
        //    _infoMsgText.text = "Please enter your nickname";
        //    return;
        //}

        //StartCoroutine("LoadOrJoinUserInfoNetCoroutine");
    }

    IEnumerator LoadOrJoinUserInfoNetCoroutine()
    {
        _infoMsgText.text = "Connecting...";

        string url = CCommon.BaseUrl + "join_or_load_user";

        WWWForm form = new WWWForm();
        form.AddField("nickname", _nicknameIF.text.Trim());

        WWW www = new WWW(url, form);

        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            // 서버에서 받은 JSON 데이타(string)을 MiniJSON으로
            // 객체 변환을 하면 object 객체로 변환해줌
            // 따라서 object 객체는 JSON 요소에 맞는 타입으로 변경해야함
            // C#에서는 데이타 as 변경할타입 형태로 캐스팅이 가능함

            // 1. JSON문자열 -> MiniJSON.jsonDecode 통해 객체화
            // 2. 객체화된 object를 Dictionary<string, object> 타입으로 캐스팅
            Dictionary<string, object> responseData = MiniJSON.jsonDecode(www.text.Trim()) as Dictionary<string, object>;

            // 응답 결과 체크
            string result = responseData["result"].ToString();


            if(result.Equals("LOAD_SUCCESS"))
            {
                // 유저 정보 딕셔너리 객체를 참조함
                Dictionary<string, object> infoData = responseData["info"] as Dictionary<string, object>;

                // Dictionary 유저정보 객체를 JSON 문자열로 변환
                string jsonInfo = MiniJSON.jsonEncode(infoData);

                // PlayerPrefs에 유저의 정보 JSON문자열을 저장함
                PlayerPrefs.SetString("BEST_SCORE", infoData["best_point"] as string);
                PlayerPrefs.SetString("NICKNAME", _nicknameIF.text);
                PlayerPrefs.SetString("USER_INFO", jsonInfo);
                PlayerPrefs.Save();

                SceneManager.LoadScene("02_Game");
            }
            else
            {
                Debug.Log("유저 정보 로드 실패");
            }
        }
        else
        {
            Debug.Log(www.error);
            _infoMsgText.text = "An error occured while connecting to the server";
            /////////////////////////////////////////////////////////////////////////////////////////////// test
            SceneManager.LoadScene("02_Game");
            /////////////////////////////////////////////////////////////////////////////////////////////// test

        }
    }

    public void OnCreditsButtonClicked()
    {
        _creditsPanel.SetActive(true);
    }

    public void OnCreditsBackButtonClicked()
    {
        _creditsPanel.SetActive(false);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("유니티 에디터에선 안꺼지지롱00;");
        Application.Quit();
    }
}
