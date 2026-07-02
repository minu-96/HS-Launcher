using System.Collections;
using UnityEngine;

public class DockManager : MonoBehaviour
{
    [Header("아이콘 생성")]
    public GameObject iconPrefab;     // GameIcon 프리팹
    public Transform dockContainer;   // 독바 (Horizontal Layout Group)

    [Header("게임 정보 DB (아이콘/설명/바 이미지)")]
    public GameDatabase gameDatabase; // gameName으로 게임 정보를 조회

    [Header("점프 애니메이션")]
    public float jumpHeight = 80f;
    public float jumpDuration = 0.25f;
    public int jumpCount = 3;          // 튀는 횟수

    const string DockKey = "launcher_dockIcons"; // 저장된 아이콘 목록 (CSV)

    void Start()
    {
        RestoreSavedIcons();
    }

    // 저장된 아이콘들을 복원 (애니메이션 없이)
    void RestoreSavedIcons()
    {
        string saved = PlayerPrefs.GetString(DockKey, "");
        if (string.IsNullOrEmpty(saved)) return;

        foreach (string name in saved.Split(','))
        {
            if (string.IsNullOrEmpty(name)) continue;
            CreateIcon(name, false);
        }
    }

    // 팝업 버튼이 호출할 함수 (새로 추가 → 저장 + 애니메이션)
    public void AddIcon(string gameName)
    {
        if (IsSaved(gameName)) return;   // 이미 있으면 중복 생성 안 함
        SaveIcon(gameName);
        CreateIcon(gameName, true);
    }

    // 실제 아이콘 오브젝트 생성 (공통) — 아이콘/설명/바는 DB에서 조회해 채움
    void CreateIcon(string gameName, bool animate)
    {
        GameObject newIcon = Instantiate(iconPrefab, dockContainer);

        GameInfo info = (gameDatabase != null) ? gameDatabase.Get(gameName) : null;

        GameIconButton iconBtn = newIcon.GetComponent<GameIconButton>();
        if (iconBtn != null)
            iconBtn.Setup(gameName, info);

        if (animate)
            StartCoroutine(JumpAnimation(newIcon.GetComponent<RectTransform>()));
    }

    // ---------------- 저장 ----------------
    bool IsSaved(string gameName)
    {
        string saved = PlayerPrefs.GetString(DockKey, "");
        return ("," + saved + ",").Contains("," + gameName + ",");
    }

    void SaveIcon(string gameName)
    {
        string saved = PlayerPrefs.GetString(DockKey, "");
        saved = string.IsNullOrEmpty(saved) ? gameName : saved + "," + gameName;
        PlayerPrefs.SetString(DockKey, saved);
        PlayerPrefs.Save();
    }

    // 전시 리셋용: 저장 + 화면 아이콘 모두 제거
    public void ClearAllIcons()
    {
        PlayerPrefs.DeleteKey(DockKey);
        PlayerPrefs.Save();
        foreach (Transform child in dockContainer)
            Destroy(child.gameObject);
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
