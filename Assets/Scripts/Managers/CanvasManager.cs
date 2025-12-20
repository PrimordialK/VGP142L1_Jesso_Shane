using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public AudioClip pauseSound;
    private AudioSource audioSource;
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button backButton;
    public Button quitButton;
    public Button creditsButton;
    public Button resumeGame;
    public Button returnToMenu;


    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;
    public GameObject creditsPanel;
    public GameObject VictoryPanel;

   

    [SerializeField] private AudioSource musicSource; // Assign in Inspector or create in code
    [SerializeField] private AudioClip backgroundMusic;

    private bool isPaused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (resumeGame) resumeGame.onClick.AddListener(() =>
        {
            isPaused = false;
            Time.timeScale = 1f;
            SetMenus(null, pauseMenuPanel);
            musicSource?.UnPause();
        });

        if (playButton) playButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f; // Unpause the game
            SceneManager.LoadScene(1);
        });
        if (creditsButton) creditsButton.onClick.AddListener(() => SetMenus(creditsPanel, mainMenuPanel));
        if (settingsButton) settingsButton.onClick.AddListener(() => SetMenus(settingsPanel, mainMenuPanel));
        if (backButton) backButton.onClick.AddListener(() =>
        {
            SetMenus(mainMenuPanel, settingsPanel);
            SetMenus(pauseMenuPanel, settingsPanel);
            SetMenus(mainMenuPanel, creditsPanel);
            SetMenus(mainMenuPanel, creditsPanel);
        });
        if (settingsButton) settingsButton.onClick.AddListener(() => SetMenus(settingsPanel, pauseMenuPanel));
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (resumeGame) resumeGame.onClick.AddListener(() =>
        {
            isPaused = false;
            Time.timeScale = 1f;
            SetMenus(null, pauseMenuPanel);
        });

        if (returnToMenu) returnToMenu.onClick.AddListener(() => SceneManager.LoadScene(0));


        if (audioSource == null)
        {
            TryGetComponent(out audioSource);
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        
    }





    void SetMenus(GameObject menuToActivate, GameObject menuToDeactivate)
    {
        if (menuToActivate) menuToActivate.SetActive(true);
        if (menuToDeactivate) menuToDeactivate.SetActive(false);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
      

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                SetMenus(pauseMenuPanel, null);
                Time.timeScale = 0f;
                musicSource?.Pause(); // Pause only music
            }
            else
            {
                SetMenus(null, pauseMenuPanel);
                Time.timeScale = 1f;
                musicSource.mute = false; // Unmute music
            }
            audioSource?.PlayOneShot(pauseSound); // Play SFX
        }
    }
}
