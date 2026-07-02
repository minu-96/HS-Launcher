using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public DockManager dock;
    public string gameName;    // 이 팝업이 어떤 게임인지 (아이콘/설명/바는 DB에서 조회)

    // 팝업 버튼 OnClick에 연결
    public void OnAddClick()
    {
        dock.AddIcon(gameName);
    }
}