using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public float loadingProgress { get; private set; }
    public bool isLoading { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);


            // Register with Service Locator
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }


    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;
        loadingProgress = 0f;

            // Notify systems that a scene load is starting
        EventSystem.Instance.Publish(GameEventType.SceneLoadStarted, sceneName);


            // Set game state to loading
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentState = GameState.Loading;
        }


            // Explicitly use UnityEngine.AsyncOperation to avoid ambiguity
        UnityEngine.AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;


            // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            loadingProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);


            if (asyncLoad.progress >= 0.9f)
            {
                    // Wait for final approval to activate the scene
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }


        loadingProgress = 1f;
        isLoading = false;


            // Notify systems that the scene has loaded
        EventSystem.Instance.Publish(GameEventType.SceneLoaded, sceneName);


            // Return to appropriate game state
        if (GameManager.Instance != null)
        {
            if (sceneName == "MainMenu")
            {
                GameManager.Instance.CurrentState = GameState.MainMenu;
            }
            else
            {
                GameManager.Instance.CurrentState = GameState.Play;
            }
        }
    }
}