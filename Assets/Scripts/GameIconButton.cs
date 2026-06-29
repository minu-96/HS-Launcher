using UnityEngine;
using UnityEngine.UI;

public class GameIconButton : MonoBehaviour
{
    public Button launchButton;
    public Image iconImage;

    private string gameName;

    // 생성될 때 어떤 게임인지 설정받음
    public void Setup(string name, Sprite icon)
    {
        gameName = name;
        if (iconImage != null) iconImage.sprite = icon;

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