using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class PickPowerMenuView : MonoBehaviour
{
    public GameObject pickPowerMenuPanel;
    public Action onPickPower;
    public PlayerController player;
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
        Time.timeScale = 0;
        pickPowerMenuPanel.gameObject.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 0;
        pickPowerMenuPanel.gameObject.SetActive(false);
    }
    public void HandlePickPower(int powerId)
    {
        player.ApplyPower(powerId);
        onPickPower?.Invoke();
    }
    public void CloseMenu()
    {
        pickPowerMenuPanel.SetActive(false);
        GameManager.instance.ChangeGameState(GameState.Playing);
    }
    public void SetUpButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            pickPowerMenuPanel.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => HandlePickPower(index));
        }
    }

}

