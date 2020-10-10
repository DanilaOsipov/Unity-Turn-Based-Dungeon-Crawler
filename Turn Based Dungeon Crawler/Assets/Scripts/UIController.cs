using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private UIElement playerControls;
    [SerializeField] private UIElement forward;
    [SerializeField] private UIElement back;
    [SerializeField] private UIElement right;
    [SerializeField] private UIElement left;
    [SerializeField] private UIElement rotateLeft;
    [SerializeField] private UIElement rotateRight;
    [SerializeField] private UIElement attack;
    [SerializeField] private UIElement exit;
    [SerializeField] private GamePopup exitPopup;
    [SerializeField] private UIElement pause;
    [SerializeField] private GamePopup pausePopup;
    [SerializeField] private GamePopup deathPopup;

    public static HealthBar HealthBar { get; private set; }
    public static UIElement PlayerControls { get; private set; }
    public static UIElement Forward { get; private set; }
    public static UIElement Back { get; private set; }
    public static UIElement Right { get; private set; }
    public static UIElement Left { get; private set; }
    public static UIElement RotateLeft { get; private set; }
    public static UIElement RotateRight { get; private set; }
    public static UIElement Attack { get; private set; }
    public static UIElement Exit { get; private set; }
    public static GamePopup ExitPopup { get; private set; }
    public static UIElement Pause { get; private set; }
    public static GamePopup PausePopup { get; private set; }
    public static GamePopup DeathPopup { get; private set; }

    private void Awake()
    {
        HealthBar = healthBar;
        PlayerControls = playerControls;
        Forward = forward;
        Back = back;
        Right = right;
        Left = left;
        RotateLeft = rotateLeft;
        RotateRight = rotateRight;
        Attack = attack;
        Exit = exit;
        ExitPopup = exitPopup;
        Pause = pause;
        PausePopup = pausePopup;
        DeathPopup = deathPopup;

        DeathPopup.Hide();
        PausePopup.Hide();
        ExitPopup.Hide();
    }
}
