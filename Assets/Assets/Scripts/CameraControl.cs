using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class Camera : MonoBehaviour
{
    [Header("Camera Mode")]
    public bool RTSMode = true;
    public bool FlyCam = false;

    [Header("Camera Movement")]
    [SerializeField] public float moveSpeed = 10f;

    [Header("Camera Rotation")]
    private float dirX;
    private float dirY;
    [SerializeField] private float rotateSpeed = 50f / 50;
    private bool isMouseClick = false;

    [Header("Tracking Target")] 
    [SerializeField] private Transform target;
    public Transform Target => target;

    [Header("Mouse Sensitivity")]
    [Tooltip("Mouse Sensitivity")] [SerializeField] public float mouseSensitivity = 10f;
    
    private Rigidbody rigid;
    
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    /// Update is called once per frame
    void Update()
    {
        Movement();
        Rotation();
        if (RTSMode == true && Target != null)
        {
            TrackingTargetRotate();
        }
        else return;
    }
    
    //Basic movement for camera
    private void Movement()
    {
        KeyControl[] controls = new []
        {
            Keyboard.current.wKey,
            Keyboard.current.sKey,
            Keyboard.current.aKey,
            Keyboard.current.dKey,
            Keyboard.current.leftShiftKey,
            Keyboard.current.spaceKey,
        };
        Vector3[] directions = new[]
        {
            transform.forward,
            -transform.forward,
            -transform.right,
            transform.right,
            -transform.up,
            transform.up,
        };
        for (int i = 0; i < controls.Length; i++)
        {
            if (controls[i].isPressed)
            {
                transform.position += directions[i] * moveSpeed * Time.deltaTime;
            }
        }
    }
    private void Rotation() {
        if(Mouse.current.middleButton.isPressed) {     
            // Get mouse movement delta (new Input System)
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            
            // Apply rotation with sensitivity
            dirY += mouseDelta.x * mouseSensitivity * Time.deltaTime; // Horizontal movement around the x axis
            dirX += mouseDelta.y * mouseSensitivity * Time.deltaTime; // Vertical movement around the y axis
            
            dirX = Mathf.Clamp(dirX, -45f, 80f); // return dirX value between -90 and 90 
            
            // Apply rotation
            transform.localRotation = Quaternion.Euler(dirX, dirY, 0);
        }  
    }
    private void TrackingTargetRotate()
    {
        Vector3 targetDir = (Target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(targetDir);
    }
    
}
