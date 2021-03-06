﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KartGame.KartSystems;
using KartGame.Track;
using UnityEngine;
using MLAgents;
using MLAgents.Sensor;
using UnityEngine.SocialPlatforms.Impl;

public class kartAgent : Agent, IInput
{
    Rigidbody rBody;
    KartMovement kart;
    Vector3 startingPosition;
    Quaternion startingRotation;
    private bool first;

    public TrackManager trackManager { get; private set; }

    private IRacer racer;
    public const float Epsilon = 0.1f;
    private int hitWallCounter = 0;

    public float Acceleration => m_Acceleration;
    public float Steering => m_Steering;
    public bool BoostPressed => m_BoostPressed;
    public bool FirePressed => m_FirePressed;
    public bool HopPressed => m_HopPressed;
    public bool HopHeld => m_HopHeld;

    float m_Acceleration;
    float m_Steering;
    bool m_HopPressed;
    bool m_HopHeld;
    bool m_BoostPressed;
    bool m_FirePressed;
    bool m_FixedUpdateHappened;


    void Start()
    {
        kart.OnKartCollision.AddListener(HitWall2);
        first = true;
    }

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        trackManager = FindObjectOfType<TrackManager>();
        racer = GetComponent<IRacer>();
        kart = GetComponent<KartMovement>();
        startingPosition = this.transform.position;
        startingRotation = this.transform.rotation;
    }

    public override float[] Heuristic()
    {
        float[] actions = new float[3];
        if (Input.GetKey(KeyCode.UpArrow))
            actions[0] = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            actions[0] = -1f;
        else
            actions[0] = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            actions[1] = -1f;
        else if (!Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
            actions[1] = 1f;
        else
            actions[1] = 0f;

        actions[2] = Input.GetKey(KeyCode.Space) ? 1:0;
        return actions;
    }

    public override void AgentAction(float[] vectorAction)
    {
        base.AgentAction(vectorAction);
        //vectorAction[0] = Acceleration
        //1 similar to up arrow
        //-1 similar to down arrow
        //0 similar to not pressing

        if (vectorAction[0] > 0.1)
        {
            m_Acceleration = 1f;
        }
        else
            m_Acceleration = 0f;

        //vectorAction[1] = Steering
        //1 similar to left arrow
        //-1 similar to right arrow
        //0 similar to not pressing
        if (vectorAction.Length > 1)
        {
            m_Steering = vectorAction[1];
        }
        
        AddReward(kart.LocalSpeed * 0.001f);

        if (vectorAction.Length > 2)
        {
            m_HopPressed = vectorAction[2] > 0.75;
        }

    }

    
    public override void AgentReset()
    {
        hitWallCounter = 0;
        if (!first)
        {
            base.AgentReset();
            kart.transform.position = startingPosition;
            kart.transform.rotation = startingRotation;
            kart.ForceMove(Vector3.zero, Quaternion.identity);
            trackManager.RestrartRace(racer);
        }
        
        first = false;

    }

    public override void CollectObservations()
    {
        AddVectorObs(kart.LocalSpeed);
        //Agent positions
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
        AddVectorObs(rBody.velocity.y);

        trackManager.m_RacerNextCheckpoints.TryGetValue(racer, out var checkpoint);
        float distance = 30;
        if (checkpoint != null)
        {
            distance = Vector3.Distance(transform.position, checkpoint.transform.position);
        }
        AddVectorObs(distance);
    }

    public void FixedUpdate()
    {
        m_FixedUpdateHappened = true;
    }

    public void achivedCheckPoint(IRacer racer, Checkpoint checkpoint)
    {
        Debug.Log("AchievementDescription checkpoint point" + kart.name + checkpoint.name + trackManager.name);
        AddReward(1);
    }

    
    public void HitWall()
    {
        hitWallCounter++;
        if (hitWallCounter >= 10)
        {
            Done();
            AgentReset();
        }
    }

    public void HitWall2()
    {
        //TODO this is an event raised from KartMovement if want to act differently based on type of collision use
        //kart.m_RaycastHitBuffer[i].transform.name;
    }



}
