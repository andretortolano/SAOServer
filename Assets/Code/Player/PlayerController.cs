using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;
    // public Interactable currentFocus;

    Camera cam;
    PlayerMotor motor;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(EventSystem.current.IsPointerOverGameObject())
        //    return;

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100, movementMask))
            {
                Debug.Log("We Hit " + hit.collider.name + " at point: " + hit.point);
                // Move the player to what we hit
                motor.MoveToPoint(hit.point);
                // Stop focusing any objects
                // RemoveFocus();
            }
        }

        //if(Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if(Physics.Raycast(ray, out hit, 100))
        //    {
        //        // Check if we hit a interactable and focus it
        //        Interactable interactable = hit.collider.GetComponent<Interactable>();
        //        if(interactable != null)
        //        {
        //            SetFocus(interactable);
        //        }
        //    }
        //}
    }

    //void SetFocus(Interactable newFocus) {
    //    Debug.Log("new Focus: " + newFocus.name + "/ currentFocus: " + currentFocus);
    //    if(newFocus != currentFocus)
    //    {
    //        if(currentFocus != null)
    //        {
    //            currentFocus.OnDefocused();
    //        }

    //        currentFocus = newFocus;
    //        newFocus.OnFocused(transform);
    //    }

    //    motor.FollowTarget(newFocus);

    //}

    //void RemoveFocus() {
    //    if(currentFocus != null)
    //    {
    //        currentFocus.OnDefocused();
    //    }
        
    //    currentFocus = null;
    //    motor.StopFollowingTarget();
    //}
}
