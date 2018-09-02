using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitConfirmation : MonoBehaviour {

    //Необходимо добавление CanvasGroup меню паузы, поскольку нам еще рано его закрывать
    //в игре - мы лишь ждем подтверждения
    public CanvasGroup pauseCanvas;
    public CanvasGroup myCanvas;

    public GameController gameController;

    public ButtonsManager buttonManager;
    private Queue<Button> interButtons;

    //Ключ для того, чтобы отличить закрытие игры на "крестик" и на кнопку в игре
    public int key = 0;

    void OnApplicationQuit()
    {
        if (key == 0)
        {
            Application.CancelQuit();
            QuittingGame();
        }
        else
        {
            YesConfirm();
        }
    }

    private void Awake()
    {
        interButtons = new Queue<Button>();
    }


    public void QuittingGame()
    {
        gameController.dataController.globalData.isQuitting = true;
        if (!gameController.IsAnyBlockOpened())
        {
            buttonManager.DisableRightMenuButtons();
            Time.timeScale = 0;
        }

        interButtons.Clear();
        foreach (Button button in buttonManager.gameButtons)
        {
            if (button.IsInteractable())
            {
                interButtons.Enqueue(button);
            }
        }
        foreach (Button button in buttonManager.confirmacceptionButtons)
        {
            if (button.IsInteractable())
            {
                interButtons.Enqueue(button);
            }
        }

        foreach (Button button in interButtons)
        {
            button.interactable = false;
        }

        if (!myCanvas.gameObject.activeInHierarchy)
        {
            myCanvas.gameObject.SetActive(true);
        }

        buttonManager.DisablePauseButtons();
        key = 1;
    }

    public void NotConfirm()
    {
        foreach (Button button in interButtons)
        {
            button.interactable = true;
        }
        interButtons.Clear();

        if (!gameController.IsAnyBlockOpened() && !pauseCanvas.gameObject.activeInHierarchy)
        {
            buttonManager.EnableRightMenuButtons();
            Time.timeScale = 1;
        }

        buttonManager.EnablePauseButtons();
        myCanvas.gameObject.SetActive(false);
        key = 0;
        gameController.dataController.globalData.isQuitting = false;
    }

    public void YesConfirm()
    {
        Application.Quit();
    }
}
