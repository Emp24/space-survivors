using UnityEngine;

public class InputManager : MonoBehaviour
{
    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // Only pause from active gameplay, so Esc can't open the pause menu over the power draft or game over screen.
        GameState state = GameManager.instance.gameState;
        if (state == GameState.Playing || state == GameState.StartGame)
        {
            GameManager.instance.ChangeGameState(GameState.Pause);
        }
    }
}
