using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
public class Player : MonoBehaviour
{
    PlayerCombat combat;
    PlayerMovement movement;
    private bool canMove;

    private void Start()
    {
        combat = GetComponent<PlayerCombat>();
        movement = GetComponent<PlayerMovement>();
        canMove = true;

        Messenger.AddListener(GameEvent.PLAYER_TURN, OnPlayerTurn);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.PLAYER_TURN, OnPlayerTurn);
    }

    private void OnPlayerTurn()
    {
        Debug.Log("Player turn.");

        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            if (!UIController.PlayerControls.IsVisible && 
                !UIController.ExitPopup.IsVisible &&
                !UIController.PausePopup.IsVisible &&
                !UIController.DeathPopup.IsVisible)
                UIController.PlayerControls.Show();

            if (CloseToExit())
            {
                if (!UIController.Exit.IsVisible)
                    UIController.Exit.Show();
            }
            else
            {
                if (UIController.Exit.IsVisible)
                    UIController.Exit.Hide();
            }

            if (combat.CanAttack())
            {
                if (!UIController.Attack.IsVisible)
                    UIController.Attack.Show();
            }
            else
            {
                if (UIController.Attack.IsVisible)
                    UIController.Attack.Hide();
            }

            if (movement.CanMoveBack())
            {
                if (!UIController.Back.IsVisible)
                    UIController.Back.Show();
            }
            else
            {
                if (UIController.Back.IsVisible)
                    UIController.Back.Hide();
            }

            if (movement.CanMoveForward())
            {
                if (!UIController.Forward.IsVisible)
                    UIController.Forward.Show();
            }
            else
            {
                if (UIController.Forward.IsVisible)
                    UIController.Forward.Hide();
            }

            if (movement.CanMoveRight())
            {
                if (!UIController.Right.IsVisible)
                    UIController.Right.Show();
            }
            else
            {
                if (UIController.Right.IsVisible)
                    UIController.Right.Hide();
            }

            if (movement.CanMoveLeft())
            {
                if (!UIController.Left.IsVisible)
                    UIController.Left.Show();
            }
            else
            {
                if (UIController.Left.IsVisible)
                    UIController.Left.Hide();
            }
        }
        else
        {
            if (UIController.PlayerControls.IsVisible)
                UIController.PlayerControls.Hide();
        }
    }

    public void Exit()
    {
        if (canMove)
        {
            canMove = false;

            if (CanExit(out Exit exit))
            {
                exit.ExitLevel();
            }
            else
            {
                canMove = true;

                UIController.ExitPopup.Show();
            }
        }
    }

    public void Attack()
    {
        if (canMove)
        {
            canMove = false;

            combat.Attack(OnPlayerTurn);
        }
    }

    public void MoveRight()
    {
        if (canMove)
        {
            canMove = false;

            movement.MoveRight(OnPlayerTurn);
        }
    }

    public void MoveLeft()
    {
        if (canMove)
        {
            canMove = false;

            movement.MoveLeft(OnPlayerTurn);
        }
    }

    public void MoveForward()
    {
        if (canMove)
        {
            canMove = false;

            movement.MoveForward(OnPlayerTurn);
        }
    }

    public void MoveBack()
    {
        if (canMove)
        {
            canMove = false;

            movement.MoveBack(OnPlayerTurn);
        }
    }

    public void RotateLeft()
    {
        if (canMove)
        {
            canMove = false;

            movement.RotateLeft(OnPlayerTurn);
        }
    }

    public void RotateRight()
    {
        if (canMove)
        {
            canMove = false;

            movement.RotateRight(OnPlayerTurn);
        }
    }

    private bool CloseToExit()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
            return hit.transform.GetComponent<Exit>() != null;
        }

        return false;
    }

    private bool CanExit(out Exit exit)
    {
        exit = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, DungeonGenerator.MovementOffset))
        {
            exit = hit.transform.GetComponent<Exit>();

            return exit != null && DungeonGenerator.EnemiesCount == 0;
        }

        return false;
    }
}
