using UnityEngine;
using UnityEngine.UI;

public class NotifyItem : MonoBehaviour
{
    [Header("UI 요소")]
    public Image iconImage;
    public Text messageText;
    public Button dismissButton;

    [HideInInspector] public NotifyManager manager;
    [HideInInspector] public string notifyId;

    // 이 알림이 가진 동작 정보
    private NotifyAction action;
    private string gameName;
    private Sprite gameIcon;

    public void Setup(NotifyManager mgr, string id, Sprite icon, string message,
                      NotifyAction act, string game, Sprite gameSprite)
    {
        manager = mgr;
        notifyId = id;
        action = act;
        gameName = game;
        gameIcon = gameSprite;

        if (iconImage != null) iconImage.sprite = icon;
        if (messageText != null) messageText.text = message;

        if (dismissButton != null)
        {
            dismissButton.onClick.RemoveAllListeners();
            dismissButton.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        // 게임 아이콘 추가 동작이면 독바에 추가
        if (action == NotifyAction.AddGameIcon && manager != null)
            manager.AddGameIconToDock(gameName, gameIcon);

        // 어떤 경우든 알림은 사라짐
        if (manager != null)
            manager.Dismiss(notifyId);
    }
}