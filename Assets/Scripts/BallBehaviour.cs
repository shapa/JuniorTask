using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{  
    const float Distance = 0.1f;

    public TextAsset jsonFile;    
    public LineRenderer ballTrace;
    public float ballSpeed;
    public State ballState;

    private Vector3[] _ballPath;    
    private int _currentWayPoint;   

    void Start()
    {
        Application.targetFrameRate = 60;
        ballState = State.Start;        
        transform.position = _ballPath[0];
        ballTrace.enabled = false;        
    }

    private void Awake()
    {
        _ballPath = GetBallPath();
    }

    void Update()
    {
        if (ballState == State.Moving && ballSpeed > 0)
        {
            
            if (Mathf.Abs(Vector3.Distance(transform.position, _ballPath[_currentWayPoint])) < Distance)
            {
                if (_currentWayPoint >= _ballPath.Length - 1)
                    ballState = State.Finish;
                else
                    _currentWayPoint++;
            }

            transform.position = Vector3.MoveTowards(transform.position, _ballPath[_currentWayPoint], ballSpeed * Time.deltaTime);    
            
            if (Mathf.Abs(Vector3.Distance(ballTrace.GetPosition(ballTrace.positionCount - 1), transform.position)) > 0.5)
            {
                ballTrace.positionCount++;
                ballTrace.SetPosition(ballTrace.positionCount - 1, transform.position);
            }
        }
    }

    public void Launch()
    {
        if (_ballPath.Length > 0)
        {
            _currentWayPoint = 1;
            ballTrace.enabled = true;
            ballTrace.positionCount++;
            ballTrace.SetPosition(0, transform.position);
            ballState = State.Moving;
        }
    }

    public void Restart()
    {
        _currentWayPoint = 1;
        transform.position = _ballPath[0];
        ballTrace.enabled = false;
        ballTrace.positionCount = 0;
        ballState = State.Start;
    }

    public Vector3[] GetBallPath()
    {
        if (jsonFile != null)
        {
            JsonCoordinates jsonCoord = JsonUtility.FromJson<JsonCoordinates>(jsonFile.text);
            Vector3[] ballPath = new Vector3[jsonCoord.x.Length];
            for (int i = 0; i < jsonCoord.x.Length; i++)
            {
                ballPath[i].x = jsonCoord.x[i];
                ballPath[i].y = jsonCoord.y[i];
                ballPath[i].z = jsonCoord.z[i];
            }
            return ballPath;
        }
        return null;
    }
}