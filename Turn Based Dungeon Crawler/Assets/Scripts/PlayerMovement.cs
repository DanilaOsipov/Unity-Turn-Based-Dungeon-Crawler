using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f;

    public MapChar MapChar => MapChar.Player;

    private void Start()
    {
        GameObject target = GameObject.Find("Enemy Prefab(Clone)");

        Debug.Log("target " + target != null);

        var path = Pathfinding.Instance.FindPath(transform, target.transform);

        if (path == null)
        {
            Debug.Log("path null");
        }
        else
        foreach (var node in path)
        {
            Debug.Log(node.Position);
        }
    }

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
