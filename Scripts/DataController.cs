using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using System.IO;

public class DataController : MonoBehaviour {

    private TextAsset citizensText;
    private TextAsset raidsText;
    private TextAsset dialoguesText;
    private TextAsset randMesText;

    public CitizensManager citizenManager;
    public GlobalData globalData;

    void Start () {
        DontDestroyOnLoad(gameObject);

        citizensText = (TextAsset)Resources.Load("Database/characters");
        raidsText = (TextAsset)Resources.Load("Database/quests");
        dialoguesText = (TextAsset)Resources.Load("Database/dialogues");
        randMesText = (TextAsset)Resources.Load("Database/randommessages");


        globalData = new GlobalData();
        globalData.SetGlobalData();

        LoadStartData();

        PrintAllCitizens();
        PrintAllRaids();
        citizenManager.GiveRandomCitizensToObshina(4);
        citizenManager.SortCitizens();
        Debug.Log("AFTER SORTING:");
        PrintInObshinaCitizens();
        PrintAllRandMessages();
    }

    public void AddCitizenToDatabase(Citizen newCitizen)
    {
        globalData.allCitizens.Add(newCitizen);

    }

    public void AddRaidToDatabase(Raid newRaid)
    {
        globalData.allRaids.Add(newRaid);

    }

    public void AddRandMessageToDatabase(string newMessage, int id)
    {
        if (id < 0) id = 0;
        if (globalData.randomMessages == null)
        {
            globalData.SetRandomMessages();
        }
        globalData.randomMessages[id].Add(newMessage);
    }

    public void AddCitizenToObshina(int index)
    {
        globalData.allCitizens[index].SetEfficiency((int)(globalData.k * globalData.allCitizens[index].GetEfficiency()));
        globalData.inObshinaCitizens.Add(globalData.allCitizens[index]);
        globalData.allCitizens.RemoveAt(index);
        globalData.inObshinaCitizens[globalData.inObshinaCitizens.Count - 1].AddCitizenToObshina();
    }

    public void KillCitizen(int index)
    {
        globalData.deadCitizens.Add(globalData.inObshinaCitizens[index]);
        globalData.inObshinaCitizens.RemoveAt(index);
        globalData.globalReputation -= 30;
    }

    public void ExpelCitizen(int index)
    {
        globalData.expelledCitizens.Add(globalData.inObshinaCitizens[index]);
        globalData.inObshinaCitizens.RemoveAt(index);
        globalData.globalReputation -= 10;
    }

    public void KillCitizen(Citizen someCitizen) //перегрузка
    {
        globalData.deadCitizens.Add(someCitizen);
        globalData.inObshinaCitizens.Remove(someCitizen);
        globalData.globalReputation -= 30;
    }

    public void ExpelCitizen(Citizen someCitizen) //перегрузка
    {
        globalData.expelledCitizens.Add(someCitizen);
        globalData.inObshinaCitizens.Remove(someCitizen);
        globalData.globalReputation -= 10;
    }

    public void UseRaid(Raid someRaid)
    {
        globalData.allRaids.Remove(someRaid);
    }

    public int GetAllCitizensCount()
    {
        return globalData.allCitizens.Count;
    }

    public int GetNonUsedRaidsCount()
    {
        return globalData.allRaids.Count;
    }

    public int GetAliveCitizensCount()
    {
        return globalData.inObshinaCitizens.Count;
    }

    public Citizen GetAliveCitizenByID(int id)
    {
        foreach (Citizen citizen in globalData.inObshinaCitizens)
        {
            if (citizen.id == id) return citizen;
        }
        return null;
    }

    public bool IsAnyAlive()
    {
        if (GetAliveCitizensCount() == 0)
            return false;
        else
            return true;
    }

    private void PrintAllCitizens()
    {
        foreach (Citizen cit in globalData.allCitizens)
        {
            Debug.Log("Citizen #" + cit.id + " " + cit.GetFullName() + " " + cit.GetEfficiency().ToString());
        }
    }

    private void PrintAllRaids()
    {
        foreach (Raid rd in globalData.allRaids)
        {
            Debug.Log("Raid #" + rd.GetId() + " " + rd.GetRaidName() + " Sum: " + rd.GetSumEfficiency().ToString());
        }
    }

    private void PrintInObshinaCitizens()
    {
        foreach (Citizen cit in globalData.inObshinaCitizens)
        {
            Debug.Log("Citizen #" + cit.id + " " + cit.GetFullName() + " Efficiency: " + cit.GetEfficiency().ToString());
        }
    }

    private void PrintAllRandMessages()
    {
        for (int i = 0; i < globalData.randomMessages.Length; i++)
        {
            for (int j = 0; j < globalData.randomMessages[i].Count; j++)
            {
                Debug.Log("ID" + i.ToString() + " Message: " + globalData.randomMessages[i][j]);
            }
        }
    }


