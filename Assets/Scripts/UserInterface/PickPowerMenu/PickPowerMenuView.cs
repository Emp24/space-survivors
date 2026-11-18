using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// On level-up, drafts random upgrades from UpgradeLibrary onto the menu's buttons.
public class PickPowerMenuView : MonoBehaviour
{
    public GameObject pickPowerMenuPanel;
    public Action onPickPower;
    public PlayerController player;

    private readonly List<Button> buttons = new List<Button>();
    private readonly List<TMP_Text> labels = new List<TMP_Text>();
    private List<Upgrade> currentDraft = new List<Upgrade>();

    void Awake()
    {
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;
        onPickPower += CloseMenu;
        SetUpButtons();
        pickPowerMenuPanel.SetActive(false);
    }

    void OnDestroy()
    {
        GameManager.instance.OnGameStateChanged -= HandleGameStateChanged;
        onPickPower -= CloseMenu;
    }

    public void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.PowerMenu:
                PauseGame();
                break;
            case GameState.Playing:
                ResumeGame();
                break;
        }
    }

    public void PauseGame()
    {
        if (!PopulateDraft())
        {
            // Everything is maxed out: nothing to pick, just keep playing.
            GameManager.instance.ChangeGameState(GameState.Playing);
            return;
        }
        Time.timeScale = 0;
        pickPowerMenuPanel.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pickPowerMenuPanel.gameObject.SetActive(false);
    }

    // Fills the buttons with a fresh draft. Returns false if there is nothing left to offer.
    private bool PopulateDraft()
    {
        currentDraft = UpgradeLibrary.Draft(player, buttons.Count);
        if (currentDraft.Count == 0) return false;

        for (int i = 0; i < buttons.Count; i++)
        {
            bool hasUpgrade = i < currentDraft.Count;
            buttons[i].gameObject.SetActive(hasUpgrade);
            if (!hasUpgrade || labels[i] == null) continue;

            Upgrade upgrade = currentDraft[i];
            int stacks = player.GetUpgradeStacks(upgrade.Id);
            string level = upgrade.MaxStacks == int.MaxValue
                ? (stacks > 0 ? $"  <size=70%>Lv {stacks + 1}</size>" : "")
                : $"  <size=70%>{stacks + 1}/{upgrade.MaxStacks}</size>";
            labels[i].text = $"<b>{upgrade.Title}</b>{level}\n<size=75%>{upgrade.Description}</size>";
        }
        return true;
    }

    public void HandlePickPower(int index)
    {
        if (index >= currentDraft.Count) return;
        player.ApplyUpgrade(currentDraft[index]);
        onPickPower?.Invoke();
    }

    public void CloseMenu()
    {
        pickPowerMenuPanel.SetActive(false);
        GameManager.instance.ChangeGameState(GameState.Playing);
    }

    public void SetUpButtons()
    {
        foreach (Transform child in pickPowerMenuPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button == null) continue;

            int index = buttons.Count;
            button.onClick.AddListener(() => HandlePickPower(index));
            buttons.Add(button);

            TMP_Text label = child.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                // Upgrade text is two lines; let it shrink to fit but never grow past the authored size.
                label.richText = true;
                label.fontSizeMax = label.fontSize;
                label.fontSizeMin = label.fontSize * 0.5f;
                label.enableAutoSizing = true;
            }
            labels.Add(label);
        }
    }
}
