using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Raid
{
	public static System.Random rnd = new System.Random();
    public List<Citizen> members;
    public int numberInQueue;

    int id;
	string raidName;
    string raidDescription;
    int raidTime;
    Vector3 coordinates;
    int sumEfficiency;
    int maxMembers;
    int minMembers;
    int penPartial;
    int penFail;
    int typeOfViews;

    public Raid()
    {
        id = -1;
        raidName = "";
        raidDescription = "";
        raidTime = -1;
        coordinates = new Vector3(0,0,0);
        sumEfficiency = 0;
        maxMembers = 0;
        minMembers = 0;
        penPartial = -1;
        penFail = -1;
        typeOfViews = 0;
        members = new List<Citizen>();
        numberInQueue = -1;
    }

    //************Getter'ы****************

    public int GetId()
    {
        return id;
    }

    public string GetRaidName()
    {
        return raidName;
    }

    public string GetRaidDescription()
    {
        return raidDescription;
    }

    public int GetRaidTime()
    {
        return raidTime;
    }

    public Vector3 GetCoordinates()
    {
        return coordinates;
    }

    public int GetSumEfficiency()
    {
        return sumEfficiency;
    }

    public int GetMaxMembers()
    {
        return maxMembers;
    }

    public int GetMinMembers()
    {
        return minMembers;
    }

    public int GetPenPartial()
    {
        return penPartial;
    }

    public int GetPenFail()
    {
        return penFail;
    }

    public int GetTypeOfViews()
    {
        return typeOfViews;
    }

    //************Set'еры******************

    public void SetId(int index)
    {
        id = index;
    }

    public void SetRaidName(string name)
    {
        raidName = name;
    }

    public void SetRaidDescription(string description)
    {
        raidDescription = description;
    }

    public void SetRaidTime(int time)
    {
        raidTime = time;
    }

    public void SetCoordinates(Vector3 coord)
    {
        coordinates = coord;
    }

    public void SetCoordinates(int x, int y)
    {
        coordinates.x = x;
        coordinates.y = y;
        coordinates.z = 0;
    }

    public void SetSumEfficiency(int needSum)
    {
        sumEfficiency = needSum;
    }

    public void SetMaxMembers(int maxMem)
    {
        maxMembers = maxMem;
    }

    public void SetMinMembers(int minMem)
    {
        minMembers = minMem;
    }

    public void SetPenPartial(int penalty)
    {
        penPartial = penalty;
    }

    public void SetPenFail(int penalty)
    {
        penFail = penalty;
    }

    public void SetTypeOfViews(int views)
    {
        typeOfViews = views;
    }

    public bool IsRaidOkay()
    {
        if (raidName.Length > 35)
        {
            Debug.Log("Incorrect raidName in Raid id#" + id.ToString());
            return false;
        }
        if (raidDescription.Length > 150)
        {
            Debug.Log("Incorrect raidDescription in Raid id#" + id.ToString());
            return false;
        }
        if (raidTime <= 0)
        {
            Debug.Log("Incorrect raidTime in Raid id#" + id.ToString());
            return false;
        }
        if (coordinates.x > 375 || coordinates.x < -375 || coordinates.y > 200 || coordinates.y < -200)
        {
            Debug.Log("Incorrect coordinates in Raid id#" + id.ToString());
            return false;
        }
        if (sumEfficiency <= 0)
        {
            Debug.Log("Incorrect sumEfficiency in Raid id#" + id.ToString());
            return false;
        }
        if (maxMembers <= 0)
        {
            Debug.Log("Incorrect maxMembers in Raid id#" + id.ToString());
            return false;
        }
        if (minMembers <= 0)
        {
            Debug.Log("Incorrect minMembers in Raid id#" + id.ToString());
            return false;
        }
        if (penFail < 0)
        {
            Debug.Log("Incorrect penFail in Raid id#" + id.ToString());
            return false;
        }
        if (penPartial < 0)
        {
            Debug.Log("Incorrect penPartial in Raid id#" + id.ToString());
            return false;
        }
        if (maxMembers < minMembers)
        {
            Debug.Log("maxMembers < minMembers in Raid id#" + id.ToString());
            return false;
        }
        return true;
    }

    public int CountSuccessPercentage() //Процент от 0 до 100
    {
        int membersEfficiency = 0;
        foreach (Citizen member in members)
        {
            membersEfficiency += member.GetEfficiencyConsiderHealth();
        }
        return (int)(membersEfficiency * 100 / sumEfficiency);
    }
}

