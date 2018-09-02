using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayClocks : MonoBehaviour {

    public Image background;
    public Sprite[] sprites;
    public Image[] images;
    private int[] clocks;
    public string clocksString;

    public GameController gameController;
    private float startTime;
    private bool isStopped = false;
    private int startHours;

	void Start () {
        startTime = 0;
        startHours = 6;
        clocks = new int[4];
        clocksString = "";
        background.enabled = true;
        Time.timeScale = 1;

    }
	
	void Update () {
        if (isStopped) return;

        CalculateClocks();
        UpdateClocks();
        CheckEndDay();
	}

    public void CalculateClocks()
    {
        float t = Time.time - startTime;

        int hours = startHours + (int)t / 10;
        if (hours < 10)
        {
            clocks[0] = 0;
            clocks[1] = hours;
        }
        else
        {
            clocks[0] = (int)hours / 10;
            clocks[1] = hours % 10;
        }

        int minutes = (int)((t % 10) * 6);
        if (minutes < 10)
        {
            clocks[2] = 0;
            clocks[3] = minutes;
        }
        else
        {
            clocks[2] = (int)minutes / 10;
            clocks[3] = minutes % 10;
        }
        clocksString = "";
        for (int i = 0; i < 4; i++)
        {
            clocksString += clocks[i].ToString();
        }
    }

    public void UpdateClocks()
    {
        for (int i = 0; i < 4; i++)
        {
            images[i].sprite = sprites[clocks[i]];
        }
    }


    public void CheckEndDay()
    {
        if (clocks[0] == 1 && clocks[1] == 7 && clocks[2] == 5 && clocks[3] == 9)
        {
            EndDay();
        }
    }

    public void EndDay()
    {
        gameController.OpenPomoshnikWindow();
        gameController.buttonsManager.gameButtons[4].gameObject.SetActive(false);
        gameController.helperText.text = "\n\n\nЭто конец первого игрового дня. Спасибо за то, что сыграли в эту демо-версию. Надеюсь, багов не было. До свидания.";
        Time.timeScale = 0;
        isStopped = true;
    }
        
}
