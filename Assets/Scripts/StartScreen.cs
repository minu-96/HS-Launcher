using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreen : MonoBehaviour
{
    public CanvasGroup startPanel;
    public float slideDuration = 0.5f;
    private bool isSliding = false;

    [Header("시작 알림 (처음 한 번만)")]
    public NotifyManager notifyManager;
    public NotifyData welcomeNotify;   // 시작 시 띄울 알림 에셋
    private bool notifyShown = false;

    public void OnDesktopOut(InputAction.CallbackContext context)
    {
        if (context.performed && !isSliding)
        {
            StartCoroutine(SlideOut());
        }
    }

    IEnumerator SlideOut()
    {
        isSliding = true;
        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            startPanel.alpha = Mathf.Lerp(1f, 0f, time / slideDuration);
            yield return null;
        }
        startPanel.alpha = 0f;
        startPanel.gameObject.SetActive(false);

        // 처음 넘어갈 때만 시작 알림 띄우기
        if (!notifyShown)
        {
            notifyShown = true;
            if (notifyManager != null && welcomeNotify != null)
                notifyManager.ShowNotify(welcomeNotify);
        }
    }

    public void OnDesktop()
    {
        if (isSliding)
        {
            startPanel.alpha = 1f;
            startPanel.gameObject.SetActive(true);
        }
        isSliding = false;
    }
}