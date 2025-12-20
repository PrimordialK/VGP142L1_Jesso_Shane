using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;


    public override void Init(MenuController currentContext)
    {
        base.Init(currentContext);
        state = MenuStates.MainMenu;

        if (playButton) playButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        if (settingsButton) settingsButton.onClick.AddListener(() => JumpTo(MenuStates.Settings));
        if (creditsButton) creditsButton.onClick.AddListener(() => JumpTo(MenuStates.Credits));
        if (quitButton) quitButton.onClick.AddListener(() => QuitGame());
    }
}