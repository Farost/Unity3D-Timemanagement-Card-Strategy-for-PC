using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour {

    public Button[] gameButtons;
    public Button[] rightMenuButtons;
    public Button[] confirmacceptionButtons;
    public Button[] pauseButtons;
    public Button[] raidButtons; //0 - закрытие, 1 - отправить

    private void Start()
    {
        EnableRightMenuButtons();
    }

    public void DisableButtons()
    {
        foreach(Button someButton in gameButtons)
        {
            someButton.interactable = false;
        }

        foreach (Button someButton in rightMenuButtons)
        {
            someButton.interactable = false;
        }

        foreach (Button someButton in confirmacceptionButtons)
        {
            someButton.interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach (Button someButton in gameButtons)
        {
            someButton.interactable = true;
        }

        foreach (Button someButton in rightMenuButtons)
        {
            someButton.interactable = true;
        }

        foreach (Button someButton in confirmacceptionButtons)
        {
            someButton.interactable = true;
        }

    }

    public void DisableRightMenuButtons()
    {
        foreach (Button someButton in rightMenuButtons)
        {
            someButton.interactable = false;
        }
    }

    public void EnableRightMenuButtons()
    {
        foreach (Button someButton in rightMenuButtons)
        {
            someButton.interactable = true;
        }
    }

    public void DisableConfirmButtons()
    {
        foreach (Button someButton in confirmacceptionButtons)
        {
            someButton.interactable = false;
        }
    }

    public void EnableConfirmButtons()
    {
        foreach (Button someButton in confirmacceptionButtons)
        {
            someButton.interactable = true;
        }
    }

    public void EnableGameButtons()
    {
        foreach (Button someButton in gameButtons)
        {
            someButton.interactable = true;
        }
    }

    public void DisableGameButtons()
    {
        foreach (Button someButton in gameButtons)
        {
            someButton.interactable = false;
        }
    }

    public void EnablePauseButtons()
    {
        foreach (Button someButton in pauseButtons)
        {
            someButton.interactable = true;
        }
    }

    public void DisablePauseButtons()
    {
        foreach (Button someButton in pauseButtons)
        {
            someButton.interactable = false;
        }
    }

    public void EnableRaidButtons()
    {
        raidButtons[0].interactable = true;
        raidButtons[1].interactable = true;
        raidButtons[2].interactable = true;
    }

    public void DisableRaidButtons()
    {
        raidButtons[0].interactable = false;
        raidButtons[1].interactable = false;
        raidButtons[2].interactable = false;
    }
}
