using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButtonBehavior : MonoBehaviour
{
    public GameObject spotlight;
    public string sceneToLoad; 

    void Start()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(_ => OnHoverEnter());
        interactable.hoverExited.AddListener(_ => OnHoverExit());
        interactable.selectEntered.AddListener(_ => OnClick());
    }

    void OnHoverEnter()
    {
        if (spotlight != null)
            spotlight.SetActive(true);
    }

    void OnHoverExit()
    {
        if (spotlight != null)
            spotlight.SetActive(false);
    }

    void OnClick()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
