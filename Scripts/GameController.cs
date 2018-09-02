using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public DayClocks dayClocks;

    public ButtonsManager buttonsManager;
    public CitizensManager citizenManager;
    public DialogueManager dialogueManager;
    public RaidsManager raidsManager;

    public Text helperText;
    public Text kickCitizenText;
    public CanvasGroup[] openingWindows;
    public DataController dataController;
    public CanvasGroup endGameWindow;

    private int playerReputation;

    private Queue<Button> interButtons;
    private int interCanvasObshinaAndRaid;
    private bool[] spawned;
    public bool[] isBlockOpened = {false, false, false, false, false };

    private int raidNumber;
    
    void Start () {
        foreach(CanvasGroup window in openingWindows)
        {
            window.gameObject.SetActive(false);
        }
        endGameWindow.gameObject.SetActive(false);
        buttonsManager.EnableRightMenuButtons();
        spawned = new bool[8] {false, false, false, false, false, false, false, false };    // 0 и 1 - спаун персонажей
        isBlockOpened = new bool[4] {false, false, false, false };
        interButtons = new Queue<Button>();
        raidNumber = -1;
    }
	
	void Update () {
		if (dayClocks.clocksString == "0615" && !spawned[0]) //1 пришедший персонаж
        {
            OpenCitizenAcceptionWindow();
            spawned[0] = true;
        }

        if (dayClocks.clocksString == "0630" && !spawned[2]) //1-я случайная вылазка
        {
            raidsManager.SpawnRaid(raidNumber);
            Debug.Log("Spawned 1st Raid");
            spawned[2] = true;
        }

        if (dayClocks.clocksString == "0907" && !spawned[3])  //2-я вылазка, номер 3
        {
            raidNumber = 4;
            raidsManager.SpawnRaid(raidNumber);
            spawned[3] = true;
        }

        if (dayClocks.clocksString == "0935" && !spawned[4])  //3-я вылазка, номер 5
        {
            raidNumber = 6;
            raidsManager.SpawnRaid(raidNumber);
            spawned[4] = true;
        }

        if (dayClocks.clocksString == "1002" && !spawned[5])  //4-я случайная вылазка
        {
            raidsManager.SpawnRaid(raidNumber);
            spawned[5] = true;
        }

        if (dayClocks.clocksString == "1200" && !spawned[6])  //5-я случайная вылазка
        {
            raidsManager.SpawnRaid(raidNumber);
            spawned[6] = true;
        }

        if (dayClocks.clocksString == "1400" && !spawned[7])  //6-я случайная вылазка
        {
            raidsManager.SpawnRaid(raidNumber);
            spawned[7] = true;
        }

        if (dayClocks.clocksString == "1145" && !spawned[1]) //2 пришедший персонаж
        {
            OpenCitizenAcceptionWindow();
            spawned[1] = true;
        }
    }

    public void OpenObshinaWindow()
    {
        foreach (var cit in dataController.globalData.inObshinaCitizens)
        {
            Debug.Log("Citizen Name: " + cit.GetShortName() + " and trust: " + cit.GetTrust());
        }
        foreach (var cit in dataController.globalData.expelledCitizens)
        {
            Debug.Log("EXPELLED CITIZEN Name: " + cit.GetShortName());
        }
        foreach (var cit in dataController.globalData.deadCitizens)
        {
            Debug.Log("DEAD CITIZEN Name: " + cit.GetShortName());
        }

        if (openingWindows[0].gameObject.activeInHierarchy)
        {
            citizenManager.CloseObshinaWindow();
        }
        else
        {
            foreach (CanvasGroup window in openingWindows)
            {
                window.gameObject.SetActive(false);
            }
            citizenManager.OpenObshinaWindow();
        }
    }

    public void CloseObshinaWindow()
    {
        citizenManager.CloseObshinaWindow();
    }

    public void OpenPomoshnikWindow()
    {
        if (openingWindows[6].gameObject.activeInHierarchy)
        {
            ClosePomoshnikWindow();
        }
        else
        {
            if (openingWindows[0].gameObject.activeInHierarchy)
            {
                CloseObshinaWindow();
            }
            foreach (CanvasGroup window in openingWindows)
            {
                window.gameObject.SetActive(false);
            }
            openingWindows[6].gameObject.SetActive(true);

            helperText.text = "\nЖителей в общине: " + dataController.globalData.inObshinaCitizens.Count.ToString() + "\nЖителей погибло: "
                + dataController.globalData.deadCitizens.Count.ToString() + "\nЖителей пропало: " + dataController.globalData.expelledCitizens.Count.ToString()
                + "\nУспешных вылазок: " + dataController.globalData.successRaidCount.ToString() + "\nПроваленных вылазок: "
                + dataController.globalData.failedRaidCount.ToString();
        }
    }

    public void ClosePomoshnikWindow()
    {
        openingWindows[6].gameObject.SetActive(false);
    }

    public void OpenDialogueWindow(int index)
    {
        if (Time.timeScale != 0)
        {
            CloseObshinaWindow();
            openingWindows[2].gameObject.SetActive(true);
            dialogueManager.DialogueWindowPreparation(index);
        }
    }

    public void CloseDialogueWindow()
    {
        openingWindows[2].gameObject.SetActive(false);
        OpenObshinaWindow();
    }

    public void OpenKickConfirmWindow()
    {
        openingWindows[4].gameObject.SetActive(true);
        kickCitizenText.text = "Избавиться от жителя " + dialogueManager.citizen.GetShortName() + " ?";
        buttonsManager.DisableButtons();
        for (int i = 2; i < 5; i++)
        {
            buttonsManager.confirmacceptionButtons[i].interactable = true;
        }
        isBlockOpened[0] = true;
        Time.timeScale = 0;
    }

    public void CloseKickConfirmWindow()
    {
        openingWindows[4].gameObject.SetActive(false);
        citizenManager.OneCitizenExpelled();
        buttonsManager.EnableButtons();
        isBlockOpened[0] = false;
        Time.timeScale = 1;
    }

    public void ExpelCitizenClick()
    {
        dataController.ExpelCitizen(dialogueManager.citizen);
        CloseKickConfirmWindow();
        CloseDialogueWindow();
        CheckAlivePeople();
    }

    public void KillCitizenClick()
    {
        dataController.KillCitizen(dialogueManager.citizen);
        citizenManager.OneCitizenKilled();
        CloseKickConfirmWindow();
        CloseDialogueWindow();
        CheckAlivePeople();
    }

    public void OpenCitizenAcceptionWindow()
    {
        
        Time.timeScale = 0;
        isBlockOpened[1] = true;
        buttonsManager.DisableRightMenuButtons();
        if (interButtons.Count > 0)
        {
            interButtons.Clear();
        }
        interCanvasObshinaAndRaid = -1;

        foreach (Button button in buttonsManager.gameButtons)
        {
            if (button.IsInteractable())
            {
                interButtons.Enqueue(button);
            }
        }

        for (int i = 0; i < 7; i+=3)
        {
            if (openingWindows[i].gameObject.activeInHierarchy) //Если включено окно общины, помощника или вылазки
            {
                interCanvasObshinaAndRaid = i;
            }
        }
        

        foreach (Button button in interButtons)
        {
            button.interactable = false;
        }

        if (interCanvasObshinaAndRaid != -1)
        {
            openingWindows[interCanvasObshinaAndRaid].gameObject.SetActive(false);
        }

        if (!openingWindows[5].gameObject.activeInHierarchy)
        {
            openingWindows[5].gameObject.SetActive(true);
        }
        buttonsManager.EnableConfirmButtons();

        citizenManager.GetNewCitizen();
    }

    public void ConfirmCitizenAcception()
    {
        citizenManager.ConfirmAcception();
        CloseCitizenAcceptionWindow();
    }

    public void CloseCitizenAcceptionWindow()
    {
        foreach (Button button in interButtons)
        {
            button.interactable = true;
        }
        interButtons.Clear();
        openingWindows[5].gameObject.SetActive(false);
        buttonsManager.EnableRightMenuButtons();
        isBlockOpened[1] = false;
        Time.timeScale = 1;

        if (interCanvasObshinaAndRaid == 0)
        {
            OpenObshinaWindow();
        }
        else
        {
            if (interCanvasObshinaAndRaid == 3)
            {
                OpenRaidWindow();
            }
            else
            {
                if (interCanvasObshinaAndRaid == 6)
                {
                    OpenPomoshnikWindow();
                }
            }
        }
        interCanvasObshinaAndRaid = -1;
    }

    public void OpenRaidWindow() //До вызова нужно изменить raidNumber, если нужна конкретная вылазка
    {
        foreach (CanvasGroup window in openingWindows)
        {
            window.gameObject.SetActive(false);
        }

        openingWindows[3].gameObject.SetActive(true);
        isBlockOpened[2] = true;
        buttonsManager.DisableRightMenuButtons();
        Time.timeScale = 0;
        raidsManager.OpenRaidWindow();
    }

    public void CloseRaidWindow()
    {
        openingWindows[3].gameObject.SetActive(false);
        isBlockOpened[2] = false;
        buttonsManager.EnableRightMenuButtons();
        raidsManager.CloseRaidWindow();
        raidNumber = -1;
        Time.timeScale = 1;
    }

    public void CheckAlivePeople()
    {
        if (!dataController.IsAnyAlive())
        {
            LooseGame();
        }
    }

    public void EndDay()
    {
        Time.timeScale = 0;
    }

    private void LooseGame()
    {
        Time.timeScale = 0;
        buttonsManager.DisableButtons();
        foreach (CanvasGroup window in openingWindows)
        {
            window.gameObject.SetActive(false);
        }
        endGameWindow.gameObject.SetActive(true);
    }

    public bool IsAnyBlockOpened()
    {
        foreach (bool block in isBlockOpened)
        {
            if (block) return true;
        }
        return false;
    }
}
