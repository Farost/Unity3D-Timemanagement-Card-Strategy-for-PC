using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    private Queue<string> sentences;
    private Queue<string> answers;

    public Text nameText;
    public Text dialogueText;
    public Text answerText;
    public Text[] cardText;
    public Image[] cardImage;

    public Button answerButton;
    public Button startButton;
    public Button kickButton;
    public Button closeButton;
    public DataController dataController;
    public ButtonsManager buttonManager;
    public CitizensManager citizenManager;

    public int buttonYDimension = 25;

    public Citizen citizen;
    private int numberDialogue;
    private Dialogue dialogue;

    void Start () {
        sentences = new Queue<string>();
        answers = new Queue<string>();
        answerButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        numberDialogue = -1;
    }

    public void DialogueWindowPreparation(int index)
    {
        citizen = dataController.globalData.inObshinaCitizens[index];
        cardImage[0].sprite = citizen.GetSprite();
        cardImage[1].sprite = citizenManager.setHealthLevels[citizen.GetHealth()];
        cardText[0].text = citizen.GetShortName();
        cardText[1].text = citizen.GetEfficiency().ToString();
        cardImage[0].enabled = true;
        cardImage[1].enabled = true;
        cardText[0].enabled = true;
        cardText[1].enabled = true;
        SetDialogueToNull();
    }

    private void SetDialogueToNull()
    {
        startButton.gameObject.SetActive(false);
        if (citizen.GetStatus() == "В общине")
        {
            kickButton.interactable = true;
            startButton.interactable = true;
        }
        else
        {
            kickButton.interactable = false;
            startButton.interactable = false;
        }
        nameText.text = citizen.GetFullName();
        dialogueText.text = "Дней прожито в общине: " + citizen.GetDays().ToString() + "\n";
        dialogueText.text = dialogueText.text + "Успешных вылазок: " + citizen.GetDoneRaids().ToString() + "\n";
        dialogueText.text = dialogueText.text + "Проваленных вылазок: " + citizen.GetFailedRaids().ToString() + "\n" + "\n";
        if (citizen.IsFactUnlocked(0))
        {
            dialogueText.text = dialogueText.text + citizen.dialogues[0].fact + "\n";
            if (citizen.IsFactUnlocked(1))
            {
                dialogueText.text = dialogueText.text + citizen.dialogues[1].fact + "\n";
                if (citizen.IsFactUnlocked(2))
                {
                    dialogueText.text = dialogueText.text + citizen.dialogues[2].fact;
                }
            }
        }
        SetEnabledDialogue();
    }

    private void SetEnabledDialogue()
    {
        CheckDialogueEnabled();
        for (int i = 0; i < 3; i++)
        {
            if (citizen.IsDialogueUnlocked(i))
            {
                numberDialogue = i;
                startButton.gameObject.SetActive(true);
                dialogue = citizen.dialogues[i];
            }
        }
    }

    private void CheckDialogueEnabled()
    {
        if (citizen.IsFactUnlocked(2)) return;
        if (!citizen.IsFactUnlocked(0) && !citizen.IsDialogueUnlocked(0))
        {
            if (citizen.GetTrust() >= citizen.GetTrustForDialog(0))
            {
                citizen.SetDialogueUnlocked(0);
                Debug.Log("Dialogue 1 Unlocked");
                Debug.Log(citizen.GetTrust().ToString() + " trust and need: " + citizen.GetTrustForDialog(0).ToString());
            }
            return;
        }
        if (citizen.IsFactUnlocked(0))
        {
            if (!citizen.IsFactUnlocked(1) && !citizen.IsDialogueUnlocked(1))
            {
                if (citizen.GetTrust() >= citizen.GetTrustForDialog(1))
                {
                    citizen.SetDialogueUnlocked(1);
                    Debug.Log("Dialogue 2 Unlocked");
                    Debug.Log(citizen.GetTrust().ToString() + " trust and need: " + citizen.GetTrustForDialog(0).ToString());
                }
                return;
            }
            if (citizen.IsFactUnlocked(1))
            {
                if (!citizen.IsDialogueUnlocked(2))
                {
                    if (citizen.GetTrust() >= citizen.GetTrustForDialog(2))
                    {
                        citizen.SetDialogueUnlocked(2);
                        Debug.Log("Dialogue 3 Unlocked");
                        Debug.Log(citizen.GetTrust().ToString() + " trust and need: " + citizen.GetTrustForDialog(0).ToString());
                    }
                }
            }
        }
    }

    public void StartDialogue()
    {
        buttonManager.DisableRightMenuButtons();

        startButton.gameObject.SetActive(false);
        kickButton.gameObject.SetActive(false);
        closeButton.GetComponent<Button>().interactable = false;
        answerButton.gameObject.SetActive(true);

        sentences.Clear();
        answers.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (string answer in dialogue.answers)
        {
            answers.Enqueue(answer);
        }

        if (sentences.Count != answers.Count)
        {
            Debug.Log("Количество ответов и реплик не равно");
        }

            DisplayNextSentence();
    }
	
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (answers.Count == 0)
        {
            Debug.Log("Ответы закончились, а реплики остались!");
            answers.Enqueue("Продолжить");
        }

        string sentence = sentences.Dequeue();
        string answer = answers.Dequeue();
        dialogueText.text = sentence;
        answerText.text = answer;

        if (answer.Length < 14)
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(100, buttonYDimension);
        }
        else if (answer.Length < 21)
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(130, buttonYDimension);
        }
        else if (answer.Length < 28)
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(170, buttonYDimension);
        }
        else if (answer.Length < 35)
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(200, buttonYDimension);
        }
        else if (answer.Length < 40)
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(225, buttonYDimension);
        }
        else
        {
            answerButton.image.rectTransform.sizeDelta = new Vector2(250, buttonYDimension);
        }


        Debug.Log(sentence);
    }

    void EndDialogue()
    {
        Debug.Log("The end of the conversation.");
        buttonManager.EnableRightMenuButtons();

        startButton.gameObject.SetActive(true);
        kickButton.gameObject.SetActive(true);
        closeButton.GetComponent<Button>().interactable = true;
        answerButton.gameObject.SetActive(false);

        citizen.SetFactUnlocked(numberDialogue);
        numberDialogue = -1;

        SetDialogueToNull();
    }
}
