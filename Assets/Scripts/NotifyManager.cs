using UnityEngine;

public class NotifyManager : MonoBehaviour
{
    [Header("팝업 항목들")]
    public GameObject screenPopup;    // 화면용 팝업 (잠깐 뜨는 것)
    public GameObject inboxPopup;     // 알림창 안의 팝업 항목

    // 버튼(화면용이든 알림창이든)에서 호출
    public void DismissPopup()
{
    // 화면용은 슬라이드로 사라지게
    UIPanel screenUI = screenPopup.GetComponent<UIPanel>();
    if (screenUI != null) screenUI.SlideOutRight();
    else if (screenPopup != null) screenPopup.SetActive(false);

    // 알림창 항목은 즉시 제거
    if (inboxPopup != null) inboxPopup.SetActive(false);
}
}