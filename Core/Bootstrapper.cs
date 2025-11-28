using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Bootstrapper : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private EventSystem eventSystemPrefab;
    [SerializeField] private InputManager inputManagerPrefab;
    [SerializeField] private GameManager gameManagerPrefab;


    [Header("Configuration")]
    [SerializeField] private bool loadMainMenu = true;


    public static Bootstrapper Instance { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        InitializeGame();
    }


    public void InitializeGame()
    {
        Debug.Log("Initializing game systems...");

        EnsureCoreSystems();
        RegisterServices();


        if (loadMainMenu)
        {
            SceneLoader.Instance.LoadScene("MainMenu");
        }

        Debug.Log("Game initialization complete");
    }


    private void EnsureCoreSystems()
    {
            // EVENT SYSTEM
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            if (eventSystemPrefab != null)
            {
                // Instantiate *your* custom event system
                Instantiate(eventSystemPrefab, transform);
            }
            else
            {
                GameObject es = new GameObject("EventSystem_UI"); // Renamed for clarity
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<StandaloneInputModule>(); // StandaloneInputModule is now recognized
                es.transform.SetParent(transform);
            }
        }


            // INPUT MANAGER
        if (InputManager.Instance == null)
        {
            if (inputManagerPrefab != null)
            {
                Instantiate(inputManagerPrefab, transform);
            }
            else
            {
                GameObject im = new GameObject("InputManager");
                im.AddComponent<InputManager>();
                im.transform.SetParent(transform);
            }
        }


            // GAME MANAGER
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab, transform);
            }
            else
            {
                GameObject gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
                gm.transform.SetParent(transform);
            }
        }


            // SAVE MANAGER
        if (SaveManager.Instance == null)
        {
            GameObject sm = new GameObject("SaveManager");
            sm.AddComponent<SaveManager>();
            sm.transform.SetParent(transform);
        }


            // SCENE LOADER
        if (SceneLoader.Instance == null)
        {
            GameObject sl = new GameObject("SceneLoader");
            sl.AddComponent<SceneLoader>();
            sl.transform.SetParent(transform);
        }
    }


    private void RegisterServices()
    {
        ServiceLocator.Instance.RegisterService(UnityEngine.EventSystems.EventSystem.current);

        ServiceLocator.Instance.RegisterService(InputManager.Instance);
        ServiceLocator.Instance.RegisterService(GameManager.Instance);
        ServiceLocator.Instance.RegisterService(SaveManager.Instance);
        ServiceLocator.Instance.RegisterService(SceneLoader.Instance);
    }
}
