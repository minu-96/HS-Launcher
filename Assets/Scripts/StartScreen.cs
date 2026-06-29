using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartScreen : MonoBehaviour
{
    public CanvasGroup startPanel;
    public float slideDuration = 0.5f;
    private bool isSliding = false;

    [Header("팝업 (처음 한 번만)")]
    public UIPanel notifyPopup;        // 화면에 잠깐 뜨는 팝업
    public float popupStayTime = 2f;   // 떠있는 시간
    private bool popupShown = false;

    public void OnDesktopOut(InputAction.CallbackContext context)
    {
        if (context.performed && !isSliding)
            StartCoroutine(SlideOut());
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

        if (!popupShown)
        {
            popupShown = true;
            StartCoroutine(PopupSequence());
        }
    }

    IEnumerator PopupSequence()
    {
        if (notifyPopup == null) yield break;

        notifyPopup.SlideInRight();              // 우측에서 등장
        yield return new WaitForSeconds(popupStayTime);  // 2초 대기
        notifyPopup.SlideOutRight();             // 우측으로 사라짐
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