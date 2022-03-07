using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool GodMode = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        InputKey();
    }
    
    void InputKey()
    {
        if (GodMode)
        {
            if (Input.GetKeyUp("space"))
            {
                GodMode = false;
            }
        }
        else
        {
            if (Input.GetKeyDown("e"))
            {
                GodMode = true;
            }
        }
    }
}
