using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Referencias UI")]
    public GameObject DialogueParent;
    public TextMeshProUGUI DialogTitleText;
    public TextMeshProUGUI DialogBodyText;
    public Image NpcPortraitUI;
    public Transform responseButtonContainer;
    public GameObject responseButtonPrefab;

    [Header("Ajustes de Animación")]
    public float animationSpeed = 0.2f; // Tiempo entre frames

    private List<DialogueResponse> currentResponses = new List<DialogueResponse>();
    private string currentTitle;
    private TMP_FontAsset currentFont;

    private Sprite portraitClosed;
    private Sprite portraitOpen;
    private Coroutine talkingCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogue();
    }

    private void Update()
    {
        if (!IsDialogueActive()) return;

        if (ScreenManager.Instance != null && ScreenManager.Instance.IsGamePaused()) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectResponseByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectResponseByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectResponseByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectResponseByIndex(3);
    }

    private void LateUpdate()
    {
        if (IsDialogueActive())
        {
            bool isMenuPaused = (ScreenManager.Instance != null && ScreenManager.Instance.IsGamePaused());

            if (!isMenuPaused)
            {
                if (Time.timeScale != 0f) Time.timeScale = 0f;
            }

            if (PlayerMovement.isControlEnabled) PlayerMovement.isControlEnabled = false;
            if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
            if (!Cursor.visible) Cursor.visible = true;
        }
    }

    public void StartDialogue(string title, DialogueNode node, TMP_FontAsset font = null, Sprite open = null, Sprite closed = null)
    {
        ShowDialogue();
        currentTitle = title;
        currentFont = font;
        portraitOpen = open;
        portraitClosed = closed;

        DialogTitleText.text = title;
        DialogBodyText.text = node.dialogueText;

        if (currentFont != null) DialogBodyText.font = currentFont;

        // Gestión de Retrato y Animación
        if (NpcPortraitUI != null)
        {
            if (portraitOpen != null && portraitClosed != null)
            {
                NpcPortraitUI.gameObject.SetActive(true);
                // Reiniciar corrutina de habla
                if (talkingCoroutine != null) StopCoroutine(talkingCoroutine);
                talkingCoroutine = StartCoroutine(AnimateTalking());
            }
            else if (portraitClosed != null) // Si solo hay una, se queda fija
            {
                NpcPortraitUI.sprite = portraitClosed;
                NpcPortraitUI.gameObject.SetActive(true);
            }
            else
            {
                NpcPortraitUI.gameObject.SetActive(false);
            }
        }

        foreach (Transform child in responseButtonContainer) Destroy(child.gameObject);
        currentResponses.Clear();

        if (node.responses.Count > 0)
        {
            int index = 0;
            foreach (DialogueResponse response in node.responses)
            {
                GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{index + 1}. {response.responseText}";
                currentResponses.Add(response);
                buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
                index++;
            }
        }
        else
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "1. [Fin]";
            buttonObj.GetComponent<Button>().onClick.AddListener(() => HideDialogue());
            currentResponses.Add(null);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator AnimateTalking()
    {
        while (true)
        {
            NpcPortraitUI.sprite = portraitOpen;
            yield return new WaitForSecondsRealtime(animationSpeed);
            NpcPortraitUI.sprite = portraitClosed;
            yield return new WaitForSecondsRealtime(animationSpeed);
        }
    }

    public void SelectResponse(DialogueResponse response)
    {
        if (response.nextNode != null)
            StartDialogue(currentTitle, response.nextNode, currentFont, portraitOpen, portraitClosed);
        else
            HideDialogue();
    }

    private void SelectResponseByIndex(int index)
    {
        if (index >= 0 && index < currentResponses.Count)
        {
            if (currentResponses[index] != null) SelectResponse(currentResponses[index]);
            else HideDialogue();
        }
    }

    private void ShowDialogue() => DialogueParent.SetActive(true);

    public void HideDialogue()
    {
        if (talkingCoroutine != null) StopCoroutine(talkingCoroutine);
        DialogueParent.SetActive(false);

        if (ScreenManager.Instance == null || !ScreenManager.Instance.IsGamePaused())
        {
            Time.timeScale = 1f;
            PlayerMovement.isControlEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public bool IsDialogueActive() => DialogueParent.activeSelf;
}