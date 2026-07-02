using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [Header("로딩 화면 오브젝트")]
    public GameObject loadingScreen;

    private Process currentGameProcess;

    public void OnGame1ButtonClick() => StartCoroutine(LaunchGame("MoaMoa"));
    public void OnGame2ButtonClick() => StartCoroutine(LaunchGame("ThePotato"));
    public void OnGame3ButtonClick() => StartCoroutine(LaunchGame("Poootato"));

    IEnumerator LaunchGame(string gameName)
    {
        // 1. 로딩 화면 표시
        loadingScreen.SetActive(true);

        // 2. 경로 설정
        string launcherDir = Directory.GetParent(Application.dataPath).FullName;
        string gamePath = "";

#if UNITY_STANDALONE_WIN
        gamePath = Path.Combine(launcherDir, "Games", gameName, gameName + ".exe");
#elif UNITY_STANDALONE_OSX
        string gamesDir = Path.Combine(launcherDir, "..", "Games");
        gamePath = Path.GetFullPath(Path.Combine(gamesDir, gameName + ".app"));
#endif

        // 3. 게임 프로세스 실행
        currentGameProcess = new Process();

#if UNITY_STANDALONE_WIN
        currentGameProcess.StartInfo.FileName = gamePath;
#elif UNITY_STANDALONE_OSX
        currentGameProcess.StartInfo.FileName = "open";
        currentGameProcess.StartInfo.Arguments = "-W \"" + gamePath + "\"";
#endif

        currentGameProcess.StartInfo.UseShellExecute = true;
        currentGameProcess.EnableRaisingEvents = true;
        currentGameProcess.Start();

        // 4. 게임 창이 뜰 때까지 대기
        yield return new WaitForSeconds(2f);

        // 5. 로딩 화면 끄고 런처 종료
        loadingScreen.SetActive(false);

        // 어떤 게임을 플레이했는지 이름 저장
        PlayerPrefs.SetString("pendingGame", gameName);
        PlayerPrefs.Save();

        Application.Quit();
    }

    public void LaunchByName(string gameName)
{
    StartCoroutine(LaunchGame(gameName));
}

}