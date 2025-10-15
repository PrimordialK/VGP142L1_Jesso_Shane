using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    public GameObject menuUI;
    public Button yesBtn;
    public Button noBtn;

    void Awake()
    {
        // Assign button listeners
        if (yesBtn != null)
            yesBtn.onClick.AddListener(OnYesClicked);
        if (noBtn != null)
            noBtn.onClick.AddListener(OnNoClicked);
    }

    public void ShowMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(true);
        }

        // If you have base.Init and state logic, keep it as needed
        // base.Init(currentContext);
        // state = MenuStates.Victory;
    }

    private void OnYesClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnNoClicked()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
        // Optionally, resume game logic here if you paused the game
        // Time.timeScale = 1f;
    }
}