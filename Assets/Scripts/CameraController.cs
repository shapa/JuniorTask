using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    const float DoubleClickTime = 0.3f;
    const float CamLimit = 80;

    public Slider speedSlider;
    public Button buttonLeft;
    public Button buttonRight;
    public float camSensitivity = 1;
    public Vector3 camOffset;

    private BallBehaviour[] _ballsList;
    private int _currentBall;
    private float _lastClick;
    private float _camX;
    private float _camY;        

    private void Awake()
    {
        _ballsList = FindObjectsOfType<BallBehaviour>();
    }

    void Start()
    {        
        speedSlider.onValueChanged.AddListener(delegate { OnSliderChange(); });
        buttonLeft.onClick.AddListener(delegate { OnButtonClick(false); });
        buttonRight.onClick.AddListener(delegate { OnButtonClick(true); });
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if ((hit.collider != null) && (hit.collider.gameObject == _ballsList[_currentBall].gameObject))
            {
                if (_ballsList[_currentBall].ballState == State.Start)
                    _ballsList[_currentBall].Launch();

                if (Time.time - _lastClick > DoubleClickTime)
                {
                    _lastClick = Time.time;
                }
                else
                    _ballsList[_currentBall].Restart();
            }
        }
        if (Input.GetMouseButton(1))
        {
            _camX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * camSensitivity;
            _camY += Input.GetAxis("Mouse Y") * camSensitivity;
            _camY = Mathf.Clamp(_camY, -CamLimit, CamLimit);
            transform.localEulerAngles = new Vector3(-_camY, _camX, 0);
            
        }
        transform.position = transform.localRotation * camOffset + _ballsList[_currentBall].transform.position;
    }

    public void OnButtonClick(bool right)
    {
        if (_ballsList[_currentBall].ballState == State.Moving)
            _ballsList[_currentBall].ballSpeed = 0;
        if (right)
            _currentBall++;
        else
            _currentBall--;
        if (_currentBall > _ballsList.Length - 1)
            _currentBall = 0;
        if (_currentBall < 0)
            _currentBall = _ballsList.Length - 1;
        speedSlider.value = _ballsList[_currentBall].ballSpeed;
    }
    public void OnSliderChange()
    {
        _ballsList[_currentBall].ballSpeed = speedSlider.value;
    }
}