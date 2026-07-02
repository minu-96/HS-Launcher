using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyManager : MonoBehaviour
{
    [Header("프리팹 (공통 틀 1개씩)")]
    public GameObject screenPopupPrefab;   // 화면용 팝업
    public GameObject inboxItemPrefab;     // 알림창 항목

    [Header("위치")]
    public Transform screenPopupParent;    // 화면용 뜨는 곳 (우상단)
    public Transform inboxParent;          // 알림창 리스트 부모

    [Header("설정")]
    public float popupStayTime = 2f;       // 화면용 유지 시간

    [Header("독바 연결")]
    public DockManager dockManager;

    [Header("알림 복원용 라이브러리 (모든 NotifyData 에셋)")]
    public NotifyData[] notifyLibrary;     // 껐다 켜도 알림창을 복원할 때 사용

    const string InboxKey = "launcher_inbox"; // 저장된 알림창 목록 (에셋 이름 CSV)

    private Dictionary<string, GameObject> screenPopups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> inboxItems = new Dictionary<string, GameObject>();
    private int idCounter = 0;

    [Header("팝업 스택 (세로 정렬/슬라이드)")]
    public float popupSpacing = 10f;   // 팝업 간 간격
    public float rowHeight = 90f;      // 한 칸 높이
    public float slideOffsetX = 600f;  // 화면 밖(오른쪽) 위치
    public float slideSpeed = 10f;     // 이동/슬라이드 속도

    // 화면 팝업 스택 (index 0 = 맨 위 = 가장 오래된 것)
    class Popup { public string key; public GameObject go; public RectTransform rect; public bool expired; public bool leaving; }
    private List<Popup> activePopups = new List<Popup>();

    void Start()
    {
        RestoreInbox();
    }

    void Update()
    {
        // 맨 위(가장 오래된) 팝업이 만료됐고 아직 안 빠지고 있으면 슬라이드 아웃 시작
        if (activePopups.Count > 0)
    {
        Popup top = activePopups[0];

        if (top.expired && !top.leaving && top.rect != null)
        {
            // 맨 위 슬롯 위치에 거의 도착했을 때만 나가게 함
            bool isAtTopSlot = Vector2.Distance(top.rect.anchoredPosition, Vector2.zero) < 5f;

            if (isAtTopSlot)
                StartCoroutine(LeaveTop(top));
        }
    }

        // 각 팝업을 자기 슬롯(위치)으로 부드럽게 이동 → 위가 빠지면 아래가 올라옴
        for (int i = 0; i < activePopups.Count; i++)
        {
            Popup p = activePopups[i];
            if (p.rect == null) continue;
            float targetY = -i * (rowHeight + popupSpacing);
            float targetX = p.leaving ? slideOffsetX : 0f;
            p.rect.anchoredPosition = Vector2.Lerp(
                p.rect.anchoredPosition, new Vector2(targetX, targetY), Time.deltaTime * slideSpeed);
        }
    }

    // 독바에 아이콘 추가 (알림 버튼이 호출)
    public void AddGameIconToDock(string gameName, Sprite icon)
    {
        // 아이콘/설명/바 이미지는 DockManager가 GameDatabase에서 가져오므로 이름만 넘김
        if (dockManager != null)
            dockManager.AddIcon(gameName);
    }

    // ---------------------------------------------------
    // 알림 띄우기
    // ---------------------------------------------------

    // 고정 알림 (에셋으로) → 화면 팝업은 매번, 알림창은 같은 내용 최대 1개(최신 순 상단)
    public void ShowNotify(NotifyData data)
    {
        if (data == null) return;

        string id = data.name;                 // 에셋 이름을 영구 식별자로 사용

        // 알림창: 같은 내용이 이미 있으면 최상단으로 이동, 없으면 새로 생성
        if (inboxItems.ContainsKey(id) && inboxItems[id] != null)
            inboxItems[id].transform.SetAsFirstSibling();
        else
        {
            SaveToInbox(id);
            CreateInbox(data.icon, data.message, data.action, data.gameName, data.gameIconSprite, id);
        }

        // 화면 팝업: 클리어할 때마다 매번 새로 띄움
        CreateScreenPopup(data.icon, data.message, data.action, data.gameName, data.gameIconSprite);
    }

    // 동적 알림 (그냥 사라지는 기본형, 저장 안 함) → 화면 팝업만
    public void ShowNotify(Sprite icon, string message)
    {
        CreateScreenPopup(icon, message, NotifyAction.DismissOnly, "", null);
    }

    // 저장된 알림창 복원 (화면 팝업 없이 알림창 항목만)
    void RestoreInbox()
    {
        string saved = PlayerPrefs.GetString(InboxKey, "");
        if (string.IsNullOrEmpty(saved)) return;

        foreach (string id in saved.Split(','))
        {
            if (string.IsNullOrEmpty(id)) continue;
            if (inboxItems.ContainsKey(id)) continue;

            NotifyData data = FindNotify(id);
            if (data != null)
                CreateInbox(data.icon, data.message, data.action, data.gameName,
                            data.gameIconSprite, id); // 복원은 화면 팝업 없이
        }
    }

    // 알림창 항목 생성 (최상단에)
    void CreateInbox(Sprite icon, string message, NotifyAction action,
                     string gameName, Sprite gameIcon, string id)
    {
        GameObject inbox = Instantiate(inboxItemPrefab, inboxParent);
        inbox.SetActive(true);
        inbox.transform.SetAsFirstSibling();
        NotifyItem inboxItem = inbox.GetComponent<NotifyItem>();
        if (inboxItem != null)
            inboxItem.Setup(this, id, icon, message, action, gameName, gameIcon);
        inboxItems[id] = inbox;
    }

    // 화면용 팝업 생성 (맨 아래 슬롯의 화면 밖에서 슬라이드 인, 스택에 추가)
    void CreateScreenPopup(Sprite icon, string message, NotifyAction action,
                           string gameName, Sprite gameIcon)
    {
        string popupKey = "popup_" + idCounter++;
        GameObject popup = Instantiate(screenPopupPrefab, screenPopupParent);
        popup.SetActive(true);
        NotifyItem popupItem = popup.GetComponent<NotifyItem>();
        if (popupItem != null)
            popupItem.Setup(this, popupKey, icon, message, action, gameName, gameIcon);
        screenPopups[popupKey] = popup;

        Popup p = new Popup { key = popupKey, go = popup, rect = popup.GetComponent<RectTransform>() };
        // 맨 아래 슬롯의 화면 밖(오른쪽)에서 시작 → Update가 제자리로 슬라이드 인
        if (p.rect != null)
            p.rect.anchoredPosition = new Vector2(slideOffsetX, -activePopups.Count * (rowHeight + popupSpacing));
        activePopups.Add(p);

        StartCoroutine(StayTimer(p));
    }

    // 각 팝업은 자기 타이머로 만료됨 (실제로 빠지는 건 맨 위 슬롯일 때만 → Update에서 처리)
    IEnumerator StayTimer(Popup p)
    {
        yield return new WaitForSeconds(popupStayTime);
        p.expired = true;
    }

    // 맨 위 팝업을 슬라이드 아웃시킨 뒤 제거
    IEnumerator LeaveTop(Popup p)
    {
        p.leaving = true;

        yield return new WaitForSeconds(0.4f);

        activePopups.Remove(p);
        screenPopups.Remove(p.key);

        if (p.go != null)
            Destroy(p.go);

        // 다음 팝업이 올라올 시간 확보
        yield return new WaitForSeconds(0.2f);
    }

    // ---------------------------------------------------
    // 알림 제거 (버튼 클릭 시 화면용 + 알림창 둘 다 + 저장에서도 제거)
    // ---------------------------------------------------
    public void Dismiss(string id)
    {
        if (screenPopups.ContainsKey(id))
        {
            if (screenPopups[id] != null) Destroy(screenPopups[id]);
            screenPopups.Remove(id);
            activePopups.RemoveAll(x => x.key == id);
        }
        if (inboxItems.ContainsKey(id))
        {
            if (inboxItems[id] != null) Destroy(inboxItems[id]);
            inboxItems.Remove(id);
        }
        RemoveFromInbox(id); // 껐다 켜도 지워진 상태 유지
    }

    // 화면용 팝업만 즉시 정리 (알림창을 열 때 호출 → 겹침 방지, 알림창 항목은 유지됨)
    public void ClearScreenPopups()
    {
        foreach (var p in screenPopups.Values)
            if (p != null) Destroy(p);
        screenPopups.Clear();
        activePopups.Clear();
    }

    // 전체 비우기 (전시 리셋용)
    public void DismissAll()
    {
        foreach (var p in screenPopups.Values) if (p != null) Destroy(p);
        foreach (var i in inboxItems.Values) if (i != null) Destroy(i);
        screenPopups.Clear();
        inboxItems.Clear();

        PlayerPrefs.DeleteKey(InboxKey);
        PlayerPrefs.Save();
    }

    // ---------------- 저장 ----------------
    void SaveToInbox(string id)
    {
        string saved = PlayerPrefs.GetString(InboxKey, "");
        if (("," + saved + ",").Contains("," + id + ",")) return; // 중복 방지
        saved = string.IsNullOrEmpty(saved) ? id : saved + "," + id;
        PlayerPrefs.SetString(InboxKey, saved);
        PlayerPrefs.Save();
    }

    void RemoveFromInbox(string id)
    {
        string saved = PlayerPrefs.GetString(InboxKey, "");
        if (string.IsNullOrEmpty(saved)) return;

        List<string> list = new List<string>(saved.Split(','));
        list.Remove(id);
        PlayerPrefs.SetString(InboxKey, string.Join(",", list));
        PlayerPrefs.Save();
    }

    NotifyData FindNotify(string id)
    {
        if (notifyLibrary != null)
            foreach (var n in notifyLibrary)
                if (n != null && n.name == id) return n;
        return null;
    }
}
