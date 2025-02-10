using UnityEngine;
using UnityEngine.UI;

public class SlideTransition : MonoBehaviour
{
    public Animator animator; // Reference to Animator
    public Button triggerButton;
    public GameObject backgroundPanel; // Background panel to toggle
    private bool isVisible = false;

    void Start()
    {
        if (triggerButton == null)
        {
            Debug.LogError("⚠️ Button is not assigned in the Inspector!");
            return;
        }

        // Ensure animation starts in the idle state
        animator.Play("IdlePanel");

        triggerButton.onClick.AddListener(ToggleSlide);
    }

    public void ToggleSlide()
    {
        isVisible = !isVisible;
        animator.SetBool("IsVisible", isVisible); // 🔹 Control animation

        // 🔹 Toggle the background panel based on visibility
        backgroundPanel.SetActive(isVisible);
    }
}