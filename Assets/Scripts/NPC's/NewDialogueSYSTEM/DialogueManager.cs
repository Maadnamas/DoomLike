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

    // Variables para recordar los datos del NPC actual
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

    // --- AQUÍ ESTÁ EL CAMBIO PRINCIPAL ---

    // 1. Este es el método público que llamas desde Actor.cs
    // Ahora recibe TODA la caja de datos (el ScriptableObject)
    public void StartDialogue(Dialogue dialogueData)
    {
        ShowDialogue();

        // Extraemos los datos del ScriptableObject y los guardamos en memoria
        currentTitle = dialogueData.npcName; // <-- Aquí tomamos el nombre nuevo
        currentFont = dialogueData.npcFont;
        portraitOpen = dialogueData.npcPortraitOpen;
        portraitClosed = dialogueData.npcPortraitClosed;

        // Configuramos la UI inicial
        DialogTitleText.text = currentTitle;
        if (currentFont != null) DialogBodyText.font = currentFont;

        // Mostramos el primer nodo (el RootNode)
        DisplayNode(dialogueData.RootNode);
    }

    // 2. Este método privado se encarga de actualizar el texto y los botones
    // Ya no necesita pedir título ni fotos porque usa las variables guardadas arriba
    private void DisplayNode(DialogueNode node)
    {
        // Actualizar texto del cuerpo
        DialogBodyText.text = node.dialogueText;

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

        // Limpiar botones viejos
        foreach (Transform child in responseButtonContainer) Destroy(child.gameObject);
        currentResponses.Clear();

        // Crear botones nuevos
        if (node.responses.Count > 0)
        {
            int index = 0;
            foreach (DialogueResponse response in node.responses)
            {
                GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{index + 1}. {response.responseText}";

                // Opcional: Aplicar la fuente también a los botones
                if (currentFont != null) buttonObj.GetComponentInChildren<TextMeshProUGUI>().font = currentFont;

                currentResponses.Add(response);
                buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
                index++;
            }
        }
        else
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "1. [Fin]";
            if (currentFont != null) buttonObj.GetComponentInChildren<TextMeshProUGUI>().font = currentFont;

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
            DisplayNode(response.nextNode); // <-- Ahora llamamos a DisplayNode internamente
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