using System.Collections;
using UnityEngine;

public class DockManager : MonoBehaviour
{
    [Header("아이콘 생성")]
    public GameObject iconPrefab;     // GameIcon 프리팹
    public Transform dockContainer;   // 독바 (Horizontal Layout Group)

    [Header("점프 애니메이션")]
    public float jumpHeight = 80f;
    public float jumpDuration = 0.4f;

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
        // 레이아웃이 위치 잡을 때까지 한 프레임 대기
        yield return null;

        Vector2 endPos = rect.anchoredPosition;        // 최종 자리
        Vector2 startPos = endPos - new Vector2(0, jumpHeight); // 아래에서 시작

        rect.anchoredPosition = startPos;

        float time = 0f;
        while (time < jumpDuration)
        {
            time += Time.deltaTime;
            float t = time / jumpDuration;
            // 통통 튀는 느낌 (위로 갔다가 살짝 안착)
            float eased = JumpEase(t);
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            yield return null;
        }
        rect.anchoredPosition = endPos;
    }

    // 점프 이징 (살짝 오버슈트)
    float JumpEase(float t)
    {
        // 끝에서 살짝 튀어오르는 효과
        float c = 1.7f;
        t -= 1f;
        return t * t * ((c + 1f) * t + c) + 1f;
    }
}