using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Top-of-screen run info (survival timer + kill counter) and a dash cooldown meter.
public class RunHUDView : MonoBehaviour
{
    private PlayerController player;
    private TMP_Text timerText;
    private TMP_Text killsText;
    private Image dashFill;
    private TMP_Text dashLabel;
    private int lastShownSecond = -1;
    private int lastShownKills = -1;

    public void Initialize(PlayerController player)
    {
        this.player = player;
        Canvas canvas = RuntimeUI.CreateCanvas(transform, "RunHUDCanvas", 40);

        timerText = RuntimeUI.CreateText(canvas.transform, "Timer", "00:00", 56, TextAlignmentOptions.Center);
        SetTopCenter(timerText.rectTransform, -20);
        timerText.fontStyle = FontStyles.Bold;

        killsText = RuntimeUI.CreateText(canvas.transform, "Kills", "Kills: 0", 32, TextAlignmentOptions.Center);
        SetTopCenter(killsText.rectTransform, -85);

        // Dash meter, bottom-center.
        RectTransform dashRoot = RuntimeUI.CreateRect(canvas.transform, "DashMeter", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 40), new Vector2(260, 18));
        Image dashBackground = dashRoot.gameObject.AddComponent<Image>();
        dashBackground.color = new Color(0f, 0f, 0f, 0.55f);

        dashFill = RuntimeUI.CreateImage(dashRoot, "Fill", new Color(0.4f, 0.85f, 1f));
        // Image.fillAmount needs a sprite; scale the fill horizontally from its left edge instead.
        dashFill.rectTransform.pivot = new Vector2(0f, 0.5f);
        RuntimeUI.Stretch(dashFill.rectTransform);

        dashLabel = RuntimeUI.CreateText(dashRoot, "Label", "DASH [SPACE]", 22, TextAlignmentOptions.Center);
        dashLabel.rectTransform.anchoredPosition = new Vector2(0, 30);
    }

    private static void SetTopCenter(RectTransform rect, float y)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, y);
    }

    void Update()
    {
        RunStats stats = RunStats.Instance;
        if (stats == null || player == null) return;

        // Only rebuild strings when the shown value changes, to avoid per-frame garbage.
        int second = Mathf.FloorToInt(stats.TimeSurvived);
        if (second != lastShownSecond)
        {
            lastShownSecond = second;
            timerText.text = RunStats.FormatTime(stats.TimeSurvived);
        }
        if (stats.Kills != lastShownKills)
        {
            lastShownKills = stats.Kills;
            killsText.text = "Kills: " + stats.Kills;
        }

        float charge = player.DashCharge;
        dashFill.rectTransform.localScale = new Vector3(charge, 1f, 1f);
        bool ready = charge >= 1f;
        dashFill.color = ready ? new Color(0.4f, 0.85f, 1f) : new Color(0.3f, 0.4f, 0.55f);
        dashLabel.alpha = ready ? 1f : 0.5f;
    }
}