    private void LoadStartData()
    {
        XElement citizens = null;
        XElement raids = null;
        XElement dialogues = null;
        XElement messages = null;


        citizens = XDocument.Parse(citizensText.text).Element("characters");
        if (citizens == null) Debug.Log("citizens are null");

        raids = XDocument.Parse(raidsText.text).Element("quests");
        if (raids == null) Debug.Log("raids are null");

        dialogues = XDocument.Parse(dialoguesText.text).Element("alldialogues");
        if (dialogues == null) Debug.Log("dialogues are null");

        messages = XDocument.Parse(randMesText.text).Element("messages");
        if (messages == null) Debug.Log("messages are null");


        LoadAllCitizens(citizens, dialogues);
        LoadAllRaids(raids);
        LoadAllRandMessages(messages);
    }

    private void LoadAllCitizens(XElement citizens, XElement alldialogues)
    {
        foreach (XElement character in citizens.Elements("character"))
        {
            Citizen newCitizen = new Citizen();
            newCitizen.id = int.Parse(character.Attribute("id").Value);
            newCitizen.SetSprite(newCitizen.id);
            newCitizen.SetShortName(character.Element("name").Value);
            newCitizen.SetFullName(character.Element("fullname").Value);
            newCitizen.SetEfficiency(int.Parse(character.Element("efficiency").Value));
            newCitizen.SetViews(int.Parse(character.Element("views").Value));
            newCitizen.SetTrust(int.Parse(character.Element("trust").Value));
            newCitizen.SetTrustCoefficient(double.Parse(character.Element("trustcoef").Value));
            newCitizen.SetTrustForDialog(0, int.Parse(character.Element("needoftrust1").Value));
            newCitizen.SetTrustForDialog(1, int.Parse(character.Element("needoftrust2").Value));
            newCitizen.SetTrustForDialog(2, int.Parse(character.Element("needoftrust3").Value));


            foreach (XElement dialogues in alldialogues.Elements("dialogues"))
            {
                if (newCitizen.id == int.Parse(dialogues.Attribute("personid").Value))
                {
                    foreach (XElement dialogue in dialogues.Elements("dialogue"))
                    {
                        int dialogueNumber = int.Parse(dialogue.Attribute("number").Value) - 1;
                        Dialogue newDialogue = new Dialogue(int.Parse(dialogue.Attribute("length").Value));
                        newDialogue.fact = dialogue.Element("fact").Value;
                        int i = 0;
                        foreach (XElement replic in dialogue.Element("replics").Elements("replic"))
                        {
                            newDialogue.sentences[i] = replic.Value;
                            i++;
                        }
                        i = 0;
                        foreach (XElement answer in dialogue.Element("answers").Elements("answer"))
                        {
                            newDialogue.answers[i] = answer.Value;
                            i++;
                        }
                        newCitizen.dialogues[dialogueNumber] = newDialogue;
                    }
                }
            }
            if (newCitizen.IsCitizenOkay())
            {
                AddCitizenToDatabase(newCitizen);
            }
        }
        globalData.SetRandomMessages();
    }

    private void LoadAllRaids(XElement raids)
    {
        foreach (XElement raid in raids.Elements("quest"))
        {
            Raid newRaid = new Raid();
            newRaid.SetId(int.Parse(raid.Attribute("id").Value));
            newRaid.SetRaidName(raid.Element("title").Value);
            newRaid.SetRaidDescription(raid.Element("title").Value);
            newRaid.SetRaidDescription(raid.Element("description").Value);
            newRaid.SetRaidTime(int.Parse(raid.Element("time").Value));
            newRaid.SetCoordinates(int.Parse(raid.Element("x").Value), int.Parse(raid.Element("y").Value));
            newRaid.SetSumEfficiency(int.Parse(raid.Element("needuspeh").Value));
            newRaid.SetMaxMembers(int.Parse(raid.Element("maxcharacters").Value));
            newRaid.SetMinMembers(int.Parse(raid.Element("mincharacters").Value));
            newRaid.SetPenPartial(int.Parse(raid.Element("penalty75").Value));
            newRaid.SetPenFail(int.Parse(raid.Element("penaltyloss").Value));
            newRaid.SetTypeOfViews(int.Parse(raid.Element("type").Value));
            globalData.maxReadedRaidID = newRaid.GetId();
            if (newRaid.IsRaidOkay())
            {
                AddRaidToDatabase(newRaid);
            }
        }
    }

    private void LoadAllRandMessages(XElement messages)
    {
        foreach (XElement message in messages.Elements("message"))
        {
            string newMessage;
            int id;
            id = int.Parse(message.Attribute("personid").Value);
            newMessage = message.Value;
            AddRandMessageToDatabase(newMessage, id);
        }
    }


}


