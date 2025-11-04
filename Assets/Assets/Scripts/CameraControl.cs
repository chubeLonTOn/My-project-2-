
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Camera : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] public float moveSpeed = 10f;

    [Header("Camera Rotation")]
    private float dirX;
    private float dirY;
    [SerializeField] private float rotateSpeed = 2f;

    [Header("Tracking Target")] 
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    [Header("Mouse Sensitivity")]
    [Tooltip("Mouse Sensitivity")] [SerializeField] public float mouseSensitivity = 10f;
    public enum States
    {
        RTSMode,
        ManualControlMode
    }
    [SerializeField] private States _states;
    void Update()
    {
        StateSwitch();
    }
    private void StateSwitch()
    {
        switch (_states)
        {
            case States.ManualControlMode:
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
                    
                    //Rotate on mouse
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
                    break;
            
            case States.RTSMode:
                // Cam auto rotation
                Vector3 targetDir = (target.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(targetDir), rotateSpeed * Time.deltaTime);
                
                // Cam auto following
                Vector3 cameraMovement = target.position + offset;
                Vector3 smoothMovement = Vector3.Lerp(transform.position, cameraMovement, 0.125f);
                transform.position = smoothMovement; 
                    break;
        }
    }
    
}
