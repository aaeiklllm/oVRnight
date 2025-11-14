
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Menu : MonoBehaviour
{
    public GameObject menuCanvas;
    public InputActionProperty menuAction;
    private bool xPressedLastFrame = false;
    private bool yPressedLastFrame = false;

    private bool isMenuOpen = false;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (menuAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        if (isMenuOpen)
        {
            UnityEngine.XR.InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            if (leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool yButton))
            {
                if (yButton && !yPressedLastFrame)
                {
                    RestartScene(); // Y button
                }
                yPressedLastFrame = yButton;
            }

            if (leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool xButton))
            {
                if (xButton && !xPressedLastFrame)
                {
                    ExitGame(); // X button
                }
                xPressedLastFrame = xButton;
            }
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuCanvas.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu"); 
    }
}
