using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public DockManager dock;
    public string gameName;    // 이 팝업이 어떤 게임인지
    public Sprite gameIcon;    // 추가할 아이콘 이미지

    // 팝업 버튼 OnClick에 연결
    public void OnAddClick()
    {
        dock.AddIcon(gameName, gameIcon);
    }
}