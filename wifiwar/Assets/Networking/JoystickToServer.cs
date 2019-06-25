using System;
using System.Collections;
using System.Collections.Generic;
using ClientMessages;
using Unity.Mathematics;
using UnityEngine;

public class JoystickToServer : MonoBehaviour
{

    public float updateRateMs = 5.0f;
    private float lastUpdateMs = 0.0f;
    private Vector2 accumulatedMove;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = UnityEngine.Time.deltaTime;
        float currentTimeMs = UnityEngine.Time.time;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        accumulatedMove.x += horizontal;
        accumulatedMove.y += vertical;

        accumulatedMove.x = (float)math.clamp(accumulatedMove.x, -1.0, 1.0);
        accumulatedMove.y = (float)math.clamp(accumulatedMove.y, -1.0, 1.0);

        //Debug.Log("cur:"+currentTimeMs+" vs "+(lastUpdateMs+updateRateMs)+" both:"+lastUpdateMs+" + "+updateRateMs);
        if (currentTimeMs > (lastUpdateMs + updateRateMs))
        {
            Debug.Log("Time:"+currentTimeMs+" Accumulated:"+accumulatedMove);
            lastUpdateMs = currentTimeMs;
            accumulatedMove.x = 0;
            accumulatedMove.y = 0;
            
            ClientMessageMoveJoy movejoy = new ClientMessageMoveJoy();
            ServerConnection.Send(movejoy);
        }
    }
}
