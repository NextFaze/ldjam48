using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public string levelName = "main";
    AsyncOperation loadLevel;

    public GameObject startButton;
    public GameObject progressBarObject;
    public Image progressBar;

    private void Awake()
    {
        progressBarObject.SetActive(false);
    }

    void Update()
    {
        if(Application.platform != RuntimePlatform.WebGLPlayer && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Press the space key to start coroutine
        if (Input.GetButtonDown("Submit") && loadLevel == null)
        {
            startButton.SetActive(false);
            progressBarObject.SetActive(true);

            // Use a coroutine to load the Scene in the background
            StartCoroutine(LoadGameSceneAsync());
        }
    }

    IEnumerator LoadGameSceneAsync()
    {
        loadLevel = SceneManager.LoadSceneAsync(levelName);

        // Wait until the asynchronous scene fully loads
        while (!loadLevel.isDone)
        {
            progressBar.fillAmount = loadLevel.progress;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log($"Done: {loadLevel.progress}");
    }
}
