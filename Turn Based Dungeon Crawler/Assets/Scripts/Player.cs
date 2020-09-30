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

    private void OnPlayerTurn()
    {
        Debug.Log("player turn");

        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            canMove = false;

            if (Input.GetKeyDown(KeyCode.W))
            {
                movement.MoveForward(OnPlayerTurn);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                movement.MoveBack(OnPlayerTurn);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                movement.MoveRight(OnPlayerTurn);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                movement.MoveLeft(OnPlayerTurn);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                movement.RotateRight(OnPlayerTurn);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                movement.RotateLeft(OnPlayerTurn);
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                combat.Attack(OnPlayerTurn);
            }
            else canMove = true;
        }
    }
}
