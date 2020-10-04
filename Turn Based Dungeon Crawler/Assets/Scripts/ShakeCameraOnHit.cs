using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraOnHit : MonoBehaviour
{
    private CameraShake cameraShake;
    private CameraBob cameraBob;

    private void Start()
    {
        cameraShake = Camera.main.transform.GetComponent<CameraShake>();
        cameraBob = cameraShake.transform.GetComponent<CameraBob>();
    }

    private void Shake()
    {
        cameraBob.ResetSpeed();

        Vector2 direction = Random.insideUnitCircle;
        cameraShake.ShakeRotateCamera(direction, () =>
        {
            cameraBob.SetIdleSpeed();

            Messenger.Broadcast(GameEvent.PLAYER_TURN);
        });
    }
}
