using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    private MapChar MapChar => MapChar.Player;

    private CameraBob cameraBob;
    private PlayerSound playerSound;

    private void Start()
    {
        cameraBob = GetComponentInChildren<CameraBob>();
        playerSound = GetComponentInChildren<PlayerSound>();
    }

    public void MoveRight(Action callback)
    {
        Move(transform.right, callback);
    }

    public bool CanMoveRight()
    {
        return Pathfinding.Instance.CanMove(transform, transform.right, out Vector3 movement);
    }

    public void MoveLeft(Action callback)
    {
        Move(-transform.right, callback);
    }

    public bool CanMoveLeft()
    {
        return Pathfinding.Instance.CanMove(transform, -transform.right, out Vector3 movement);
    }

    public void MoveForward(Action callback)
    {
        Move(transform.forward, callback);
    }

    public bool CanMoveForward()
    {
        return Pathfinding.Instance.CanMove(transform, transform.forward, out Vector3 movement);
    }

    public void MoveBack(Action callback)
    {
        Move(-transform.forward, callback);
    }

    public bool CanMoveBack()
    {
        return Pathfinding.Instance.CanMove(transform, -transform.forward, out Vector3 movement);
    }

    public void RotateLeft(Action callback)
    {
        StartCoroutine(RotateInternal(transform.rotation * Quaternion.Euler(0, -90, 0), callback));
    }

    public void RotateRight(Action callback)
    {
        StartCoroutine(RotateInternal(transform.rotation * Quaternion.Euler(0, 90, 0), callback));
    }

    private IEnumerator RotateInternal(Quaternion targetRotation, Action callback)
    {
        Quaternion startRotation = transform.rotation;

        for (float t = 0; t < 1; t += rotationSpeed * Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;

        callback();
    }

    private void Move(Vector3 dir, Action callback)
    {
        if (Pathfinding.Instance.CanMove(transform, dir, out Vector3 movement))
        {
            Pathfinding.Instance.UpdateObjectOnMap(transform, MapChar, movement);

            Vector3 start = transform.localPosition;
            Vector3 direction = movement - start;
            movement = start + direction;
            movement *= DungeonGenerator.MovementOffset;
            movement.y = transform.position.y;

            StartCoroutine(MoveInternal(movement, callback));
        }
        else callback();
    }

    private IEnumerator MoveInternal(Vector3 to, Action callback)
    {
        Vector3 from = transform.localPosition;
        
        cameraBob.SetWalkingSpeed();

        playerSound.Step(() => { });

        for (float t = 0; t < 1; t += movementSpeed * Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(from, to, t);

            yield return null;
        }

        cameraBob.SetIdleSpeed();

        if (!BattleSystem.BattleInProcess) callback();
        else Messenger.Broadcast(GameEvent.ENEMY_TURN);
    }
}
