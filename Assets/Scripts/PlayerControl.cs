using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public Transform mainCameraTransform;
    public float moveSpeed = 7;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;

    float smoothInputMagnitude;
    float smoothVelocity;
    float angle;
    Vector3 velocity;
    bool disabled;
    Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Guard.OnGuardSpotPlayer += Disable;
    }

    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, CrossPlatformInputManager.GetAxisRaw("Vertical")).normalized;
        }
        
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude,ref smoothVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * smoothInputMagnitude * moveSpeed;
    }

    private void FixedUpdate()
    {
        rigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidBody.MovePosition(rigidBody.position + velocity * Time.deltaTime);
        Vector3 pos = new Vector3(transform.position.x,mainCameraTransform.position.y,transform.position.z);
        mainCameraTransform.transform.position = pos;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Finish")
        {
            Disable();
            if(OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    private void Disable()
    {
        disabled = true;
    }

    void OnDestroy()
    {
        Guard.OnGuardSpotPlayer -= Disable;
    }
}
