using System.Collections;
using UnityEngine;

public class DockManager : MonoBehaviour
{
    [Header("아이콘 생성")]
    public GameObject iconPrefab;     // GameIcon 프리팹
    public Transform dockContainer;   // 독바 (Horizontal Layout Group)

    [Header("점프 애니메이션")]
    public float jumpHeight = 80f;
    public float jumpDuration = 0.25f;
    public int jumpCount = 3;          // 튀는 횟수

    // 팝업 버튼이 호출할 함수
    public void AddIcon(string gameName, Sprite icon)
    {
        GameObject newIcon = Instantiate(iconPrefab, dockContainer);

        // 프리팹 버튼을 코드로 연결
        GameIconButton iconBtn = newIcon.GetComponent<GameIconButton>();
        if (iconBtn != null)
            iconBtn.Setup(gameName, icon);

        // 점프 애니메이션 시작
        StartCoroutine(JumpAnimation(newIcon.GetComponent<RectTransform>()));
    }

    IEnumerator JumpAnimation(RectTransform rect)
    {
        // 레이아웃이 자리 잡을 때까지 대기
        yield return null;
        yield return null; // 한 프레임 더 (레이아웃 안정화)

        Vector2 basePos = rect.anchoredPosition; // 독바에서의 제자리

        for (int i = 0; i < jumpCount; i++)
        {
            // 점점 작아지는 높이 (통통 줄어드는 느낌)
            float height = jumpHeight * (1f - (float)i / jumpCount);
            yield return StartCoroutine(SingleJump(rect, basePos, height));
        }

        rect.anchoredPosition = basePos; // 마지막엔 정확히 제자리
    }

    IEnumerator SingleJump(RectTransform rect, Vector2 basePos, float height)
    {
        float time = 0f;
        while (time < jumpDuration)
        {
            time += Time.deltaTime;
            float t = time / jumpDuration;

            // 0 → 1 → 0 포물선 (위로 갔다 내려옴)
            float y = Mathf.Sin(t * Mathf.PI) * height;
            rect.anchoredPosition = basePos + new Vector2(0, y);
            yield return null;
        }
        rect.anchoredPosition = basePos;
    }

}