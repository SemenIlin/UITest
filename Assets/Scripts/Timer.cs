using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimeInSeconds;
    [SerializeField] private TextMeshProUGUI TimerText;

    private int hours;
    private int minuts;
    private int seconds;
    private void Update()
    {
        TimerText.text = GetTicTimerText(Time.deltaTime);
    }

    private string GetTicTimerText(float deltaTime)
    {
        TimeInSeconds -= deltaTime;
        hours = (int)Mathf.Floor(TimeInSeconds / 3600.0f);
        minuts = (int)Mathf.Floor(TimeInSeconds / 60.0f);
        seconds = (int)Mathf.Floor(TimeInSeconds % 60.0f);
        return string.Format("{0:d2}:{1:d2}:{2:d2}", hours, minuts, seconds);
    }
}
