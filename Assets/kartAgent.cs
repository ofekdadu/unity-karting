using System.Collections;
using System.Collections.Generic;
using KartGame.KartSystems;
using KartGame.Track;
using UnityEngine;
using MLAgents;
using MLAgents.Sensor;

public class kartAgent : Agent
{
    [Tooltip("A reference to the KartMovement script of the kart.")]
    [RequireInterface(typeof(IMovable))]
    public Object movable;
    [Tooltip("A reference to the TrackManager script for this track.")]
    public TrackManager trackManager;
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.position = new Vector3(Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4);


        //trackManager.ReplaceMovable(movable);
    }

    public override void CollectObservations()
    {
        //Agent positions
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
        AddVectorObs(rBody.velocity.y);

        // ReyPerception (sight) 
        float rayDistance = 50f;
        float[] rayAngles = {0f, 30f, 45f, 60f, 90f, 120f, 135f, 150f, 180f};
        string[] detectableObject = {"wall", "speedPads", "checkPoints", "startFinishLine"};
        //AddVectorObs(RayPerceptionSensorComponent3D.);
    }

}
