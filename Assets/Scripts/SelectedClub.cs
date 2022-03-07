using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedClub : MonoBehaviour
{
    public GameObject[] Clubs;
    
    public float[] PowerClubsY;
    public float[] PowerClubsZ;

    private int _currentClub = 0;
    
    void Update()
    {
        InputKey();
    }

    void InputKey()
    {
        if (Input.GetKeyDown("q"))
        {
            SwitchClub();
        }
    }

    void SwitchClub()
    {
        Clubs[_currentClub].SetActive(false);
        
        _currentClub = (_currentClub + 1) % 3;
        
        Clubs[_currentClub].SetActive(true);
    }

    public float ClubPowerZ()
    {
        return PowerClubsZ[_currentClub];
    }

    public float ClubPowerY()
    {
        return PowerClubsY[_currentClub];
    }
}
