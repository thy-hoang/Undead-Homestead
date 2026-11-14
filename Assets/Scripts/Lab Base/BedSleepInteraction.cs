using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BedSleepInteraction : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Behaviour playerMovement;

    [Header("Day and Night")]
    [SerializeField] private DayNightCycle dayNightCycle;

    [Header("Interaction Prompt")]
    [SerializeField] private TMP_Text interactionPrompt;

    [Header("Sleeping Panel")]
    [SerializeField] private GameObject sleepingPanel;

    [Header("Sleep Settings")]
    [SerializeField] private float sleepDuration = 2f;

    private bool playerIsNearby;
    private bool isSleeping;

    //function: Start
    //purpose: this function hides the interaction prompt and sleeping panel when the game begins
    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        if (sleepingPanel != null)
        {
            sleepingPanel.SetActive(false);
        }
    }

    //function: Update
    //purpose: this function checks whether the player pressed E near the bed
    private void Update()
    {
        if (!playerIsNearby || isSleeping)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(Sleep());
        }
    }

    //function: OnTriggerEnter2D
    //purpose: this function displays the interaction prompt when the player enters the bed range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerIsNearby = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.text = "Press E to sleep";
            interactionPrompt.gameObject.SetActive(true);
        }
    }

    //function: OnTriggerExit2D
    //purpose: this function hides the interaction prompt when the player leaves the bed range
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerIsNearby = false;

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    //function: Sleep
    //purpose: this function shows the sleeping panel, advances the day, and resets the time
    private IEnumerator Sleep()
    {
        isSleeping = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (sleepingPanel != null)
        {
            sleepingPanel.SetActive(true);
        }

        yield return new WaitForSeconds(sleepDuration);

        if (dayNightCycle != null)
        {
            dayNightCycle.StartNewDay();
        }

        yield return new WaitForSeconds(0.5f);

        if (sleepingPanel != null)
        {
            sleepingPanel.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isSleeping = false;
    }
}