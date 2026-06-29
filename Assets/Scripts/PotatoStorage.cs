using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotatoType
{
    public string gameName;          // 런처에서 넘기는 이름과 일치 (예: "Game1")
    public GameObject potatoPrefab;  // 그 게임의 감자 프리팹
}

public class PotatoStorage : MonoBehaviour
{
    [Header("게임별 감자 종류")]
    public PotatoType[] potatoTypes;

    [Header("감자 생성 위치 (순서대로 채워짐)")]
    public Transform[] spawnPoints;

    void Start()
    {
        // 게임을 플레이하고 돌아왔는지 확인
        string pending = PlayerPrefs.GetString("pendingGame", "");
        if (pending != "")
        {
            AddPotato(pending);
            PlayerPrefs.SetString("pendingGame", "");
            PlayerPrefs.Save();
        }

        DisplayPotatoes();
    }

    void AddPotato(string gameName)
    {
        List<string> collected = LoadList();

        // 위치 개수를 넘으면 더 이상 추가 안 함
        if (collected.Count >= spawnPoints.Length)
            return;

        collected.Add(gameName);
        SaveList(collected);
    }

    void DisplayPotatoes()
    {
        List<string> collected = LoadList();

        for (int i = 0; i < collected.Count && i < spawnPoints.Length; i++)
        {
            GameObject prefab = GetPrefab(collected[i]);
            if (prefab != null)
            {
                GameObject potato = Instantiate(prefab, spawnPoints[i]);
                potato.transform.localPosition = Vector3.zero; // 위치 마커에 정확히 배치
            }
        }
    }

    GameObject GetPrefab(string gameName)
    {
        foreach (var type in potatoTypes)
        {
            if (type.gameName == gameName)
                return type.potatoPrefab;
        }
        return null;
    }

    List<string> LoadList()
    {
        string data = PlayerPrefs.GetString("potatoList", "");
        if (data == "") return new List<string>();
        return new List<string>(data.Split(','));
    }

    void SaveList(List<string> list)
    {
        PlayerPrefs.SetString("potatoList", string.Join(",", list));
        PlayerPrefs.Save();
    }

    // 전시용 리셋
    public void ResetPotatoes()
    {
        PlayerPrefs.DeleteKey("potatoList");
        PlayerPrefs.Save();
    }
}