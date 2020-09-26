using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int startRoomId;

    private void Start()
    {
        Messenger<int>.AddListener(GameEvent.PLAYER_ENTERED_THE_ROOM, OnPlayerEnteredTheRoom);
    }

    private void OnPlayerEnteredTheRoom(int roomId)
    {
        if (startRoomId == roomId)
            Debug.Log("go! from " + startRoomId);
    }

    private void Update()
    {
        
    }
}
