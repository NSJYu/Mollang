using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsManager : MonoBehaviour
{
    public static PlayerControls playerControls;

    private void Awake()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }
    }
    private void OnEnable() { playerControls?.Enable(); }
    private void OnDisable() { playerControls?.Disable(); }
}