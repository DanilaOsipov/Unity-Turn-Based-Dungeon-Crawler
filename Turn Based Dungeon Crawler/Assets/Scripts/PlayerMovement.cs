using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f;

    private MapChar MapChar => MapChar.Player;

    private bool canMove = true;

    private void Update()
    {
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(transform.forward);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Move(-transform.forward);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(transform.right);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Move(-transform.right);
            }
        }
    }

    private void Move(Vector3 dir)
    {
        if (Pathfinding.Instance.CanMove(transform, dir, out Vector3 movement))
        {
            Pathfinding.Instance.UpdateObjectOnMap(transform, MapChar, movement);

            Vector3 start = transform.localPosition;
            Vector3 direction = movement - start;
            movement = start + direction;
            movement *= DungeonGenerator.MovementOffset;
            movement.y = transform.position.y;

            StartCoroutine(MoveInternal(movement));
        }
    }

    private IEnumerator MoveInternal(Vector3 to)
    {
        canMove = false;

        for (float t = 0; t < 1; t += speed * Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, to, t);

            yield return null;
        }

        canMove = true;
    }
}
