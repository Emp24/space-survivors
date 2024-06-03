using UnityEngine;

public class InputManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GameManager.instance.ChangeGameState(GameState.Pause);
        }
    }
}
