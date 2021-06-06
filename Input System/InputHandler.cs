using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

//Class containing custom input handling methods
//Made to work with the new unity input system
public class InputHandler : MonoBehaviour
{
    public enum Devices
    {
        KBM,
        Gamepad
    }
    public static Devices CurrentDevice;
    public static void UpdateCurrentDevice(string DeviceName)
    {
        if(DeviceName.Equals("Controller"))
        {
            CurrentDevice = Devices.Gamepad;
        }
        else
        {
            CurrentDevice = Devices.KBM;
        }
    }
    public static bool GetAction(InputAction Action)
    {      
        if(Action.activeControl != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool GetActonDown(InputAction Action)
    {

        return false;
    }
}
