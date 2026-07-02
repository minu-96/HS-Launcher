using UnityEngine;

// 모든 게임의 정보를 담는 DB. (Create > Launcher > GameDatabase 로 에셋 1개 생성)
// 도크 아이콘은 gameName으로 이 DB를 조회해 아이콘/설명/바 이미지를 스스로 채운다.
[CreateAssetMenu(menuName = "Launcher/GameDatabase")]
public class GameDatabase : ScriptableObject
{
    public GameInfo[] games;

    // 이름으로 게임 정보 찾기 (없으면 null)
    public GameInfo Get(string gameName)
    {
        if (games == null) return null;
        foreach (var g in games)
            if (g != null && g.gameName == gameName) return g;
        return null;
    }
}
