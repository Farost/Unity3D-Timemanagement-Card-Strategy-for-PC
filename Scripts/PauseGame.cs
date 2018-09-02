using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PauseGame : MonoBehaviour {

    public ButtonsManager butManager;
    public Button menuButton;
    public GameController gameController;

    public Transform canvas;
    public CanvasGroup confirmCanvas;
    public CanvasGroup kickConfirmCanvas;

    private void Start()
    {
        if (canvas.gameObject.activeInHierarchy)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void Update () {
        Pause(0);
    }

    public void Pause(int check)
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || check == 1) && !confirmCanvas.gameObject.activeInHierarchy)
        {
            if (!canvas.gameObject.activeInHierarchy)
            {
                gameController.dataController.globalData.isPaused = true;
                if (kickConfirmCanvas.gameObject.activeInHierarchy)
                {
                    kickConfirmCanvas.gameObject.SetActive(false);
                }
                canvas.gameObject.SetActive(true);

                butManager.DisableButtons();
                if (!gameController.IsAnyBlockOpened())
                {
                    Time.timeScale = 0;
                }
            }
            else
            {
                gameController.dataController.globalData.isPaused = false;
                canvas.gameObject.SetActive(false);
                if (!gameController.IsAnyBlockOpened())
                {
                    butManager.EnableButtons();
                    Time.timeScale = 1;
                }
                else
                {
                    butManager.EnableConfirmButtons();
                    Debug.Log("Enabling");
                    butManager.EnableRaidButtons();
                }
            }
        }
    }
}
