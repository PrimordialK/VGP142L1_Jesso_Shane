using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMenu : BaseMenu
{
    public Button backButton;
    public RectTransform creditsText; // Assign your text's RectTransform in the Inspector
    public float moveSpeed = 50f;     // Units per second

    private Coroutine moveCoroutine;

    public override void Init(MenuController currentContext)
    {
        base.Init(currentContext);
        state = MenuStates.Credits;

        if (backButton) backButton.onClick.AddListener(() => JumpBack());
    }

    private void OnEnable()
    {
        if (creditsText != null)
        {
            moveCoroutine = StartCoroutine(MoveTextUpwards());
        }
    }

    private void OnDisable()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private System.Collections.IEnumerator MoveTextUpwards()
    {
        while (true)
        {
            creditsText.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
