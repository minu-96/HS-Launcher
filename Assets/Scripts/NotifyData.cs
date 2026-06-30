using UnityEngine;

public enum NotifyAction
{
    DismissOnly,   // 그냥 사라지기만
    AddGameIcon    // 독바에 게임 아이콘 추가
}

[CreateAssetMenu(menuName = "Notify/NotifyData")]
public class NotifyData : ScriptableObject
{
    public Sprite icon;
    [TextArea] public string message;

    [Header("버튼 동작")]
    public NotifyAction action = NotifyAction.DismissOnly;

    [Header("게임 아이콘 추가용 (AddGameIcon일 때)")]
    public string gameName;       // 실행할 게임 이름
    public Sprite gameIconSprite; // 독바에 넣을 아이콘
}