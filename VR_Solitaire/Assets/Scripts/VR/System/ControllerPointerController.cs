using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerPointerController : VRTK_UIPointer
{
    public ControllerPointerController leftController; // The left controller
    public ControllerPointerController rightController; // The right controller

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckUIPointerState();

        //CheckPlayerGazing(leftController.transform.parent.GetComponentInChildren<CheckIfGazed>());
        //CheckPlayerGazing(rightController.transform.parent.GetComponentInChildren<CheckIfGazed>());
    }

    ///// <summary>
    ///// Check if the player is gazing at this controller
    ///// </summary>
    //public void CheckPlayerGazing(CheckIfGazed gazable)
    //{
    //    if (gazable.isBeingGazed) // If the controller is being gazed by the player, then highlight the trigger button
    //    {
    //        gazable.transform.parent.GetComponentInChildren<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.yellow, 0.5f);
    //        gazable.transform.parent.GetComponentInChildren<VRTK_ControllerActions>().SetControllerOpacity(0.8f);
    //    }
    //    else
    //    {
    //        gazable.transform.parent.GetComponentInChildren<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.black, 0.5f);
    //        gazable.transform.parent.GetComponentInChildren<VRTK_ControllerActions>().SetControllerOpacity(1f);
    //    }
    //}

    /// <summary>
    /// Checking if the UIPointer is active for the left and right controller
    /// </summary>
    public void CheckUIPointerState()
    {
        if (leftController.PointerActive() && leftController.activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined) // If the left UIPointer is just actived
        {
            TurnOnUIPointer(leftController);
            leftController.beamEnabledState = false;
        }

        // If the left UIPointer is actived and the activate button is released, then the UIPointer can select and press canvas UI elements
        if (leftController.activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined && !leftController.controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TriggerClick))
        {
            leftController.beamEnabledState = true;
        }

        if (rightController.PointerActive() && rightController.activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined) // If the right UIPointer is actived
        {
            TurnOnUIPointer(rightController);
            rightController.beamEnabledState = false;
        }

        // If the right UIPointer is actived and the activate button is released, then the UIPointer can select and press canvas UI elements
        if (rightController.activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined && !rightController.controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TriggerClick))
        {
            rightController.beamEnabledState = true;
        }
    }

    /// <summary>
    /// Turn on related UIPointer elements
    /// </summary>
    public void TurnOnUIPointer(VRTK_UIPointer UIPointer)
    {
        // Change the function of Trigger_Click from activate UIPointer to select UICanvas elements
        UIPointer.activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        UIPointer.selectionButton = VRTK_ControllerEvents.ButtonAlias.TriggerClick;

        // Make the pointer renderer always visible
        UIPointer.GetComponent<VRTK_StraightPointerRenderer>().tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;
        UIPointer.GetComponent<VRTK_StraightPointerRenderer>().cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOn;

        if (UIPointer == leftController) // If the left UIPointer is turned on then the right hand UIPointer should turns off
        {
            TurnOffUIPointer(rightController);
        }
        else
        {
            TurnOffUIPointer(leftController);
        }
    }

    /// <summary>
    /// Turn off related UIPointer elements
    /// </summary>
    public void TurnOffUIPointer(ControllerPointerController UIPointer)
    {
        // Change the function of Trigger_Click from select UICanvas elements to activate UIPointer
        UIPointer.activationButton = VRTK_ControllerEvents.ButtonAlias.TriggerClick;
        UIPointer.selectionButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        // Make the pointer renderer always invisible for the right hand UIPointer
        UIPointer.GetComponent<VRTK_StraightPointerRenderer>().tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
        UIPointer.GetComponent<VRTK_StraightPointerRenderer>().cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;

        UIPointer.beamEnabledState = false;
    }
}
