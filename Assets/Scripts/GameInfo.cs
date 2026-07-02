using UnityEngine;

// 게임 1개의 표시 정보. (Create > Launcher > GameInfo 로 에셋 생성)
// gameName은 다른 시스템(감자 해금, 런처 실행 경로 Games/<gameName>.app,
// NotifyData.gameName)과 정확히 일치해야 한다. 예: MoaMoa / ThePotato / Poootato
[CreateAssetMenu(menuName = "Launcher/GameInfo")]
public class GameInfo : ScriptableObject
{
    public string gameName;          // 식별자 (실행/해금 이름과 동일)
    public Sprite icon;              // 도크 아이콘
    public Sprite descriptionImage;  // 마우스 오버 시 뜨는 설명 이미지
    public Sprite barImage;          // 작은 바 이미지
}
