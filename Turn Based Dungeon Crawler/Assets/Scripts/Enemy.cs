using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyCombat))]
public class Enemy : MonoBehaviour
{
    public EnemyCombat Combat { get; private set; }
    public EnemyMovement Movement { get; private set; }

    public int startRoomId;
    public GameObject target;

    private void Start()
    {
        Combat = GetComponent<EnemyCombat>();
        Movement = GetComponent<EnemyMovement>();
        Movement.target = target;

        Messenger<int>.AddListener(GameEvent.PLAYER_ENTERED_THE_ROOM, OnPlayerEnteredTheRoom);
    }

    private void OnPlayerEnteredTheRoom(int roomId)
    {
        if (startRoomId == roomId)
        {
            BattleSystem.Enemies.Add(this);
        }
    }
}
