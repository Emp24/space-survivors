using TMPro;
using UnityEngine;

public class HUDView : MonoBehaviour
{
    public PlayerController playerController;
    public TMP_Text experience;
    public TMP_Text level;
    public void Awake()
    {
        playerController.onPlayerDataUpdated += UpdateHUD;
    }
    public void UpdateHUD()
    {
        UpdateExperience();
        UpdateLevel();
    }

    public void UpdateExperience()
    {
        experience.text = "Experience: " + playerController.xp.ToString() + "/" + playerController.nextLevelXp.ToString();
    }

    public void UpdateLevel()
    {
        level.text = "Level: " + playerController.playerLevel.ToString();
    }
}
