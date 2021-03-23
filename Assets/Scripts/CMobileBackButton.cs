using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMobileBackButton : MonoBehaviour
{
    [SerializeField] private GameObject _pausedCanvas;
    public static bool IsPaused = false;
    private float _timeScaleBeforePause;

    private void Awake()
    {
        IsPaused = false;
        _pausedCanvas.SetActive(false);
    }

    private void Update()
    {
        //#if UNITY_ANDROID
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            if(IsPaused)
            {
                resumeGame();
                ////added for Android back button reaction
                //AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                //activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                pauseGame();
            }
            //Application.Quit();
        }
        //#endif
    }

    private void pauseGame()
    {
        _pausedCanvas.SetActive(true);
        IsPaused = true;
        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        AudioListener.volume = 0.0f;//set volume audio
        Debug.Log("paused..");
    }

    private void resumeGame()
    {
        _pausedCanvas.SetActive(false);
        IsPaused = false;
        Time.timeScale = _timeScaleBeforePause;
        AudioListener.pause = false;
        AudioListener.volume = 1.0f; //set audio volume
        Debug.Log("resumed..");
    }
}
