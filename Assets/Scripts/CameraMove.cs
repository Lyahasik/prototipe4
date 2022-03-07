using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameManager GameManager;
    public GolfMove GolfMove;
    public GameObject GolfHook;
    
    [Space] public float SpeedMove = 1.0f;
    public float SpeedRotate = 1.0f;

    private Vector3 _vectorMove;

    void Update()
    {
        InputKey();
        Move();
    }

    void InputKey()
    {
        if (GameManager.GodMode
            && !GolfMove.IsSuccess())
        {
            _vectorMove = new Vector3(Input.GetAxis("Horizontal") * SpeedMove, 0, Input.GetAxis("Vertical") * SpeedMove);

            if (Input.GetKey("e"))
            {
                _vectorMove += new Vector3(0, SpeedMove * Time.deltaTime, 0);
            }
            if (Input.GetKey("q"))
            {
                _vectorMove += new Vector3(0, -SpeedMove * Time.deltaTime, 0);
            }

            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * SpeedRotate, Space.World);
            transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * -SpeedRotate);
        }
        else
        {
            _vectorMove = Vector3.zero;
        }
    }

    void Move()
    {
        transform.Translate(_vectorMove);

        if (GameManager.GodMode == false
            && GolfMove.GetMove() == false)
        {
            HookGolf();
        }
    }

    public void HookGolf()
    {
        transform.position = GolfHook.transform.position;
        transform.rotation = GolfHook.transform.rotation;
    }
}
