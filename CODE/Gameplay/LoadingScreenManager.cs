using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public Image loadingBarFill;

    

    public IEnumerator LoadSceneAsync(string LoadSceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadSceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progress;
            yield return null;
        }
    }
}