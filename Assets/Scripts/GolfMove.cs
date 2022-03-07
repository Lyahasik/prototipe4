using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class GolfMove : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject[] TargetLuses;
    public GameObject[] CheckPoints;
    public Image ImagePower;
    public Image ImageBackPower;
    public GameObject Arrow;

    private bool _move = false;
    public int _currentLuse = 0;
    private bool _successLuse = false;

    private Rigidbody _rb;
    private bool _startPower = false;
    private int _rezusPower = 1;
    private float _power = 0.5f;
    private float _y = 1.0f;
    private float _currentAngle = -15.0f;
    public bool _checkFreeze = false;
    private float _freezeValue = 0.2f;
    private float _delayFreeze = 20.0f;
    private float _timeFreeze;

    private float _delayStop = 7.0f;
    private bool _freeze = true;

    private SelectedClub _selectedClub;
    
    // UI

    public GameObject[] InfoLuses;
    public Text TextShot;

    public GameObject MenuSuccess;
    public GameObject MenuScore;

    public Text TextRank;
    
    public Text[] TextScoreLuses;
    public int[] ParLuses;
    public int CountShot = 1;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _selectedClub = GetComponent<SelectedClub>();
        
        Spawn();
        WriteLuse();
    }


    void Update()
    {
        if (!_successLuse)
        {
            if (GameManager.GodMode == true)
            {
                _startPower = false;
                OffUI();
            }
            else
            {
                InputKey();
            }

            SwitchPower();
        }
        else
        {
            if (_currentLuse == TargetLuses.Length - 1)
            {
                CountScoreLuse();
                MenuScore.SetActive(true);
            }
            else if (Input.GetKeyUp("return"))
            {
                if (_currentLuse < TargetLuses.Length - 1)
                {
                    CountScoreLuse();
                    
                    _currentLuse++;
                    _successLuse = false;
                    GameManager.GodMode = false;
                    _checkFreeze = false;
                    
                    MenuSuccess.SetActive(false);
                    WriteLuse();
                    
                    CountShot = 0;
                    UpCountShot();
                    
                    Spawn();
                }
            }
        }
        
        CheckMove();
    }

    void Spawn()
    {
        _move = false;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = CheckPoints[_currentLuse].transform.position;
        transform.LookAt(TargetLuses[_currentLuse].transform);
        transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
    }

    public bool IsSuccess()
    {
        return _successLuse;
    }

    void InputKey()
    {
        if (!_startPower && !_move)
        {
            OnUI();

            if (Input.GetKeyUp("up")
                && _currentAngle > -45)
            {
                _currentAngle -= 15;
                _y += 1.0f;
                Arrow.transform.eulerAngles = new Vector3(_currentAngle, Arrow.transform.eulerAngles.y, Arrow.transform.eulerAngles.z);
            }
            if (Input.GetKeyUp("down")
                && _currentAngle < -15)
            {
                _currentAngle += 15;
                _y -= 1.0f;
                Arrow.transform.eulerAngles = new Vector3(_currentAngle, Arrow.transform.eulerAngles.y, Arrow.transform.eulerAngles.z);
            }
            
            transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * 5.0f, Space.World);
        }
        
        if (Input.GetKeyDown("space"))
        {
            if (_startPower)
            {
                _move = true;
                
                _rb.useGravity = true;
                _rb.AddForce(Vector3.Normalize(new Vector3(transform.forward.x * _selectedClub.ClubPowerZ(), _y * _selectedClub.ClubPowerY(), transform.forward.z * _selectedClub.ClubPowerZ())) * _power, ForceMode.Impulse);
                Invoke("OnCheckFreeze", 1.0f);
                
                _timeFreeze = Time.time + _delayFreeze;
                _startPower = false;
                OffUI();
            }
            else
            {
                StartPower();
            }
        }

        if (Input.GetKeyDown("tab"))
        {
            MenuScore.SetActive(true);
        }
        
        if (Input.GetKeyUp("tab"))
        {
            MenuScore.SetActive(false);
        }
    }

    void StartPower()
    {
        if (_move == false)
        {
            _startPower = true;
        }
    }

    void SwitchPower()
    {
        if (_startPower)
        {
            _power = Mathf.Clamp(_power + 1.0f * Time.deltaTime * _rezusPower, 0.5f, 3.0f);

            if (_power == 3.0f)
            {
                _rezusPower = -1;
            }
            else if (_power == 0.5f)
            {
                _rezusPower = 1;
            }

            ImagePower.fillAmount = (_power - 0.5f) / 2.5f;
        }
    }

    public bool GetMove()
    {
        return _move;
    }

    void OnCheckFreeze()
    {
        _checkFreeze = true;
    }

    void CheckMove()
    {
        if (_move && !_successLuse // bug?
            && (_timeFreeze < Time.time
            || _checkFreeze == true && Vector3.Magnitude(_rb.velocity) < _freezeValue))
        {
            GameManager.GodMode = false;
            _move = false;
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.LookAt(TargetLuses[_currentLuse].transform);
            transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
            OnUI();
            _checkFreeze = false;
            
            UpCountShot();
        }
    }

    void OnUI()
    {
        _power = 0.5f;
        ImagePower.fillAmount = 0.0f;
        ImageBackPower.enabled = true;
        ImagePower.enabled = true;
        Arrow.SetActive(true);
    }
    
    void OffUI()
    {
        ImageBackPower.enabled = false;
        ImagePower.enabled = false;
        Arrow.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish")
            && other.gameObject == TargetLuses[_currentLuse])
        {
            _successLuse = true;
            
            if (_currentLuse != TargetLuses.Length - 1)
            {
                SuccessRank();
            }
        }
    }
    
    // UI
    
    void WriteLuse()
    {
        if (_currentLuse > 0)
        {
            InfoLuses[_currentLuse - 1].SetActive(false);
            InfoLuses[_currentLuse].SetActive(true);
        }
    }

    void CountScoreLuse()
    {
        TextScoreLuses[_currentLuse].text = CountShot.ToString();
    }

    void UpCountShot()
    {
        CountShot++;
        TextShot.text = "Shot " + CountShot;
    }

    void SuccessRank()
    {
        if (CountShot == 1)
        {
            TextRank.text = "Ace !";
        }
        else if (ParLuses[_currentLuse] - CountShot >= 3)
        {
            TextRank.text = "Albatross !";
        }
        else if (ParLuses[_currentLuse] - CountShot == 2)
        {
            TextRank.text = "Eagle !";
        }
        else if (ParLuses[_currentLuse] - CountShot >= 0)
        {
            TextRank.text = "Birdie !";
        }
        else if (ParLuses[_currentLuse] - CountShot < 0)
        {
            TextRank.text = "Above par +" + (ParLuses[_currentLuse] - CountShot) + "!";
        }
        
        MenuSuccess.SetActive(true);
    }
}
