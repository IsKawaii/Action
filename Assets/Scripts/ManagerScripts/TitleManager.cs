using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private bool firstPush = false, canGoNextScene = false;
    public FadeImage fade;
    public string nextSceneName; 

    public void PushNewGameButton()
    {
        if (!firstPush)
        {
            firstPush = true;
            canGoNextScene = true;
            fade.StartFadeOut();
        }
    }

    private void Update()
    {
        if (canGoNextScene && fade.IsFadeOutComplete())
        {
            canGoNextScene = false;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
