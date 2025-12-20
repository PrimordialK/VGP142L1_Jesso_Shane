using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    public AudioClip deathSound;
    private AudioSource audioSource;


    public delegate void PlayerSpawnDelegate(PlayerController playerInstance);
    public event PlayerSpawnDelegate OnPlayerControllerCreated;

    #region Player Controller Information
    public PlayerController playerPrefab;
    private PlayerController _playerInstance;
    public PlayerController playerInstance => _playerInstance;
    private Vector3 currentCheckpoint;
    #endregion

    public event Action<int> OnLivesChanged;

    #region Stats
    private int _lives = 3;
    private int _score = 0;

    public int score
    {
        get => _score;
        set
        {
            if (value < 0)
                _score = 0;
            else
                _score = value;
        }
    }
    public int lives
    {
        get => _lives;
        set
        {
            if (value < 0)
            {
                Debug.Log("Game Over! You have no lives left.");
                GameOver();
                _lives = 0;
            }
            else if (value < _lives)
            {
                // Play death sound from GameManager's AudioSource
                if (deathSound != null && audioSource != null)
                    audioSource.PlayOneShot(deathSound);

                Debug.Log("Lost a life ");
                Respawn();
                _lives = value;
            }
            else if (value > maxLives)
            {
                _lives = maxLives;
            }
            else
            {
                _lives = value;
            }
            Debug.Log($"Lives: {_lives}");
            OnLivesChanged?.Invoke(_lives);
        }
    }



    public int maxLives = 9;
    #endregion

    #region Singleton Pattern
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure AudioSource exists and is routed to SFX mixer group
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            if (sfxMixerGroup != null)
                audioSource.outputAudioMixerGroup = sfxMixerGroup;
            return;
        }
        Destroy(gameObject);
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void GameOver()
    {
        SceneManager.LoadScene(3);
    }

    void Respawn()
    {
        if (_playerInstance != null)
        {

            Destroy(_playerInstance.gameObject);
        }
        StartCoroutine(RespawnAfterDelay(4f));
    }

    private System.Collections.IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _playerInstance = Instantiate(playerPrefab, currentCheckpoint, Quaternion.identity);
        OnPlayerControllerCreated?.Invoke(_playerInstance);
    }

    public void StartLevel(Vector3 startPosition)
    {
        currentCheckpoint = startPosition;

        // Prevent duplicate player instances
        if (_playerInstance != null)
        {
            Destroy(_playerInstance.gameObject);
        }

        _playerInstance = Instantiate(playerPrefab, currentCheckpoint, Quaternion.identity);
        OnPlayerControllerCreated?.Invoke(_playerInstance);
    }

    // Update is called once per frame
    void Update()
    {


        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    int buildIndex = SceneManager.GetActiveScene().buildIndex;
        //    if (buildIndex == 0)
        //    {
        //        SceneManager.LoadScene(1);
        //    }
        //    else
        //    {
        //        SceneManager.LoadScene(0);
        //    }
        //}
    }
    public void StartGame() => SceneManager.LoadScene(1);

    // Add this in your GameManager class

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            Time.timeScale = 1f;


        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   

    // C# property to retrieve save/load manager
    public static LoadSaveManager StateManager
    {
        get
        {
            if (!statemanager)
                statemanager = instance.GetComponent<LoadSaveManager>();

            if (!statemanager)
                statemanager = instance.gameObject.AddComponent<LoadSaveManager>();

            return statemanager;
        }
    }

    // Internal reference to single active instance of object - for singleton behaviour
    private static GameManager instance = null;

    // Internal reference to Saveload Game Manager
    private static LoadSaveManager statemanager = null;

    // Should load from save game state on level load, or just restart level from defaults
    private static bool bShouldLoad = false;

    // Called before Start on object creation
    //void Awake()
    //{
    //    //Check if there is an existing instance of this object
    //    if ((instance) && (instance.GetInstanceID() != GetInstanceID()))
    //        Destroy(gameObject); //Delete duplicate
    //    else
    //    {
    //        instance = this; //Make this object the only instance
    //        DontDestroyOnLoad(gameObject); //Set as do not destroy
    //    }
    //}

    // Restart Game
    public void RestartGame()
    {
        // Load first level
        SceneManager.LoadScene(1);
    }

    // Exit Game
    public void ExitGame()
    {
        Application.Quit();
    }

    // Save Game
    public void SaveGame()
    {
        // Print the path where the XML is save
        Debug.Log(Application.persistentDataPath);

        // Call save game functionality
        StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");
    }

    // Load Game
    public void LoadGame()
    {
        StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");
    }
}