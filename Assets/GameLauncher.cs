using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    void LaunchGame(string gameName)
    {
        string launcherDir = Directory.GetParent(Application.dataPath).FullName;
        string gamePath;

#if UNITY_STANDALONE_WIN
        gamePath = Path.Combine(launcherDir, "Games", gameName, gameName + ".exe");
        Process.Start(gamePath);

#elif UNITY_STANDALONE_OSX
        string gamesDir = Path.Combine(launcherDir, "..", "Games");
        gamePath = Path.Combine(gamesDir, gameName + ".app");
        Process.Start("open", "\"" + gamePath + "\"");
#endif
    }

    public void OnGame1ButtonClick() => LaunchGame("Game1");
    public void OnGame2ButtonClick() => LaunchGame("Game2");
}