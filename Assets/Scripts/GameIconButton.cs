using UnityEngine;
using UnityEngine.UI;

public class GameIconButton : MonoBehaviour
{
    public Button launchButton;
    public Image iconImage;
    public Image descriptionImage;  // 설명 이미지 (GameButton이 hover로 켜고 끔)
    public Image barImage;          // 작은 바 이미지

    private string gameName;

    // 게임 정보(DB)로 아이콘/설명/바를 채움
    public void Setup(string name, GameInfo info)
    {
        gameName = name;

        if (info != null)
        {
            if (iconImage != null && info.icon != null) iconImage.sprite = info.icon;
            if (descriptionImage != null && info.descriptionImage != null)
                descriptionImage.sprite = info.descriptionImage;
            if (barImage != null && info.barImage != null)
                barImage.sprite = info.barImage;
        }

        // 버튼 클릭 시 게임 실행 연결 (코드로)
        launchButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // 런처의 게임 실행 함수 호출
        GameLauncher launcher = FindObjectOfType<GameLauncher>();
        if (launcher != null)
            launcher.LaunchByName(gameName);
    }
}
