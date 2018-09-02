using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalData
{
    public bool isPaused;
    public bool isQuitting;

    public List<Citizen> allCitizens;
    public List<Citizen> inObshinaCitizens;
    public List<Citizen> deadCitizens;
    public List<Citizen> expelledCitizens;
    public List<Raid> allRaids;
    public List<string>[] randomMessages;
    public int successRaidCount;
    public int failedRaidCount;

    public int maxReadedRaidID;
    public double k;
    public int day;
    public int globalReputation;

    public void SetGlobalData()
    {
        allCitizens = new List<Citizen>();
        inObshinaCitizens = new List<Citizen>();
        deadCitizens = new List<Citizen>();
        expelledCitizens = new List<Citizen>();
        allRaids = new List<Raid>();
        successRaidCount = 0;
        failedRaidCount = 0;
        day = 1;
        globalReputation = 0;
        k = 1 + 0.3 * (day - 1);
        maxReadedRaidID = 0;
        isPaused = false;
        isQuitting = false;
        randomMessages = null;
    }

    public void SetRandomMessages()
    {
        randomMessages = new List<string>[allCitizens.Count+1];
        for (int i = 0; i < allCitizens.Count + 1; i++)
        {
            randomMessages[i] = new List<string>();
        }
    }

}


