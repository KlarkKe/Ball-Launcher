using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSprintJoint;
    private Camera mainCamera;
    private bool isDragging;

    private void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    void Update()
    {
        if (currentBallRigidbody == null) { return; }

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            return;
        }

        isDragging = true;
        currentBallRigidbody.bodyType = RigidbodyType2D.Kinematic;

        Vector2 touchPositon = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositon);

        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSprintJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.bodyType = RigidbodyType2D.Dynamic;
        currentBallRigidbody = null;

        Invoke("DetachBall", detachDelay);
    }

    private void DetachBall()
    {
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
