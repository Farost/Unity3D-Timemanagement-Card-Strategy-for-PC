using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Citizen {

    private static System.Random rnd = new System.Random();

    public int id;
    [SerializeField]
    string shortName;
    [SerializeField]
    string fullName;
    int days;
    int efficiency;
    int health;
    int trust;
    double trustCoef;
    int views;
    public Dialogue[] dialogues;
    int[] needTrustForDialogue;
    bool[] isUnlockedDialogue;
    bool[] isUnlockedFact;
    Sprite citizenSprite;
    int doneRaids;
    int failedRaids;

    int statusValue;

    public Citizen()
    {
        dialogues = new Dialogue[3];
        needTrustForDialogue = new int[3];
        health = 0;
        days = 0;
        doneRaids = 0;
        failedRaids = 0;
        isUnlockedDialogue = new bool[3] {false, false, false};
        isUnlockedFact = new bool[3] {false, false, false};
    }

    public void AddCitizenToObshina()  
    {
        GenerateHealth();
        statusValue = 2;
    }

    private void GenerateHealth()
    {
        double temp = System.Convert.ToDouble(rnd.Next(10000)) / 100;
        if (temp > 0 && temp <= 60)
            health = 2;
        if (temp > 60 && temp <= 90)
            health = 1;
        if (temp > 90 && temp <= 100)
            health = 0;
    }

    // **********Getter'ы**********

    public string GetShortName()
    {
        return shortName;
    }

    public string GetFullName()
    {
        return fullName;
    }

    public int GetDays()
    {
        return days;
    }

    public int GetEfficiency()
    {
        return efficiency;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetEfficiencyConsiderHealth() //Учесть уровень здоровья
    {
        if (health == 2) return efficiency;

        if (health == 1) return (int)(efficiency * 0.8);

        else return (int)(efficiency * 0.5);
    }

    public int GetTrust()
    {
        return trust;
    }

    public double GetTrustCoefficient()
    {
        return trustCoef;
    }

    public int GetViews()
    {
        return views;
    }

    public Sprite GetSprite()
    {
        return citizenSprite;
    }

    public int GetTrustForDialog(int index)
    {
        if (index < 0 || index > 2)
        {
            Debug.Log("ALERT! INDEX OUT OF RANGE!");
            return 0;
        }
        else
            return needTrustForDialogue[index];
    }

    public string GetStatus()
    {
        if (statusValue == 3)
            return "На вылазке";
        if (statusValue == 2)
            return "В общине";
        if (statusValue == 1)
            return "В изгнании";
        if (statusValue == 0)
            return "Мертв";
        else
            return "Неизвестно";
    }

    public int GetDoneRaids()
    {
        return doneRaids;
    }

    public int GetFailedRaids()
    {
        return failedRaids;
    }

    public bool IsFactUnlocked(int index)
    {
        if (isUnlockedFact[index])
            return true;
        else
            return false;
    }

    public bool IsDialogueUnlocked(int index)
    {
        if (isUnlockedDialogue[index])
            return true;
        else
            return false;
    }

    //*******************Set'еры*********

    public void SetShortName(string name)
    {
        shortName = name;
    }

    public void SetFullName(string name)
    {
        fullName = name;
    }

    public void SetEfficiency(int eff)
    {
        efficiency = eff;
    }

    public void SetHealth(int hp)
    {
        health = hp;
    }

    public void SetTrust(int tru)
    {
        trust = tru;
    }

    public void SetTrustCoefficient(double tc)
    {
        trustCoef = tc;
    }

    public void SetViews(int view)
    {
        views = view;
    }

    public void SetTrustForDialog(int i, int j)
    {
        needTrustForDialogue[i] = j;
    }

    public void KillCitizen()
    {
        statusValue = 0;
    }

    public void ExpelCitizen()
    {
        statusValue = 1;
    }

    public void CitizenToRaid()
    {
        statusValue = 3;
    }

    public void CitizenFromRaid()
    {
        statusValue = 2;
    }

    public void SetSprite(int i)
    {
        citizenSprite = Resources.Load<Sprite>("Citizens/Avatars/" + i.ToString() + "-citizen");
    }

    public void SetFactUnlocked(int index)
    {
        isUnlockedFact[index] = true;
        isUnlockedDialogue[index] = false;
    }

    public void SetDialogueUnlocked(int index)
    {
        isUnlockedDialogue[index] = true;
    }

    public void SetDialogueLocked(int index)
    {
        isUnlockedDialogue[index] = false;
    }

    public bool IsCitizenOkay()
    {
        if (shortName.Length > 15)
        {
            Debug.Log("Incorrect shortName Length in Citizen id#" + id.ToString());
            return false;
        }
        if (fullName.Length > 30)
        {
            Debug.Log("Incorrect fullName Length in Citizen id#" + id.ToString());
            return false;
        }
        if (efficiency <= 0)
        {
            Debug.Log("Incorrect Efficiency in Citizen id#" + id.ToString());
            return false;
        }
        if (trustCoef > 2 || trustCoef < 0.1)
        {
            Debug.Log("Incorrect TrustCoefficient in Citizen id#" + id.ToString());
            return false;
        }
        if (views < -10 || views > 10)
        {
            Debug.Log("Incorrect Views in Citizen id#" + id.ToString());
            return false;
        }
        return true;
    }

    public void OneCitizenKilled()
    {
        switch (views)
        {
            case 2:
                trust -= (int)(60 / trustCoef); //Добряк
                break;
            case -2:
                trust += (int)(60 * trustCoef); //Агрессор
                break;
            case 3:
                trust += (int)(10 * trustCoef); //Инноватор
                break;
            case -3:
                trust -= (int)(10 / trustCoef); //Консерватор
                break;
        }
    }

    public void OneCitizenExpelled()
    {
        switch (views)
        {
            case 2:
                trust -= (int)(30 / trustCoef); //Добряк
                break;
            case -2:
                trust += (int)(30 * trustCoef); //Агрессор
                break;
            case 3:
                trust += (int)(10 * trustCoef); //Инноватор
                break;
            case -3:
                trust -= (int)(10 / trustCoef); //Консерватор
                break;
        }
    }

    public void OneCitizenAccepted()
    {
        switch (views)
        {
            case 2:
                trust += (int)(30 * trustCoef); //Добряк
                break;
            case -2:
                trust -= (int)(20 / trustCoef); //Агрессор
                break;
            case 3:
                trust += (int)(40 * trustCoef); //Инноватор
                break;
            case -3:
                trust -= (int)(30 / trustCoef); //Консерватор
                break;
            case 1:
                trust += (int)(30 * trustCoef);  //Лидер
                break;
        }
    }

    public void OneCitizenDead()
    {
        switch (views)
        {
            case 2:
                trust -= (int)(30 / trustCoef); //Добряк
                break;
            case -2:
                trust += (int)(20 * trustCoef); //Агрессор
                break;
            case 1:
                trust -= (int)(60 / trustCoef);  //Лидер
                break;
        }
    }

    public void ChangeHealth(int i)
    {
        if (health >= 0)
            health += i;
        if (health < 0)
            statusValue = 0;
    }

    public void PlusOneDay()
    {
        days++;
    }

    public void PlusOneDoneRaid()
    {
        doneRaids++;
    }

    public void PlusOneFailedRaid()
    {
        failedRaids++;
    }

    void GetMedicine(int medicine)
    {  
        if (health == 1)
            health += 1;   //прибаляет 1 (потому что выше нельзя)
        if (health == 0)
            health += medicine; //прибавляет тип лекарства (1 или 2)
    }

    void AddDay()
    {
        days++;
        trust += 5;
    }

    public void DoneRaid(int perc, int raidViewsType) //вылазка завершена
    {            
        if (perc >= 100)
        {
            doneRaids++;
            trust += (int)(10 * trustCoef);
            efficiency += 10;
        }
        if (perc < 100 && perc >= 75)
        {
            doneRaids++;
            efficiency += 5;
        }
            
        if (perc < 75)
        {
            failedRaids++;
            trust -= (int)(30 / trustCoef);
        }
            
        if (views == raidViewsType)
        {
            trust += (int)(10 * trustCoef);
        }
        if (views == -1*(raidViewsType))
        {
            trust -= (int)(10 / trustCoef);
        }

    }

}
