using System.Collections;
using UnityEngine;

// 감자 한 개의 등장 연출.
// 화면 위에서 떨어져 바닥 높이(groundY)에서 멈추고, 바닥 위 랜덤 좌표(targetX)로 굴러간다.
// 좌표는 모두 부모(감자 생성 위치) 기준 로컬 좌표다.
public class PotatoDrop : MonoBehaviour
{
    [Header("속도")]
    public float fallSpeed = 10f;    // 낙하 속도 (units/sec)
    public float rollSpeed = 4f;     // 구르는 속도 (units/sec)
    public float spinPerUnit = 540f; // 1유닛 구를 때 회전 각도(도)

    // 위에서 떨어져 바닥에 도달 후 targetX로 굴러감 (새로 얻은 감자)
    public void Drop(float startX, float startY, float groundY, float targetX)
    {
        transform.localPosition = new Vector3(startX, startY, 0f);
        StopAllCoroutines();
        StartCoroutine(DropRoutine(groundY, targetX));
    }

    // 애니메이션 없이 바닥에 바로 배치 (이미 보유한 감자)
    public void Settle(float x, float groundY)
    {
        transform.localPosition = new Vector3(x, groundY, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }

    IEnumerator DropRoutine(float groundY, float targetX)
    {
        // 1) 낙하
        while (transform.localPosition.y > groundY)
        {
            Vector3 p = transform.localPosition;
            p.y = Mathf.MoveTowards(p.y, groundY, fallSpeed * Time.deltaTime);
            transform.localPosition = p;
            yield return null;
        }

        // 2) 바닥 위 targetX로 구르기 (이동 방향으로 회전)
        float dir = Mathf.Sign(targetX - transform.localPosition.x);
        while (Mathf.Abs(targetX - transform.localPosition.x) > 0.01f)
        {
            float step = rollSpeed * Time.deltaTime;
            Vector3 p = transform.localPosition;
            p.x = Mathf.MoveTowards(p.x, targetX, step);
            transform.localPosition = p;
            transform.Rotate(0f, 0f, -dir * spinPerUnit * step);
            yield return null;
        }
    }
}
