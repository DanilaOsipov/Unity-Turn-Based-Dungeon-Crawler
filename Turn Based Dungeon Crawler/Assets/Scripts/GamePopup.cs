using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePopup : UIElement
{
    public override void Show()
    {
        base.Show();

        UIController.PlayerControls.Hide();
        UIController.HealthBar.Hide();
        UIController.Pause.Hide();

        Time.timeScale = 0;
    }

    public override void Hide()
    {
        base.Hide();

        UIController.PlayerControls.Show();
        UIController.HealthBar.Show();
        UIController.Pause.Show();

        Time.timeScale = 1.0f;
    }
}
