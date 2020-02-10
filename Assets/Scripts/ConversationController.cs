﻿using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationController : MonoBehaviour {
    public static ConversationController Instance;

    public bool IsFinishedConversing { get; private set; }

    [SerializeField]
    private GameObject DialogueOverlay;
    [SerializeField]
    private TextMeshProUGUI Title;
    [SerializeField]
    private TextMeshProUGUI Prompt;
    [SerializeField]
    private Image Portrait;
    [SerializeField]
    private Transform ButtonPanel;
    [SerializeField]
    private GameObject PrefabButton;

    private Conversation Manager;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start() {
        DialogueOverlay.SetActive(false);
    }

    public void SetDialogueVisibility(bool isDialogueVisible) {
        DialogueOverlay.SetActive(isDialogueVisible);
    }

    private void UpdateDialogue(ConversationNodeId id) {
        if (id == null) {
            IsFinishedConversing = true;
            SetDialogueVisibility(false);
            Manager.SetActiveNode(Manager.GreetingNode.Id);
        } else {
            IsFinishedConversing = false;
            Manager.SetActiveNode(id);
        }

        UpdateDialogue();
        UpdateOptions();
    }

    private void UpdateDialogue() {
        string titleText = Manager.GetTitle();
        string portraitPath = Manager.GetPortraitPath();
        string promptText = Manager.GetPrompt();

        Title.text = titleText;
        Prompt.text = promptText;
        if (portraitPath != null) {
            Portrait.sprite = Resources.Load<Sprite>(PrunePath(portraitPath));
        }
    }

    private string PrunePath(string path) {
        string pattern = "(?<=Resources\\/).*(?=\\.(png|jpg|jpeg))";
        return Regex.Match(path, pattern).Value;
    }

    private void UpdateOptions() {
        foreach (Transform child in ButtonPanel) {
            Destroy(child.gameObject);
        }

        ConversationNode node = Manager.ActiveNode;
        for (int i = 0; i < node.Options.Count; i++) {
            ConversationOption option = node.Options[i];
            CreateButton(option).transform.SetParent(ButtonPanel, false);
        }
    }

    private GameObject CreateButton(ConversationOption option) {
        GameObject buttonObject = Instantiate(PrefabButton);

        Button buttonComponent = buttonObject.GetComponent<Button>();
        buttonComponent.GetComponentInChildren<TMP_Text>().text = option.Response;
        buttonComponent.onClick.AddListener(delegate {
            SelectOption(option.DestinationId);
        });

        return buttonObject;
    }

    private void SelectOption(ConversationNodeId id) {
        Manager.SetActiveNode(id);
        UpdateDialogue(id);
    }

    public void SetDialogueManager(Conversation manager) {
        if (manager != null) {
            this.Manager = manager;
            UpdateDialogue(manager.ActiveNode.Id);
        }
    }
}
