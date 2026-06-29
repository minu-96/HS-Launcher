using UnityEngine;
using TMPro;
using System;

public class ClockUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text dateText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateClock();
        InvokeRepeating(nameof(UpdateClock), 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateClock()
    {
        DateTime now = DateTime.Now;

        timeText.text = now.ToString("tt h:mm");
        dateText.text = now.ToString("yyyy-MM-dd");
    }
}
