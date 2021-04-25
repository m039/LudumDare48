using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;
using UnityEngine.SceneManagement;
using System;

public static class SceneTools
{

    public static void LoadScene(MonoBehaviour monoBehaviour, string sceneName)
    {
        IEnumerator loadScene()
        {
            var fader = GameObject.FindObjectOfType<CanvasGroupFader>();
            yield return fader.FadeOut(0);
            SceneManager.LoadScene(sceneName);
        }

        monoBehaviour.StartCoroutine(loadScene());
    }

    public static void AppearScene(MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(GameObject.FindObjectOfType<CanvasGroupFader>().FadeIn(1));
    }
}
