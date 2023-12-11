using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerMovementController : MonoBehaviour
{
    public Joystick Joystick;
    private RigidbodyFirstPersonController rbController;
    public FixedTouchField touchField;
    private Animator animator;
    
    void Start()
    {
        rbController = GetComponent<RigidbodyFirstPersonController>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rbController.joystickInputAxis.x = Joystick.Horizontal;
        rbController.joystickInputAxis.y = Joystick.Vertical;
        rbController.mouseLook.lookInputAxis = touchField.TouchDist;
        
        animator.SetFloat("Horizontal", Joystick.Horizontal);
        animator.SetFloat("Vertical", Joystick.Vertical);
        
        
        if (Mathf.Abs(Joystick.Horizontal) > 0.9 || Mathf.Abs(Joystick.Vertical) > 0.9)
        {
            animator.SetBool("isRunning", true);
            rbController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            animator.SetBool("isRunning", false);
            rbController.movementSettings.ForwardSpeed = 5;
        }
    }
}
