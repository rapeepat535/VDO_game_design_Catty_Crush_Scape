using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutSceneController : MonoBehaviour
{
    public GameObject videoPlayer;

    void Start()
    {
        videoPlayer.GetComponent<VideoPlayer>().loopPointReached += OnVideoEnded;
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        Debug.Log("Video ended, calling another function...");
        next();
    }

    void OnDestroy()
    {
        videoPlayer.GetComponent<VideoPlayer>().loopPointReached -= OnVideoEnded;
    }

 
    void next()
    {
        SceneManager.LoadScene("CatPlayGround");
    }
}
