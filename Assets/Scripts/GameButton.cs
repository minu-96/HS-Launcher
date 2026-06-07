using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject descriptionImage; // 설명 이미지 오브젝트 연결
    public GameObject BarImage; // 설명 이미지 오브젝트 연결

    void Start()
    {
        descriptionImage.SetActive(false); // 시작 시 숨김
        BarImage.SetActive(false); // 시작 시 숨김
    }

    // 마우스 올리면
    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionImage.SetActive(true);
        BarImage.SetActive(true);
    }

    // 마우스 치우면
    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionImage.SetActive(false);
        BarImage.SetActive(false); // 시작 시 숨김
    }
}