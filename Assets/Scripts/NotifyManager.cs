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

    // 상단 필드에 추가
    [Header("독바 연결")]
    public DockManager dockManager;

    // 독바에 아이콘 추가 (알림 버튼이 호출)
    public void AddGameIconToDock(string gameName, Sprite icon)
    {
        if (dockManager != null)
            dockManager.AddIcon(gameName, icon);
    }

    private Dictionary<string, GameObject> screenPopups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> inboxItems = new Dictionary<string, GameObject>();
    private int idCounter = 0;

    // ---------------------------------------------------
    // 알림 띄우기
    // ---------------------------------------------------

    // 고정 알림 (에셋으로)
    // 고정 알림 (에셋으로) - 동작 정보 포함
    public void ShowNotify(NotifyData data)
    {
        if (data != null)
            ShowNotifyInternal(data.icon, data.message,
                            data.action, data.gameName, data.gameIconSprite);
    }

    // 동적 알림 (그냥 사라지는 기본형)
    public void ShowNotify(Sprite icon, string message)
    {
        ShowNotifyInternal(icon, message, NotifyAction.DismissOnly, "", null);
    }

    // 실제 생성 (공통)
    void ShowNotifyInternal(Sprite icon, string message,
                            NotifyAction action, string gameName, Sprite gameIcon)
    {
        string id = "notify_" + idCounter++;

        GameObject inbox = Instantiate(inboxItemPrefab, inboxParent);
        inbox.SetActive(true);
        inbox.transform.SetAsFirstSibling();
        NotifyItem inboxItem = inbox.GetComponent<NotifyItem>();
        if (inboxItem != null)
            inboxItem.Setup(this, id, icon, message, action, gameName, gameIcon);
        inboxItems[id] = inbox;

        GameObject popup = Instantiate(screenPopupPrefab, screenPopupParent);
        popup.SetActive(true);
        NotifyItem popupItem = popup.GetComponent<NotifyItem>();
        if (popupItem != null)
            popupItem.Setup(this, id, icon, message, action, gameName, gameIcon);
        screenPopups[id] = popup;

        UIPanel ui = popup.GetComponent<UIPanel>();
        if (ui != null) ui.SlideInRight();
        StartCoroutine(AutoHideScreen(id));
    }

    // 화면용만 자동으로 사라짐 (알림창 항목은 남음)
    IEnumerator AutoHideScreen(string id)
    {
        yield return new WaitForSeconds(popupStayTime);

        if (screenPopups.ContainsKey(id) && screenPopups[id] != null)
        {
            UIPanel ui = screenPopups[id].GetComponent<UIPanel>();
            if (ui != null) ui.SlideOutRight();
            else screenPopups[id].SetActive(false);
        }
    }

    // ---------------------------------------------------
    // 알림 제거 (버튼 클릭 시 화면용 + 알림창 둘 다)
    // ---------------------------------------------------
    public void Dismiss(string id)
    {
        if (screenPopups.ContainsKey(id))
        {
            if (screenPopups[id] != null) Destroy(screenPopups[id]);
            screenPopups.Remove(id);
        }
        if (inboxItems.ContainsKey(id))
        {
            if (inboxItems[id] != null) Destroy(inboxItems[id]);
            inboxItems.Remove(id);
        }
    }

    // 전체 비우기 (전시 리셋용)
    public void DismissAll()
    {
        foreach (var p in screenPopups.Values) if (p != null) Destroy(p);
        foreach (var i in inboxItems.Values) if (i != null) Destroy(i);
        screenPopups.Clear();
        inboxItems.Clear();
    }
}