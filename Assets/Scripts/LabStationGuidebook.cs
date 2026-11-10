using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LabDeskGuidebook : MonoBehaviour
{
    [SerializeField] private GameObject guidebookPanel;
    [SerializeField] private TMP_Text guidebookText;
    [SerializeField] private TMP_Text interactionPrompt;

    private bool playerInRange;

    //function: Start
    //purpose: this function hides the guidebook and interaction prompt at the beginning
    private void Start()
    {
        if (guidebookPanel != null)
        {
            guidebookPanel.SetActive(false);
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
            interactionPrompt.text = "Press E to view tutorial";
        }

        if (guidebookText != null)
        {
            guidebookText.text =
                "WELCOME TO UNDEAD HOMESTEAD\n\n" +
                "The world has been overrun by zombies. " +
                "Your goal is to survive, rebuild your home, " +
                "and discover what caused the outbreak.\n\n" +
                "OBJECTIVES\n\n" +
                "1. Explore the surrounding areas.\n" +
                "2. Gather food, water, and supplies.\n" +
                "3. Grow crops at your home base.\n" +
                "4. Craft tools and improve your buildings.\n" +
                "5. Find and cure zombies using antidotes.\n" +
                "6. Recruit cured zombies to help your homestead.";
        }
    }

    //function: Update
    //purpose: this function opens or closes the guidebook when the player presses E
    private void Update()
    {
        if (playerInRange &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleGuidebook();
        }
    }

    //function: ToggleGuidebook
    //purpose: this function shows or hides the guidebook panel
    private void ToggleGuidebook()
    {
        if (guidebookPanel == null)
        {
            return;
        }

        bool guidebookIsOpen = guidebookPanel.activeSelf;

        guidebookPanel.SetActive(!guidebookIsOpen);

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(guidebookIsOpen);
        }
    }

    //function: OnTriggerEnter2D
    //purpose: this function shows the interaction prompt when the player enters the range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
            }
        }
    }

    //function: OnTriggerExit2D
    //purpose: this function hides the prompt and closes the guidebook when the player leaves
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }

            if (guidebookPanel != null)
            {
                guidebookPanel.SetActive(false);
            }
        }
    }
}