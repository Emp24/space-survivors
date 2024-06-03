using UnityEngine;

public class InputManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (GameManager.instance.gameState != GameState.Pause)
            {
                GameManager.instance.ChangeGameState(GameState.Pause);
            }
        }
    }
}
