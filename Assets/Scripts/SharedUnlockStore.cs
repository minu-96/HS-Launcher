using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

// 스네이크와 런처(서로 다른 Unity 앱)가 함께 읽고 쓰는 공유 저장소.
// PlayerPrefs는 앱(회사명+제품명)마다 분리되므로 앱 경계를 넘어 공유되지 않는다.
// 그래서 두 앱이 합의된 절대 경로의 JSON 파일을 통해 해금 정보를 주고받는다.
// 이 파일은 SnakeGame과 HS-Launcher 양쪽 프로젝트에 동일하게 들어가야 한다.
//
// 감자는 "중복 획득"이 가능하므로 게임별 누적 개수(count)를 저장한다.
// unlocked 리스트는 최초 해금 여부(기존 호환)를 위해 유지한다.
public static class SharedUnlockStore
{
    [Serializable]
    class Data
    {
        public List<string> unlocked = new List<string>();   // 최초 해금 여부 (호환 유지)
        public List<string> countGames = new List<string>(); // 감자 개수 트래킹 게임명
        public List<int> countValues = new List<int>();      // 위와 병렬, 누적 감자 개수
    }

    // 두 앱이 동일하게 계산하는 공유 경로 (사용자 단위 고정 위치)
    static string Dir =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HS_Games");
    static string FilePath => Path.Combine(Dir, "unlocks.json");

    static Data Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonUtility.FromJson<Data>(File.ReadAllText(FilePath)) ?? new Data();
        }
        catch (Exception e) { Debug.LogWarning($"[SharedUnlockStore] 읽기 실패: {e.Message}"); }
        return new Data();
    }

    static void Save(Data data)
    {
        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(FilePath, JsonUtility.ToJson(data));
        }
        catch (Exception e) { Debug.LogWarning($"[SharedUnlockStore] 쓰기 실패: {e.Message}"); }
    }

    static int IndexOf(Data data, string gameName) => data.countGames.IndexOf(gameName);

    // 게임 조건 달성 시 호출 → 감자 +1 (중복 획득 가능)
    public static void Unlock(string gameName)
    {
        if (string.IsNullOrEmpty(gameName)) return;

        Data data = Load();
        if (!data.unlocked.Contains(gameName)) data.unlocked.Add(gameName);

        int idx = IndexOf(data, gameName);
        if (idx < 0) { data.countGames.Add(gameName); data.countValues.Add(1); }
        else data.countValues[idx]++;

        Save(data);
        Debug.Log($"[SharedUnlockStore] {gameName} 감자 +1 (총 {GetPotatoCount(gameName)}개) ({FilePath})");
    }

    // 누적 감자 개수. (구버전 파일 호환: count가 없고 unlocked만 있으면 1개로 간주)
    public static int GetPotatoCount(string gameName)
    {
        Data data = Load();
        int idx = IndexOf(data, gameName);
        if (idx >= 0) return data.countValues[idx];
        return data.unlocked.Contains(gameName) ? 1 : 0;
    }

    public static bool IsUnlocked(string gameName) => GetPotatoCount(gameName) > 0;

    // 전시용 전체 리셋
    public static void ResetAll() => Save(new Data());
}
