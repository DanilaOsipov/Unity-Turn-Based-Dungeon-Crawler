using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    public int roomId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            Messenger<int>.Broadcast(GameEvent.PLAYER_ENTERED_THE_ROOM, roomId);
        }
    }
}
