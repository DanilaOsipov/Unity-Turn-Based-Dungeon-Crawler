using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBasedMovement : MonoBehaviour {
    private enum MovementStates
    {
        Forward,
        Right,
        Back,
        Left
    }

    [SerializeField] private GameObject startGrid;
    private GameObject currentGrid;

    public float rotationSpeed = 200.0f;
    public float movementSpeed = 6.0f;

    private bool isMoving;

    // Use this for initialization
    void Start () {
        gameObject.transform.position = new Vector3(startGrid.transform.position.x,
                                                    startGrid.transform.position.y + 1.25f,
                                                    startGrid.transform.position.z);
        currentGrid = startGrid;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(transform.localEulerAngles.y);
            Debug.Log(isMoving);
        }

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ReverseMovementStatus();
                Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
                StartCoroutine(Rotate(targetRotation, rotationSpeed, ReverseMovementStatus));
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ReverseMovementStatus();
                Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
                StartCoroutine(Rotate(targetRotation, rotationSpeed, ReverseMovementStatus));
            }
            else if (Input.GetKeyDown(KeyCode.W) && CanMove(MovementStates.Forward))
            {
                ReverseMovementStatus();
                StartCoroutine(Move(MovementStates.Forward, ReverseMovementStatus));
            }
            else if (Input.GetKeyDown(KeyCode.D) && CanMove(MovementStates.Right))
            {
                ReverseMovementStatus();
                StartCoroutine(Move(MovementStates.Right, ReverseMovementStatus));
            }
            else if (Input.GetKeyDown(KeyCode.S) && CanMove(MovementStates.Back))
            {
                ReverseMovementStatus();
                StartCoroutine(Move(MovementStates.Back, ReverseMovementStatus));
            }
            else if (Input.GetKeyDown(KeyCode.A) && CanMove(MovementStates.Left))
            {
                ReverseMovementStatus();
                StartCoroutine(Move(MovementStates.Left, ReverseMovementStatus));
            }
        }
    }

    private bool CanMove(MovementStates movementState)
    {
        RaycastHit hit = new RaycastHit();
        float check = 1.5f;
        switch (movementState)
        {
            case MovementStates.Forward:
                if (!Physics.Raycast(currentGrid.transform.position, gameObject.transform.forward, out hit))
                {
                    return false;
                }
                break;
            case MovementStates.Right:
                if (!Physics.Raycast(currentGrid.transform.position, gameObject.transform.right, out hit))
                {
                    return false;
                }
                break;
            case MovementStates.Back:
                if (!Physics.Raycast(currentGrid.transform.position, -gameObject.transform.forward, out hit))
                {
                    return false;
                }
                break;
            case MovementStates.Left:
                if (!Physics.Raycast(currentGrid.transform.position, -gameObject.transform.right, out hit))
                {                                                                 
                    return false;
                }
                break;
        }
        Debug.Log(hit.distance);
        return hit.distance <= check && hit.transform.name == "MovementGrid";
               //hit.transform.gameObject.layer == LayerMask.NameToLayer("MovementGrid");
    }

    private IEnumerator Move(MovementStates movementState, Action callback)
    {
        Vector3 targetPosition = new Vector3();
        RaycastHit hit = new RaycastHit();

        switch (movementState)
        {
            case MovementStates.Forward:
                Physics.Raycast(currentGrid.transform.position, gameObject.transform.forward, out hit);
                break;
            case MovementStates.Right:
                Physics.Raycast(currentGrid.transform.position, gameObject.transform.right, out hit);
                break;
            case MovementStates.Back:
                Physics.Raycast(currentGrid.transform.position, -gameObject.transform.forward, out hit);
                break;
            case MovementStates.Left:
                Physics.Raycast(currentGrid.transform.position, -gameObject.transform.right, out hit);
                break;
        }

        targetPosition = hit.transform.position;
        targetPosition.y = transform.position.y;

        while (transform.position.x != targetPosition.x || transform.position.z != targetPosition.z)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                                                     movementSpeed * Time.deltaTime);

            yield return null;
        }
        currentGrid = hit.transform.gameObject;

        callback();
    }

    private void ReverseMovementStatus()
    {
        isMoving = !isMoving;
    }

    private IEnumerator Rotate(Quaternion targetRotation, float rotationSpeed, Action callback)
    {
        while (transform.rotation.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        callback();
    }
}
