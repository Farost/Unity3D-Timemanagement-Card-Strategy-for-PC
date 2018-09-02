using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandMesManager : MonoBehaviour {

    private static System.Random rnd = new System.Random();

    public Animator animator;
    public Text messageText;
    public Image citizenImage;
    public DataController dataController;
    private string randMessage;
    private float timerToClose;  //Ожидание до закрытия появившегося
    private float timerOfWaiting; //Ожидание до появления нового
    private float startTime; //Время появления нового сообщения или закрытия прошлого

    private void Start () {
        randMessage = "Рандомное сообщение, которое содержит множество интересных позиций и понятий о жизни и смысле жизни";
        animator.SetBool("IsOpen", false);
        startTime = 0.0f;
        timerToClose = 0.0f;
        timerOfWaiting = 13.0f;
    }

    private void Update()
    {
        if (animator.GetBool("IsOpen") && Time.timeScale != 0)
        {
            UpdateTimer(out timerToClose);
        }

        if (timerToClose <= 0.0f && animator.GetBool("IsOpen") && Time.timeScale != 0)
        {
            CloseMessage();
        }

        if (timerOfWaiting > 0.0f && Time.timeScale != 0)
        {
            UpdateWaitingTimer(ref timerOfWaiting);
        }
        else
        {
            if (!animator.GetBool("IsOpen") && Time.timeScale != 0)
            {
                PushMessage();
            }
        }
    }

    public void UpdateTimer(out float timer)
    {
        float t = Time.time - startTime;
        timer = 5.0f - t;
    }

    public void UpdateWaitingTimer (ref float timer)
    {
        float t = Time.time - startTime;
        timer -= t/20;
    }

    public void PushMessage()
    {
        randMessage = GetRandMessage();
        animator.SetBool("IsOpen", true);
        StopAllCoroutines();
        timerToClose = 15.0f;
        startTime = Time.time;
        StartCoroutine(TypeSentence(randMessage));
    }

    public string GetRandMessage()
    {
        int i = rnd.Next(0, dataController.globalData.randomMessages.Length - 1);
        Citizen citizen = dataController.GetAliveCitizenByID(i);
        if (citizen == null) i = 0;
        int j = rnd.Next(0, dataController.globalData.randomMessages[i].Count-1);
        if (i == 0)
        {
            int k = rnd.Next(0, dataController.globalData.inObshinaCitizens.Count - 1);
            citizenImage.sprite = dataController.globalData.inObshinaCitizens[k].GetSprite();
        }
        else citizenImage.sprite = citizen.GetSprite();
        return dataController.globalData.randomMessages[i][j];
    }

    IEnumerator TypeSentence(string message)
    {
        messageText.text = "";
        foreach(char letter in message.ToCharArray())
        {
            messageText.text += letter;
            yield return null;
        }
    }

    public void CloseMessage()
    {
        animator.SetBool("IsOpen", false);
        timerOfWaiting = rnd.Next(20, 200) + rnd.Next(0, 9) / 10;
        startTime = Time.time;
    }
    
}
