using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : MonoBehaviour
{
    public PlayerController playerController;
    public TMP_Text experience;
    public TMP_Text level;
    public Image healthBar;
    public void Awake()
    {
        playerController.onPlayerDataUpdated += UpdateHUD;
    }
    public void UpdateHUD()
    {
        UpdateExperience();
        UpdateLevel();
        UpdateHealth();
    }

    public void UpdateExperience()
    {
        experience.text = "Experience: " + playerController.xp.ToString() + "/" + playerController.nextLevelXp.ToString();
    }

    public void UpdateLevel()
    {
        level.text = "Level: " + playerController.playerLevel.ToString();
    }
    public void UpdateHealth()
    {
        healthBar.fillAmount = playerController.health / 100f;
    }
}
