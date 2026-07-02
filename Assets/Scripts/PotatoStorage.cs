using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PotatoType
{
    public string gameName;          // "MoaMoa", "ThePotato", "Poootato"
    public GameObject potatoPrefab;  // 해당 게임의 감자 프리팹

    [Header("바닥 범위 (생성 위치 기준 로컬 좌표)")]
    public float groundWidth = 3f;   // 바닥 가로 폭 (감자가 굴러갈 범위: ±groundWidth/2)
    public float groundY = 0f;       // 감자가 멈추는 바닥 높이
    public float dropHeight = 6f;    // 위에서 떨어지기 시작하는 높이
}

public class PotatoStorage : MonoBehaviour
{
    [Header("게임별 감자 종류")]
    public PotatoType[] potatoTypes;

    [Header("감자 생성 위치 (게임별 고정)")]
    public Transform[] spawnPoints;  // potatoTypes와 같은 순서/개수

    [Header("알림 연동")]
    public NotifyManager notifyManager;
    public NotifyData[] unlockNotifies; // 게임별 해금 알림 (potatoTypes 순서와 동일)

    // 게임별로 이미 화면에 표시(연출 완료)한 감자 개수 (런처 로컬 저장)
    const string ShownKeyPrefix = "shown_potato_";

    void Start()
    {
        RefreshPotatoes();
    }

    // 공유 저장소의 누적 개수를 읽어 감자를 배치한다.
    // 기존 보유분은 즉시 배치, 새로 얻은 분은 낙하 연출 + 해금 알림.
    void RefreshPotatoes()
    {
        for (int i = 0; i < potatoTypes.Length && i < spawnPoints.Length; i++)
        {
            string gameName = potatoTypes[i].gameName;
            string shownKey = ShownKeyPrefix + gameName;

            int total = SharedUnlockStore.GetPotatoCount(gameName);
            int shown = PlayerPrefs.GetInt(shownKey, 0);
            if (shown > total) shown = total; // 리셋 등으로 개수가 줄면 보정

            // 이미 보유하던 감자: 애니메이션 없이 바닥에 배치
            for (int k = 0; k < shown; k++)
                SpawnPotato(i, animated: false);

            // 새로 얻은 감자: 위에서 떨어지는 연출
            int newly = total - shown;
            for (int k = 0; k < newly; k++)
                SpawnPotato(i, animated: true);

            // 새로 얻은 게 있으면 해금 알림 (게임당 1번)
            if (newly > 0) ShowUnlockNotify(i);

            PlayerPrefs.SetInt(shownKey, total);
        }
        PlayerPrefs.Save();
    }

    void SpawnPotato(int i, bool animated)
    {
        PotatoType type = potatoTypes[i];
        if (type.potatoPrefab == null) return;

        GameObject go = Instantiate(type.potatoPrefab, spawnPoints[i]);
        PotatoDrop drop = go.GetComponent<PotatoDrop>();
        if (drop == null) drop = go.AddComponent<PotatoDrop>();

        float half = type.groundWidth * 0.5f;
        float targetX = Random.Range(-half, half);

        if (animated)
        {
            float startX = Random.Range(-half, half);
            drop.Drop(startX, type.dropHeight, type.groundY, targetX);
        }
        else
        {
            drop.Settle(targetX, type.groundY);
        }
    }

    void ShowUnlockNotify(int i)
    {
        if (notifyManager != null && i < unlockNotifies.Length && unlockNotifies[i] != null)
            notifyManager.ShowNotify(unlockNotifies[i]);
    }

    // 전시용 리셋 (감자만)
    public void ResetAllPotatoes()
    {
        SharedUnlockStore.ResetAll();

        // 표시 카운트도 초기화
        foreach (var t in potatoTypes)
            PlayerPrefs.DeleteKey(ShownKeyPrefix + t.gameName);
        PlayerPrefs.Save();

        // 화면의 감자도 제거
        foreach (var point in spawnPoints)
            foreach (Transform child in point)
                Destroy(child.gameObject);
    }

    // 전시용 전체 리셋: 저장된 모든 것(감자 해금, 도크 아이콘, 스타트스크린/첫팝업 상태 등)을 초기화
    public void ResetEverything()
    {
        // 런처의 모든 PlayerPrefs (도크 아이콘, 스타트스크린 열림상태, 첫팝업 표시여부, 감자 표시 카운트 등)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 스네이크와 공유하는 해금 파일
        SharedUnlockStore.ResetAll();

        // 깨끗한 처음 상태로 씬을 다시 로드 → 모든 화면 요소가 초기 상태로 재구성됨
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
