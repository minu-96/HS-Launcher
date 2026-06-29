using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIPanel : MonoBehaviour
{
    [Header("기본 설정")]
    public float duration = 0.2f;
    public float slideDistance = 500f;

    private CanvasGroup cg;
    private RectTransform rect;
    private Coroutine current;
    private Vector2 homePos;
    private bool initialized = false;

    private bool isOpen = false;   // 패널이 열려있는지 자체 관리

    void Awake()
{
    EnsureInit();
    isOpen = gameObject.activeSelf;  // 시작 상태에 맞춰 초기화
}

    void EnsureInit()
    {
        if (initialized) return;
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        homePos = rect.anchoredPosition;
        initialized = true;
    }

    // ---------- 페이드 ----------
    public void FadeIn()  { EnsureInit(); gameObject.SetActive(true); Run(FadeRoutine(1f, true)); }
    public void FadeOut() { EnsureInit(); Run(FadeRoutine(0f, false)); }

    // ---------- 슬라이드 인 ----------
    public void SlideInLeft()  { EnsureInit(); gameObject.SetActive(true); Run(SlideInRoutine(new Vector2(-slideDistance, 0))); }
    public void SlideInRight() { EnsureInit(); gameObject.SetActive(true); Run(SlideInRoutine(new Vector2(slideDistance, 0))); }
    public void SlideInUp()    { EnsureInit(); gameObject.SetActive(true); Run(SlideInRoutine(new Vector2(0, slideDistance))); }
    public void SlideInDown()  { EnsureInit(); gameObject.SetActive(true); Run(SlideInRoutine(new Vector2(0, -slideDistance))); }

    // ---------- 슬라이드 아웃 ----------
    public void SlideOutLeft()  { EnsureInit(); Run(SlideOutRoutine(new Vector2(-slideDistance, 0))); }
    public void SlideOutRight() { EnsureInit(); Run(SlideOutRoutine(new Vector2(slideDistance, 0))); }
    public void SlideOutUp()    { EnsureInit(); Run(SlideOutRoutine(new Vector2(0, slideDistance))); }
    public void SlideOutDown()  { EnsureInit(); Run(SlideOutRoutine(new Vector2(0, -slideDistance))); }

    // ---------- 단순 표시/숨김 ----------
    public void Show() { EnsureInit(); gameObject.SetActive(true); cg.alpha = 1f; }
    public void Hide() { gameObject.SetActive(false); }

    // ---------- 토글 (같은 쪽으로 들어오고 나감) ----------
    // ---------- 토글 (자체 플래그로 판정) ----------
    public void ToggleFade()
    {
        EnsureInit();
        if (isOpen) FadeOut();
        else FadeIn();
        isOpen = !isOpen;
    }
    public void ToggleSlideLeft()
    {
        EnsureInit();
        if (isOpen) SlideOutLeft();
        else SlideInLeft();
        isOpen = !isOpen;
    }
    public void ToggleSlideRight()
    {
        EnsureInit();
        if (isOpen) SlideOutRight();
        else SlideInRight();
        isOpen = !isOpen;
    }
    public void ToggleSlideUp()
    {
        EnsureInit();
        if (isOpen) SlideOutUp();
        else SlideInUp();
        isOpen = !isOpen;
    }
    public void ToggleSlideDown()
    {
        EnsureInit();
        if (isOpen) SlideOutDown();
        else SlideInDown();
        isOpen = !isOpen;
    }

    // ---------- 내부 코루틴 ----------
    IEnumerator FadeRoutine(float to, bool activeAtStart)
    {
        if (activeAtStart) gameObject.SetActive(true);
        float from = cg.alpha;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, Ease(time / duration));
            yield return null;
        }
        cg.alpha = to;
        if (to == 0f) gameObject.SetActive(false);
    }

    // 바깥(homePos + offset)에서 제자리(homePos)로 들어옴
    IEnumerator SlideInRoutine(Vector2 offset)
    {
        gameObject.SetActive(true);
        rect.anchoredPosition = homePos + offset;
        yield return Move(homePos);
    }

    // 제자리(homePos)에서 바깥(homePos + offset)으로 나감
    IEnumerator SlideOutRoutine(Vector2 offset)
    {
        rect.anchoredPosition = homePos;        // 항상 제자리에서 출발
        yield return Move(homePos + offset);
        gameObject.SetActive(false);
        rect.anchoredPosition = homePos;        // 다음 등장 위해 복구
    }

    IEnumerator Move(Vector2 to)
    {
        Vector2 from = rect.anchoredPosition;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(from, to, Ease(time / duration));
            yield return null;
        }
        rect.anchoredPosition = to;
    }

    void Run(IEnumerator routine)
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(routine);
    }

    float Ease(float t) => 1f - Mathf.Pow(1f - t, 3f);
}