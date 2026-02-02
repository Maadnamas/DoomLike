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
    public Transform responseButtonContainer;
    public GameObject responseButtonPrefab;

    private List<DialogueResponse> currentResponses = new List<DialogueResponse>();
    private string currentTitle;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogue();
    }

    private void Update()
    {
        if (!IsDialogueActive()) return;

        // VERIFICACIÓN DE PAUSA
        // Si ScreenManager ya tiene la función, la usamos aquí.
        if (ScreenManager.Instance != null && ScreenManager.Instance.IsGamePaused())
        {
            return; // Si está en pausa, ignoramos los números
        }

        // Inputs
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectResponseByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectResponseByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectResponseByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectResponseByIndex(3);
    }

    private void LateUpdate()
    {
        if (IsDialogueActive())
        {
            // Verificamos pausa para no pelear con el TimeScale
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

    public void StartDialogue(string title, DialogueNode node)
    {
        ShowDialogue();
        currentTitle = title;
        DialogTitleText.text = title;
        DialogBodyText.text = node.dialogueText;

        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
        currentResponses.Clear();

        if (node.responses.Count > 0)
        {
            int index = 0;
            foreach (DialogueResponse response in node.responses)
            {
                GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
                string numberedText = $"{index + 1}. {response.responseText}";
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = numberedText;

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

    private void SelectResponseByIndex(int index)
    {
        if (index >= 0 && index < currentResponses.Count)
        {
            DialogueResponse response = currentResponses[index];
            if (response != null) SelectResponse(response);
            else HideDialogue();
        }
    }

    public void SelectResponse(DialogueResponse response)
    {
        if (response.nextNode != null)
        {
            StartDialogue(currentTitle, response.nextNode);
        }
        else
        {
            HideDialogue();
        }
    }

    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }

    public void HideDialogue()
    {
        DialogueParent.SetActive(false);

        // Al cerrar, verificamos que NO esté en pausa antes de devolver control
        if (ScreenManager.Instance == null || !ScreenManager.Instance.IsGamePaused())
        {
            Time.timeScale = 1f;
            PlayerMovement.isControlEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}