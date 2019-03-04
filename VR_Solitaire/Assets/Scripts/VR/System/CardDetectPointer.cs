using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CardDetectPointer : MonoBehaviour
{
    public ControllerPointer controllerPointerA; // Left controller pointer
    public ControllerPointer controllerPointerB; // Right controller pointer
    public Color touchedColor; // The color when the card is touched

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (controllerPointerA == null || controllerPointerB == null)
        {
            GetPointers();
        }

        CheckPointerTouch();
    }

    public void CheckPointerTouch()
    {
        bool touched = false;

        // Check if card is touched
        if (controllerPointerA != null && controllerPointerA.GetComponent<ControllerPointer>() &&
            controllerPointerA.GetComponent<ControllerPointer>().currentTouchingCard != null && controllerPointerA.GetComponent<ControllerPointer>().currentTouchingCard == this)
        {
            touched = true;
        }
        if (controllerPointerB != null && controllerPointerB.GetComponent<ControllerPointer>() &&
            controllerPointerB.GetComponent<ControllerPointer>().currentTouchingCard != null && controllerPointerB.GetComponent<ControllerPointer>().currentTouchingCard == this)
        {
            touched = true;
        }

        // Unhightlight card
        if (!touched)
        {
            //InteractableObjectEventArgs a = new InteractableObjectEventArgs();
            //GetComponent<VRTK_InteractableObject>().OnInteractableObjectUntouched(a);
            GetComponent<SpriteRenderer>().material.color = Color.white;
        }
    }

    public void GetPointers()
    {
        if (controllerPointerA == null)
        {
            controllerPointerA = FindObjectOfType<ControllerPointer>();
        }

        if (controllerPointerB == null)
        {
            ControllerPointer[] pointers = FindObjectsOfType<ControllerPointer>();
            foreach (ControllerPointer p in pointers)
            {
                if (p != controllerPointerA)
                {
                    controllerPointerB = p;
                }
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        // If the card is pointed by a pointer
        if (col.GetComponent<VRTK_PlayerObject>() && col.GetComponent<VRTK_PlayerObject>().objectType == VRTK_PlayerObject.ObjectTypes.Pointer &&
            col.GetComponent<SphereCollider>() && !col.GetComponent<MeshRenderer>())
        {
            //InteractableObjectEventArgs a = new InteractableObjectEventArgs();
            //GetComponent<VRTK_InteractableObject>().OnInteractableObjectTouched(a);
            GetComponent<SpriteRenderer>().material.color = touchedColor;

            // Create initial ControllerPointer component
            if (!col.GetComponent<ControllerPointer>())
            {
                col.gameObject.AddComponent<ControllerPointer>();
                col.GetComponent<ControllerPointer>().currentTouchingCard = this;
            }
            else
            {
                col.GetComponent<ControllerPointer>().currentTouchingCard = this;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        // If the pointer leaves the card
        if (col.GetComponent<VRTK_PlayerObject>() && col.GetComponent<VRTK_PlayerObject>().objectType == VRTK_PlayerObject.ObjectTypes.Pointer &&
            col.GetComponent<SphereCollider>() && !col.GetComponent<MeshRenderer>())
        {
            GetComponent<SpriteRenderer>().material.color = Color.white;

            col.GetComponent<ControllerPointer>().currentTouchingCard = null;
        }
    }
}
