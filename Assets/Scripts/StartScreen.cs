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
    public NotifyData[] welcomeNotifies;   // 시작 시 띄울 알림 에셋들 (Poootato, MoaMoa 등)

    // 저장 키 (런처 전용)
    const string OpenKey = "launcher_startScreenOpen"; // 1=열림, 0=닫힘
    const string WelcomeKey = "launcher_welcomeShown"; // 0/1

    void Start()
    {
        // 저장된 열림/닫힘 상태 복원 (기본값 = 열림)
        bool open = PlayerPrefs.GetInt(OpenKey, 1) == 1;
        startPanel.alpha = open ? 1f : 0f;
        startPanel.gameObject.SetActive(open);
    }

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

        // 닫힘 상태 저장
        PlayerPrefs.SetInt(OpenKey, 0);
        PlayerPrefs.Save();

        // 첫 팝업: 평생 한 번만 (등록된 시작 알림 전부 표시)
        if (PlayerPrefs.GetInt(WelcomeKey, 0) == 0)
        {
            PlayerPrefs.SetInt(WelcomeKey, 1);
            PlayerPrefs.Save();
            if (notifyManager != null && welcomeNotifies != null)
                foreach (var n in welcomeNotifies)
                    if (n != null) notifyManager.ShowNotify(n);
        }
    }

    public void OnDesktop()
    {
        // 진행 중인 슬라이드(닫힘 애니메이션)가 있으면 중단하고 즉시 열기
        StopAllCoroutines();
        isSliding = false;

        startPanel.alpha = 1f;
        startPanel.gameObject.SetActive(true);

        // 열림 상태 저장
        PlayerPrefs.SetInt(OpenKey, 1);
        PlayerPrefs.Save();
    }
}
