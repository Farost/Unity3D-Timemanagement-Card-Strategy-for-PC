using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaidsManager : MonoBehaviour {

    private static System.Random rnd = new System.Random();

    public CanvasGroup raidResultCanvas;
    public Canvas raidFlagCanvas;

    public Image raidFlag;
    public Text raidFlagTimer;
    public Text raidName;
    public Text raidDescription;
    private string raidDescriptionMemory;
    public Image[] obshinaImages;
    public Image[] obshinaHealthBars;
    public Image[] selectedImages;
    public Image[] selectedHealthBars;
    public Text[] obshinaCitizenNames;
    public Text[] obshinaCitizenEfficiency;
    public Text[] selectedCitizenNames;
    public Text[] selectedCitizenEfficiency;
    public Sprite[] cardSprites; //0 - рамка, 1 - галочка, 2 - Х
    private bool[] selectedFilled;
    private bool[] obshinaFilled;
    public Text helperText;
    
    public CitizensManager citizensManager;
    public ButtonsManager buttonManager;
    public GameController gameController;

    private float startTime;
    private List<Raid> inProgressRaids;
    public List<Image> progressBars;
    public List<Text> raidNameText;
    public List<Animator> animators;

    private Raid currentRaid;
    private int[] returnPositions;
    private int[] initialSelectedSprite;

    void Start () {
        inProgressRaids = new List<Raid>();
        returnPositions = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
        initialSelectedSprite = new int[8] {2, 2, 2, 2, 2, 2, 2, 2 };
        obshinaFilled = new bool[9];
        selectedFilled = new bool[8];
        raidFlagTimer.text = "0";
        raidFlag.enabled = false;
        raidFlagTimer.enabled = false;
        raidDescriptionMemory = "";
        startTime = 0.0f;
        HideAllCards();
        currentRaid = null;
        progressBars[0].fillAmount = 1;
        progressBars[1].fillAmount = 1;
        animators[0].SetBool("IsOpen", false);
        animators[1].SetBool("IsOpen", false);
        helperText.text = "Текст";
    }
	
	void Update () {
        if (Time.timeScale != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                if (progressBars[i].fillAmount != 1)
                {
                    progressBars[i].fillAmount += Time.deltaTime / inProgressRaids[i].GetRaidTime();
                }
                else
                {
                    if (animators[i].GetBool("IsOpen"))
                    {
                        EndRaid(i);
                    }
                }
            }
        }

        if (raidFlagTimer.text != "0" && currentRaid != null)
        {
            if (animators[1].GetBool("IsOpen"))
            {
                raidDescription.text = "Люди волнуются. Дождитесь возвращения людей с вылазки '"+inProgressRaids[1].GetRaidName()+"'";
                raidDescription.color = Color.red;
            }
            else
            {
                if (raidDescription.color == Color.red)
                {
                    raidDescription.text = raidDescriptionMemory;
                    raidDescription.color = raidName.color;
                }
                buttonManager.raidButtons[1].interactable = IsSelectedMin();  //Проверка, достигли ли мы нужного числа
            }
            UpdateTimer();
        }
        else
        {
            if (raidFlag.isActiveAndEnabled)
            {
                raidFlag.enabled = false;
                raidFlagTimer.enabled = false;
            }
        }
    }

    public void UpdateTimer()
    {
        float t = Time.time - startTime;
        raidFlagTimer.text = (currentRaid.GetRaidTime() - (int)t).ToString();
    }

    public void SpawnRaid(int raidNumber)
    {
        currentRaid = IsRaidNotUsed(raidNumber);
        if (currentRaid == null) //Если запрашиваемая вылазка уже была использована или ее ID == -1
        {
            raidNumber = rnd.Next(0, citizensManager.dataController.globalData.allRaids.Count - 1); //Случайная вылазка
            currentRaid = citizensManager.dataController.globalData.allRaids[raidNumber];
        }
        
        startTime = Time.time;
        raidFlagTimer.text = currentRaid.GetRaidTime().ToString();

        //Располагаем относительно Canvas
        RectTransform CanvasRect = raidFlagCanvas.GetComponent<RectTransform>();
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(raidFlagCanvas.transform.position);
        raidFlag.rectTransform.anchoredPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f) + currentRaid.GetCoordinates().x + 0.116f),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f) + currentRaid.GetCoordinates().y));

        raidFlag.enabled = true;
        raidFlagTimer.enabled = true;
    }

    public void OpenRaidWindow()
    {
        int i = 0;
        foreach (Citizen citizen in citizensManager.dataController.globalData.inObshinaCitizens)
        {
            if (!IsCitizenOnRaid(citizen))
            {
                obshinaImages[i].sprite = citizen.GetSprite();
                obshinaHealthBars[i].sprite = citizensManager.setHealthLevels[citizen.GetHealth()];
                obshinaCitizenNames[i].text = citizen.GetShortName();
                obshinaCitizenEfficiency[i].text = citizen.GetEfficiency().ToString();
                obshinaImages[i].enabled = true;
                obshinaHealthBars[i].enabled = true;
                obshinaCitizenNames[i].enabled = true;
                obshinaCitizenEfficiency[i].enabled = true;
                obshinaFilled[i] = true;
                i++;
            }
        }

        ShowSelectCardsDefault();
        for (int j = 0; j < currentRaid.GetMaxMembers(); j++)
        {
            selectedImages[j].sprite = cardSprites[1];
            initialSelectedSprite[j] = 1;
        }

        raidName.text = currentRaid.GetRaidName();
        raidDescription.text = currentRaid.GetRaidDescription();
        raidDescriptionMemory = raidDescription.text;
        buttonManager.raidButtons[1].interactable = false;
    }

    public Raid IsRaidNotUsed(int number)
    {
        foreach (Raid raid in citizensManager.dataController.globalData.allRaids)
        {
            if (raid.GetId() == number) return raid;
        }
        return null;
    }

    public bool IsCitizenOnRaid(Citizen citizen)
    {
        foreach (Raid raid in inProgressRaids)
        {
            if (raid != null && raid.members.Contains(citizen))
            {
                return true;
            }
        }
        return false;
    }

    public void CloseRaidWindow()
    {
        ShowSelectCardsDefault();
        HideAllCards();
    }

    public void HideAllCards()
    {
        foreach (Image image in obshinaImages)
        {
            image.enabled = false;
        }
        foreach (Image image in obshinaHealthBars)
        {
            image.enabled = false;
        }
        foreach (Image image in selectedImages)
        {
            image.enabled = false;
        }
        foreach (Image image in selectedHealthBars)
        {
            image.enabled = false;
        }

        foreach (Text text in obshinaCitizenNames)
        {
            text.enabled = false;
        }
        foreach (Text text in obshinaCitizenEfficiency)
        {
            text.enabled = false;
        }

        foreach (Text text in selectedCitizenNames)
        {
            text.enabled = false;
        }
        foreach (Text text in selectedCitizenEfficiency)
        {
            text.enabled = false;
        }
        
        for (int i = 0; i < 8; i++)
        {
            selectedFilled[i] = false;
        }
        for (int i = 0; i < 9; i++)
        {
            obshinaFilled[i] = false;
        }
    }

    public void ShowSelectCardsDefault()
    {
        foreach (Image image in selectedImages)
        {
            image.sprite = cardSprites[2];
            image.enabled = true;
        }
        foreach (Image image in selectedHealthBars)
        {
            image.enabled = false;
        }

        foreach (Text text in selectedCitizenNames)
        {
            text.enabled = false;
        }
        foreach (Text text in selectedCitizenEfficiency)
        {
            text.enabled = false;
        }

        for (int i = 0; i < 8; i++)
        {
            returnPositions[i] = -1;
            initialSelectedSprite[i] = 2;
        }
    }

    public void ReturnCitizenFromRaid(int number)
    {
        if (!selectedFilled[number] || citizensManager.dataController.globalData.isPaused ||
            citizensManager.dataController.globalData.isQuitting) return;
        int index = returnPositions[number]; //Место, куда надо вернуть
        if (index < 0)
        {
            Debug.Log("ReturnCitizenFromRaid index < 0");
            return;
        }
        obshinaCitizenNames[index].enabled = true;
        obshinaCitizenEfficiency[index].enabled = true;
        obshinaHealthBars[index].enabled = true;
        obshinaImages[index].sprite = selectedImages[number].sprite;
        obshinaFilled[index] = true;

        selectedCitizenNames[number].enabled = false;
        selectedCitizenEfficiency[number].enabled = false;
        selectedHealthBars[number].enabled = false;
        selectedImages[number].sprite = cardSprites[initialSelectedSprite[number]];
        selectedFilled[number] = false;
        returnPositions[number] = -1;
    }

    public void PickCitizenToRaid(int number)
    {
        if (!obshinaFilled[number] || IsSelectedFull() || citizensManager.dataController.globalData.isPaused ||
            citizensManager.dataController.globalData.isQuitting) return;
  
        int i = -1;
        bool k = true;
        while (k && i < 7)
        {
            i++;
            k = selectedFilled[i];
        }
        if (!k)
        {
            selectedCitizenNames[i].enabled = true;
            selectedCitizenNames[i].text = obshinaCitizenNames[number].text;
            selectedCitizenEfficiency[i].enabled = true;
            selectedCitizenEfficiency[i].text = obshinaCitizenEfficiency[number].text;
            selectedHealthBars[i].enabled = true;
            selectedHealthBars[i].sprite = obshinaHealthBars[number].sprite;
            selectedImages[i].sprite = obshinaImages[number].sprite;
            selectedFilled[i] = true;
            returnPositions[i] = number; //Место, куда надо вернуть, запоминается в массив из 8 мест

            obshinaCitizenNames[number].enabled = false;
            obshinaCitizenEfficiency[number].enabled = false;
            obshinaHealthBars[number].enabled = false;
            obshinaImages[number].sprite = cardSprites[0];
            obshinaFilled[number] = false;
        }
    }

    private bool IsSelectedFull()
    {
        int filled = 0;
        for (int i = 0; i < 8; i++)
        {
            if (selectedFilled[i]) filled++;
        }
        if (currentRaid.GetMaxMembers() == filled) return true;
        else return false;
    }

    private bool IsSelectedMin()
    {
        int filled = 0;
        for (int i = 0; i < 8; i++)
        {
            if (selectedFilled[i]) filled++;
        }
        if (currentRaid.GetMinMembers() <= filled) return true;
        else return false;
    }

    public void StartRaid()
    {
        for (int i = 0; i < 8; i++)
        {
            if (selectedFilled[i])
            {
                Citizen citizen = citizensManager.FindCitizenByName(selectedCitizenNames[i].text);
                if (citizen == null)
                {
                    Debug.Log("Citizen " + i.ToString() + " is null");
                    return;
                }
                currentRaid.members.Add(citizen);
                citizen.CitizenToRaid();
            }
        }

        if (inProgressRaids.Count == 1 && inProgressRaids[inProgressRaids.Count-1] == null)
        {
            inProgressRaids.Clear();
        }

        currentRaid.numberInQueue = inProgressRaids.Count;
        if (currentRaid.numberInQueue > 1)
        {
            Debug.Log("This prototype works only with 2 coraids. There will be an error.");
        }
        citizensManager.dataController.UseRaid(currentRaid);
        
        inProgressRaids.Add(currentRaid);
        Debug.Log("STARTED RAID NUMBER IN QUEUE "+currentRaid.numberInQueue+" WITH " + currentRaid.members.Count.ToString() + " CITIZENS!");
        animators[currentRaid.numberInQueue].SetBool("IsOpen", true);
        raidNameText[currentRaid.numberInQueue].text = currentRaid.GetRaidName();
        progressBars[currentRaid.numberInQueue].fillAmount = 0;
        currentRaid = null;
        gameController.CloseRaidWindow(); //дальше переносимся в Update()
    }

    private void EndRaid(int index)
    {
        animators[index].SetBool("IsOpen", false);
        Debug.Log("Raid queue " + index.ToString() + " has been ended.");
        int successPercentage = inProgressRaids[index].CountSuccessPercentage();
        string helperMessage = "Вылазка '" + inProgressRaids[index].GetRaidName() + "' завершена. ";
        int damage = 0;
        if (successPercentage >= 100)
        {
            helperMessage += "Все прошло лучше некуда! Все бойцы вернулись целыми.\n";
            citizensManager.dataController.globalData.successRaidCount++;
        }
        if (successPercentage >= 75 && successPercentage < 100)
        {
            helperMessage += "Не без проблем, но мы справились. Вот итог:\n";
            damage = inProgressRaids[index].GetPenPartial();
            citizensManager.dataController.globalData.successRaidCount++;
        }
        if (successPercentage < 75)
        {
            helperMessage += "Отряд не справился, вылазка провалена. Вот, что вышло:\n";
            damage = inProgressRaids[index].GetPenFail();
            citizensManager.dataController.globalData.failedRaidCount++;
        }

        List<int> initialHealth = new List<int>();
        foreach (Citizen citizen in inProgressRaids[index].members)
        {
            citizen.CitizenFromRaid();
            initialHealth.Add(citizen.GetHealth());
            citizen.DoneRaid(successPercentage, inProgressRaids[index].GetTypeOfViews());  //Сначала прибавялем, а потом даем урон для того, чтобы
        }                                                                                //не бегать по списку members 2 раза
        DoPunishment(damage, inProgressRaids[index].members);
        for (int i = 0; i < inProgressRaids[index].members.Count; i++)
        {
            int difference = initialHealth[i] - inProgressRaids[index].members[i].GetHealth();
            if (difference != 0)
            {
                if (difference == 1) helperMessage += "Житель " + inProgressRaids[index].members[i].GetShortName() + " был ранен.";
                else
                    if (difference == 2) helperMessage += "Житель " + inProgressRaids[index].members[i].GetShortName() + " был серьезно ранен.";
                    else
                        if (difference == 3) helperMessage += "Житель " + inProgressRaids[index].members[i].GetShortName() + " не перенес полученных увечий.";
                if (inProgressRaids[index].members[i].GetStatus() == "Мертв")
                {
                    helperMessage += "\nК сожалению, он погиб.";
                    citizensManager.dataController.KillCitizen(inProgressRaids[index].members[i]);
                    citizensManager.OneCitizenDead();
                }
                helperMessage += "\n";
            }
        }

        Debug.Log(helperMessage);

        if (index == 1 || (index == 0 && !animators[1].GetBool("IsOpen")))
        {
            inProgressRaids.Remove(inProgressRaids[index]);
        }
        else
        {
            inProgressRaids[index] = null;
        }

        gameController.CheckAlivePeople();
        OpenRaidResultWindow(helperMessage);
    }

    private void DoPunishment(int n, List<Citizen> members)
    {
        for (int i = 0; i < n; i++)
        {
            int index = rnd.Next(0, members.Count - 1);
            if (members[index].GetHealth() >= 0)
                members[index].ChangeHealth(-1);
        }
    }

    public void OpenRaidResultWindow(string helperMessage)
    {
        Time.timeScale = 0;
        raidResultCanvas.gameObject.SetActive(true);
        buttonManager.DisableRightMenuButtons();
        gameController.isBlockOpened[3] = true;
        helperText.text = helperMessage;
        helperText.fontSize = 16;
        if (helperMessage.Length > 320)
        {
            helperText.fontSize = 14;
        }
        if (helperMessage.Length > 400)
        {
            helperText.fontSize = 12;
        }
        if (helperMessage.Length > 100 && helperMessage.Length < 200)
        {
            helperText.text = "\n" + helperText.text;
        }
        if (helperMessage.Length < 100 && helperMessage.Length > 50)
        {
            helperText.text = "\n\n" + helperText.text;
        }
        if (helperMessage.Length < 50)
        {
            helperText.text = "\n\n\n" + helperText.text;
        }
    }

    public void CloseRaidResultWindow()
    {
        Time.timeScale = 1;
        raidResultCanvas.gameObject.SetActive(false);
        buttonManager.EnableRightMenuButtons();
        gameController.isBlockOpened[3] = false;
    }

}