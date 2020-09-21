using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMapObject
{
    [SerializeField] private float speed = 6.0f;

    public MapChar MapChar => MapChar.Player;

    public void Move(Vector3 to)
    {
        StartCoroutine(MoveInternal(to));
    }

    private IEnumerator MoveInternal(Vector3 to)
    {
        for (float t = 0; t < 1; t += speed * Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, to, t);

            yield return null;
        }
    }
}
