using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public enum TimeOfDay
    {
        Day,
        Night
    }

    [Header("Normal Schedule")]
    [SerializeField] private float normalPhaseLengthMinutes = 10f;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = true;

    [Tooltip("1x = 10 minutes, 10x = 1 minute, 2x = 5 minutes")]
    [Min(0.01f)]
    [SerializeField] private float timeMultiplier = 1f;

    [Header("Night Visual")]
    [SerializeField] private CanvasGroup nightOverlay;

    [Header("Clock UI")]
    [SerializeField] private TMP_Text timeText;

    private TimeOfDay currentTime = TimeOfDay.Day;
    private float timePassed;

    //function: Start
    //purpose: this function sets the starting time and updates the game visuals and clock
    private void Start()
    {
        currentTime = TimeOfDay.Day;
        timePassed = 0f;

        UpdateVisuals();
        UpdateTimeText();
    }

    //function: Update
    //purpose: this function advances time, switches between day and night, and updates the clock
    private void Update()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= GetCurrentPhaseLengthSeconds())
        {
            SwitchTimeOfDay();
        }

        UpdateTimeText();
    }

    //function: GetCurrentPhaseLengthSeconds
    //purpose: this function returns the current day or night length in seconds after applying the time multiplier
    private float GetCurrentPhaseLengthSeconds()
    {
        float phaseLengthMinutes = normalPhaseLengthMinutes;

        if (debugMode)
        {
            phaseLengthMinutes /= timeMultiplier;
        }

        return phaseLengthMinutes * 60f;
    }

    //function: SwitchTimeOfDay
    //purpose: this function changes the game from day to night or night to day
    private void SwitchTimeOfDay()
    {
        timePassed = 0f;

        if (currentTime == TimeOfDay.Day)
        {
            currentTime = TimeOfDay.Night;
            Debug.Log("Night has started.");
        }
        else
        {
            currentTime = TimeOfDay.Day;
            Debug.Log("Day has started.");
        }

        UpdateVisuals();
    }

    //function: UpdateVisuals
    //purpose: this function darkens the screen during nighttime
    private void UpdateVisuals()
    {
        if (nightOverlay == null)
        {
            return;
        }

        if (currentTime == TimeOfDay.Night)
        {
            nightOverlay.alpha = 0.45f;
        }
        else
        {
            nightOverlay.alpha = 0f;
        }
    }

    //function: UpdateTimeText
    //purpose: this function converts the compressed schedule into a 24-hour clock
    private void UpdateTimeText()
    {
        if (timeText == null)
        {
            return;
        }

        float phaseProgress = timePassed / GetCurrentPhaseLengthSeconds();

        // Each phase represents twelve in-game hours.
        int minutesIntoPhase = Mathf.FloorToInt(phaseProgress * 720f);

        int totalMinutes;

        if (currentTime == TimeOfDay.Day)
        {
            // Day begins at 6:00 AM.
            totalMinutes = 6 * 60 + minutesIntoPhase;
        }
        else
        {
            // Night begins at 6:00 PM.
            totalMinutes = 18 * 60 + minutesIntoPhase;

            if (totalMinutes >= 1440)
            {
                totalMinutes -= 1440;
            }
        }

        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;

        string period = hour >= 12 ? "PM" : "AM";

        int displayHour = hour % 12;

        if (displayHour == 0)
        {
            displayHour = 12;
        }

        timeText.text = string.Format(
            "{0}:{1:00} {2}",
            displayHour,
            minute,
            period
        );
    }
}