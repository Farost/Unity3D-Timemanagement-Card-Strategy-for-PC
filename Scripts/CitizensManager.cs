using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CitizensManager : MonoBehaviour
{
    private static System.Random rnd = new System.Random();
    private int newCitizenIndex;

    public GameController gameController;
    public CanvasGroup obshinaCanvas;
    public Image[] obshinaImages;
    public Image[] obshinaHealthBars;
    public Text[] obshinaCitizenNames;
    public Text[] obshinaCitizenEfficiency;
    public Sprite[] setHealthLevels;
    public DataController dataController;
    public Text[] obshinaWindowText;

    public Text citizenAcceptionText;

    private void Start()
    {
        HideAllCitizens();
    }

    public void GiveRandomCitizensToObshina(int quant)
    {
        for (int i = 0; i < quant; i++)
        {
            int index = rnd.Next(0, dataController.globalData.allCitizens.Count - 1);
            dataController.AddCitizenToObshina(index);
        }
    }

    public void OpenObshinaWindow()
    {
        obshinaCanvas.gameObject.SetActive(true);
        int i = 0;
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            obshinaImages[i].sprite = citizen.GetSprite();
            obshinaHealthBars[i].sprite = setHealthLevels[citizen.GetHealth()];
            obshinaCitizenNames[i].text = citizen.GetShortName();
            obshinaCitizenEfficiency[i].text = citizen.GetEfficiency().ToString();
            obshinaImages[i].enabled = true;
            obshinaHealthBars[i].enabled = true;
            obshinaCitizenNames[i].enabled = true;
            obshinaCitizenEfficiency[i].enabled = true;
            i++;
        }
        obshinaWindowText[0].text = "День №" + dataController.globalData.day;
        obshinaWindowText[1].text = "Количество жителей: " + dataController.globalData.inObshinaCitizens.Count;
    }

    public void CloseObshinaWindow()
    {
        HideAllCitizens();
        obshinaCanvas.gameObject.SetActive(false);
    }

    public void GetNewCitizen()
    {
        newCitizenIndex = rnd.Next(0, dataController.globalData.allCitizens.Count - 1);
        string effString;
        if (dataController.globalData.allCitizens[newCitizenIndex].GetEfficiency() < 30)
        {
            effString = "Он слаб, но готов выложиться на полную. ";
        }
        else
        {
            if (dataController.globalData.allCitizens[newCitizenIndex].GetEfficiency() < 60)
            {
                effString = "Он выглядит довольно тренированным. ";
            }
            else
            {
                effString = "Кажется, он действительно силен! ";
            }
        }
        string shortName = dataController.globalData.allCitizens[newCitizenIndex].GetShortName();
        citizenAcceptionText.text = "Босс, у входа найден новый выживший по имени " + shortName + ". " + effString + "Позволить ему присоединиться?";
    }

    public void ConfirmAcception()
    {
        OneCitizenAccepted();
        dataController.AddCitizenToObshina(newCitizenIndex);
        SortCitizens();
    }

    public void SortCitizens() //Сильнейшие находятся в начале
    {
        dataController.globalData.inObshinaCitizens.Sort(CompareCitizensReverse);
    }

    public void HideAllCitizens()
    {
        foreach (Image image in obshinaImages)
        {
            image.enabled = false;
        }
        foreach (Image image in obshinaHealthBars)
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

    }

    public Citizen FindCitizenByName(string citizenName)
    {
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            if (citizen.GetShortName() == citizenName) return citizen;
        }
        return null;
    }

    public void OneCitizenKilled()
    {
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            citizen.OneCitizenKilled();
        }
    }

    public void OneCitizenExpelled()
    {
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            citizen.OneCitizenExpelled();
        }
    }

    public void OneCitizenDead()
    {
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            citizen.OneCitizenDead();
        }
    }

    public void OneCitizenAccepted()
    {
        foreach (Citizen citizen in dataController.globalData.inObshinaCitizens)
        {
            citizen.OneCitizenAccepted();
        }
    }

    private static int CompareCitizensReverse(Citizen cit1, Citizen cit2)
    {
        if (cit1 == null)
        {
            if (cit2 == null)
            {
                return 0;  //Равны
            }
            else
            {
                return 1; //cit2 больше
            }
        }
        else
        {
            if (cit2 == null)
            {
                return -1; //cit1 больше
            }
            else
            {

                if (cit1.GetEfficiency() > cit2.GetEfficiency())
                {
                    return -1; //cit1 больше
                }
                else
                {
                    if (cit1.GetEfficiency() < cit2.GetEfficiency())
                    {
                        return 1; //cit2 больше
                    }
                    else
                    {
                        if (System.String.Compare(cit1.GetShortName(), cit2.GetShortName(), true) >= 0)
                        {
                            return 1; //Но по алфавиту в нужном порядке, если их эффективность равна
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
        }

    }
}