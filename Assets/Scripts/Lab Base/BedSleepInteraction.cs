using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BedSleepInteraction : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private Behaviour playerMovement;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Bed Position")]
    [SerializeField] private Transform sleepPosition;

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
    private RigidbodyConstraints2D originalConstraints;

    //function: Start
    //purpose: this function hides the interaction prompt and sleeping panel at the beginning
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

        if (playerRigidbody != null)
        {
            originalConstraints = playerRigidbody.constraints;
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
    //purpose: this function displays the sleep prompt when the player enters the interaction range
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
    //purpose: this function hides the sleep prompt when the player leaves the interaction range
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
    //purpose: this function teleports the player to the bed, prevents movement, and advances to the next day
    private IEnumerator Sleep()
    {
        isSleeping = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        if (player != null && sleepPosition != null)
        {
            player.transform.position = new Vector3(
                sleepPosition.position.x,
                sleepPosition.position.y,
                player.transform.position.z
            );
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
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

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isSleeping = false;
    }
}