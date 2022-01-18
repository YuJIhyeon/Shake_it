using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

//Hand >> wrist각 변화 (X축, yaw)
//Stage >> arm 각 변화 (Z축, Roll)
public class Agent_S : Agent
{

    public GameObject stage;
    public Rigidbody ball;
    public GameObject Hand;
    public Transform HoledPlane;
   

    float thetaX, thetaY, thetaZ;
    float ballX, ballY, ballZ;
    float alpha, beta, gamma;

    float deltaT = 0.6f;
    float temp;
    float scaler = 1f;

    bool flag = false;

    float[] arr = new float[2];
    float[] act = new float[3];
    int idx = 0;

    public override void Initialize()
    {
        myInit();
        arr[0] = -1f;
        arr[1] = 1f;

        act[0] = 1;
        act[1] = 2;
        act[2] = 3;
    }

    public override void OnEpisodeBegin()
    {
        myInit();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Ball
        //position(distance)
        //sensor.AddObservation(ball.transform.position - HoledPlane.transform.position); // Distance between hole and ball
        sensor.AddObservation(Vector3.Distance(HoledPlane.position, ball.position)); // Distance between hole and ball
        
        //velocity
        sensor.AddObservation(ball.velocity);
        sensor.AddObservation(ball.angularVelocity);

        //Stage and Endpoint
        //rotation
        sensor.AddObservation(Hand.transform.localRotation.x); //rotation of endpoint( yaw part )
        sensor.AddObservation(stage.transform.localRotation.z); //rotation of endpoint( roll part )
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        ColliderCheck Collision1 = GameObject.Find("side (2)").GetComponent<ColliderCheck>();
        ColliderCheck Collision2 = GameObject.Find("side (3)").GetComponent<ColliderCheck>();
        FileOut Trial = GameObject.Find("env").GetComponent<FileOut>(); //env <->curvedEnv*******************************************************************

        //ColliderCheck Collision3 = GameObject.Find("Holdeplane Colider").GetComponent<ColliderCheck>();

        if (Collision1.isCollision == true || Collision2.isCollision == true) // || Collision3.isCollision == true)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }

        var continuousActionsOut = actionBuffers.ContinuousActions;

        var actionZ = 2f * Mathf.Clamp(continuousActionsOut[0], -10f, 10f);
        var actionX = 2f * Mathf.Clamp(continuousActionsOut[1], -10f, 10f);

        //Debug.Log("Z:" + actionZ.ToString() + ", X:" + actionX.ToString());
        //Debug.Log("EndPoint:" + Hand.transform.localRotation.x.ToString());
        //Debug.Log("Manipulator:" + stage.transform.localRotation.z.ToString());

        ////=============================================================================================
        //if (actionX == 0f && Hand.transform.localRotation.x < 0f)
        //{
        //    actionX = actionX + 0.1f;
        //    //continuousActionsOut[1] = actionX;
        //}
        //else if (actionX == 0f && Hand.transform.localRotation.x > 0f)
        //{
        //    actionX = actionX - 0.1f;
        //    // continuousActionsOut[1] = actionX;
        //}
        //else if ((float)Mathf.Round(Hand.transform.localRotation.x) == 0f)
        //{
        //    idx = Random.Range(0, 2);
        //    actionX = 10f * arr[idx];
        //    //continuousActionsOut[1] = actionX;
        //}

        //if (actionZ == 0f && stage.transform.localRotation.z < 0f)
        //{
        //    actionZ = actionZ + 0.1f;
        //    //continuousActionsOut[0] = actionZ;
        //}
        //else if (actionZ == 0f && stage.transform.localRotation.z > 0f)
        //{
        //    actionZ = actionZ - 0.1f;
        //    //continuousActionsOut[0] = actionZ;
        //}
        //else if ((float)Mathf.Round(stage.transform.localRotation.z) == 0f)
        //{
        //    idx = Random.Range(0, 2);
        //    actionX = 10f * arr[idx];
        //    //continuousActionsOut[0] = actionZ;
        //}
        //=============================================================================================


        //if ((Hand.transform.localRotation.x < 90f && actionX > 0f) ||
        //    (Hand.transform.localRotation.x > -90f && actionX < 0f))
        //{
        //    //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
        //    thetaX = thetaX + actionX; //* deltaT;
        //    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
        //}
        //if ((stage.transform.localRotation.z < 90f && actionZ > 0f) ||
        //    (stage.transform.localRotation.z > -90f && actionZ < 0f))
        //{
        //    //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
        //    thetaZ = thetaZ + actionZ;// * deltaT;
        //    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
        //}

        if (actionX == 0f && actionZ == 0f)
        {
            idx = Random.Range(0, 3);
            if (act[idx] == 3)
            {
                idx = Random.Range(0, 2);
                actionX = 2f * arr[idx];
                idx = Random.Range(0, 2);
                actionZ = 2f * arr[idx];

            }
            else if (act[idx] == 2)
            {
                idx = Random.Range(0, 2);
                actionX = 3f * arr[idx];
            }
            else
            {
                idx = Random.Range(0, 2);
                actionZ = 3f * arr[idx];

            }
        }

        ////5,6,7 model
        ////stage -> manipulator, Hand -> EndPoint                        # 0.25 <-> 90
        //if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > 0f) && actionX < 0f) ||
        //    ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x <= 0f) && actionX > 0f))
        //{
        //    //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
        //    thetaX = thetaX + actionX * deltaT;
        //    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

        //}
        //else
        //{
        //    thetaX = thetaX - actionX * deltaT;  
        //    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
        //}
        //if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > 0f) && actionZ < 0f) ||
        //    ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z <= 0f) && actionZ > 0f))
        //{
        //    //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
        //    thetaZ = thetaZ + actionZ * deltaT;
        //    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
        //}
        //else
        //{  
        //    thetaZ = thetaZ - actionZ * deltaT; // 
        //    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
        //}

        // 과하게 꺾이는 것 방지
        if (Mathf.Abs(Hand.transform.localRotation.x) > 100f)
        {
            Trial.NewTrial = true;
            SetReward(-10f);
            EndEpisode();
        }

        //물리엔진 오류 방지 - 초기값 세팅 공 위치 튕기는 현상 방지
        if (Vector3.Distance(HoledPlane.position, ball.position) > 15f)
        {
            Trial.NewTrial = true;
            EndEpisode();
        }

        //물리엔진 오류 방지 - 메쉬 뚫는 rigid body
        if (flag == true)
        {
            flag = false;
            Collision1.isCollision = false;
            Collision2.isCollision = false;
            //Collision3.isCollision = false;
            Trial.NewTrial = true;
            SetReward(-10f);
            EndEpisode();
        }

        SetReward(-0.0001f); // 가만히 있지 마
        Debug.Log(((float)Mathf.Round(ball.angularVelocity.magnitude * 100f) / 100f).ToString());

        if ((float)Mathf.Round(ball.angularVelocity.magnitude) == 0f)
        {
            scaler = 3f;
        }
        else if((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f >= 1f && (float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f < 1.5f)
        {
            scaler = 2f;
        }
        else
        {
            scaler = 1f;
        }

       if (Vector3.Distance(HoledPlane.position, ball.position) >= 4f && Vector3.Distance(HoledPlane.position, ball.position) < 6f)
       {
            SetReward(0.2f);
            actionX = scaler * actionX;
            actionZ = scaler * actionZ;
            if (((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f == 0f) ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f >= 1f && (float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f < 1.5f)  ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f >= 3.0f))
            {
                SetReward(-0.1f);
                if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > -0.05f) && actionX < 0f) ||
                    ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x <= 0.05f) && actionX > 0f))
                {
                    
                    //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
                    thetaX = thetaX + actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                }
                else
                {
                    thetaX = thetaX - actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
                }
                if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > -0.05f) && actionZ < 0f) ||
                    ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z <= 0.05f) && actionZ > 0f))
                {
                    //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
                    thetaZ = thetaZ + actionZ * deltaT;
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }
                else
                {
                    thetaZ = thetaZ - actionZ * deltaT; // 
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }


                //thetaX = thetaX + actionX * deltaT;
                //Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                //thetaZ = thetaZ + actionZ * deltaT;
                //stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                //EndEpisode();

            }
            //else
            //{
            //    if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > 30f) && actionX < 0f) ||
            //        ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x < -30f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 5f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 30f && Hand.transform.localRotation.x > 10f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -30f && Hand.transform.localRotation.x < -10f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 10f && Hand.transform.localRotation.x > 5f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -10f && Hand.transform.localRotation.x < -5f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 3f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 5f && Hand.transform.localRotation.x > 0f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -5f && Hand.transform.localRotation.x < 0f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 2f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }
            //    else
            //    {
            //        thetaX = thetaX - 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }


            //    if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > 30f) && actionZ < 0f) ||
            //        ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z < -30f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 5f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 30f && stage.transform.localRotation.z > 10f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -30f && stage.transform.localRotation.z < -10f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 4f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 10f && stage.transform.localRotation.z > 5f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -10f && stage.transform.localRotation.z < -5f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 3f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 5f && stage.transform.localRotation.z > 0f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -5f && stage.transform.localRotation.z < 0f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 2f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else
            //    {
            //        thetaZ = thetaZ - 4f * actionZ * deltaT; // 
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }


            //}

        }
        else if (Vector3.Distance(HoledPlane.position, ball.position) >= 0.5f && Vector3.Distance(HoledPlane.position, ball.position) < 4f)
        {
            SetReward(0.3f);
            actionX = scaler * actionX;
            actionZ = scaler * actionZ;
            if (((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f == 0.0f) ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f > 0.6f && (float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f < 1f) ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f >= 3.0f))
            {
                SetReward(-0.1f);

                if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > -0.05f) && actionX < 0f) ||
                    ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x <= 0.05f) && actionX > 0f))
                {
                    //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
                    thetaX = thetaX + actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                }
                else
                {
                    thetaX = thetaX - actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
                }
                if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > -0.05f) && actionZ < 0f) ||
                    ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z <= 0.05f) && actionZ > 0f))
                {
                    //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
                    thetaZ = thetaZ + actionZ * deltaT;
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }
                else
                {
                    thetaZ = thetaZ - actionZ * deltaT; // 
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }

                //thetaX = thetaX + actionX * deltaT;
                //Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                //thetaZ = thetaZ + actionZ * deltaT;
                //stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);

                //EndEpisode();
            }
            //else
            //{
            //    if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > 30f) && actionX < 0f) ||
            //        ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x < -30f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 5f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 30f && Hand.transform.localRotation.x > 10f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -30f && Hand.transform.localRotation.x < -10f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 10f && Hand.transform.localRotation.x > 5f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -10f && Hand.transform.localRotation.x < -5f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 3f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 5f && Hand.transform.localRotation.x > 0f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -5f && Hand.transform.localRotation.x < 0f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 2f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }
            //    else
            //    {
            //        thetaX = thetaX - 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }


            //    if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > 30f) && actionZ < 0f) ||
            //        ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z < -30f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 5f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 30f && stage.transform.localRotation.z > 10f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -30f && stage.transform.localRotation.z < -10f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 4f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 10f && stage.transform.localRotation.z > 5f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -10f && stage.transform.localRotation.z < -5f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 3f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 5f && stage.transform.localRotation.z > 0f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -5f && stage.transform.localRotation.z < 0f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 2f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else
            //    {
            //        thetaZ = thetaZ - 4f * actionZ * deltaT; // 
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }

            //}

        }

        else if (Vector3.Distance(HoledPlane.position, ball.position) < 0.5f)
        {
            SetReward(1f);
            if ((float)Mathf.Round(ball.velocity.magnitude * 100f) / 100f == 0.0f)
            {
                SetReward(50f);
                Trial.NewTrial = true;
                EndEpisode();
            }
        }
        else
        {
            SetReward(-0.1f);
            actionX = scaler * actionX;
            actionZ = scaler * actionZ;

            if (((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f == 0.0f) ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f > 0.6f && (float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f < 1f) ||
                ((float)Mathf.Round(ball.angularVelocity.magnitude * 10f) / 10f >= 3.0f))
            {
                SetReward(-0.1f);

                if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > -0.05f) && actionX < 0f) ||
                    ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x <= 0.05f) && actionX > 0f))
                {
                    //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
                    thetaX = thetaX + actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                }
                else
                {
                    thetaX = thetaX - actionX * deltaT;
                    Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
                }
                if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > -0.05f) && actionZ < 0f) ||
                    ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z <= 0.05f) && actionZ > 0f))
                {
                    //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
                    thetaZ = thetaZ + actionZ * deltaT;
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }
                else
                {
                    thetaZ = thetaZ - actionZ * deltaT; // 
                    stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                }

                //thetaX = thetaX + actionX * deltaT;  
                //Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

                //thetaZ = thetaZ + actionZ * deltaT;
                //stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
                // EndEpisode();
            }
            //else
            //{
            //    if (((Hand.transform.localRotation.x < 90f && Hand.transform.localRotation.x > 30f) && actionX < 0f) ||
            //        ((Hand.transform.localRotation.x > -90f && Hand.transform.localRotation.x < -30f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 5f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 30f && Hand.transform.localRotation.x > 10f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -30f && Hand.transform.localRotation.x < -10f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 10f && Hand.transform.localRotation.x > 5f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -10f && Hand.transform.localRotation.x < -5f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 3f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

            //    }
            //    else if (((Hand.transform.localRotation.x <= 5f && Hand.transform.localRotation.x > 0f) && actionX < 0f) ||
            //            ((Hand.transform.localRotation.x >= -5f && Hand.transform.localRotation.x < 0f) && actionX > 0f))
            //    {
            //        //Hand.transform.Rotate(new Vector3(1, 0, 0), actionX*deltaT);
            //        thetaX = thetaX + 2f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }
            //    else
            //    {
            //        thetaX = thetaX - 4f * actionX * deltaT;
            //        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
            //    }


            //    if (((stage.transform.localRotation.z < 90f && stage.transform.localRotation.z > 30f) && actionZ < 0f) ||
            //        ((stage.transform.localRotation.z > -90f && stage.transform.localRotation.z < -30f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 5f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 30f && stage.transform.localRotation.z > 10f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -30f && stage.transform.localRotation.z < -10f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 4f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 10f && stage.transform.localRotation.z > 5f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -10f && stage.transform.localRotation.z < -5f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 3f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else if (((stage.transform.localRotation.z < 5f && stage.transform.localRotation.z > 0f) && actionZ < 0f) ||
            //            ((stage.transform.localRotation.z > -5f && stage.transform.localRotation.z < 0f) && actionZ > 0f))
            //    {
            //        //stage.transform.Rotate(new Vector3(0, 0, 1), actionZ*deltaT);
            //        thetaZ = thetaZ + 2f * actionZ * deltaT;
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //    else
            //    {
            //        thetaZ = thetaZ - 4f * actionZ * deltaT; // 
            //        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
            //    }
            //}

        }

        

        //// ball이 빠져버리는 경우 방지
        //if (ball.transform.position.y < -50)
        //{
        //    EndEpisode();
        //}
        //Debug.Log("D:" + (Vector3.Distance(HoledPlane.position, ball.position)).ToString()+ " V:" + ((float)Mathf.Round(ball.velocity.magnitude)).ToString()+ " A:" + ((float)Mathf.Round(ball.angularVelocity.magnitude)).ToString());

        //가만히 있지 마!
        //SetReward(-0.001f); 

        ////5,6,7,8
        //if (Vector3.Distance(HoledPlane.position, ball.position) >= 2f) //2
        //{
        //    if (Vector3.Distance(HoledPlane.position, ball.position) < 4f)  //4
        //    {
        //        //if (ball.velocity.magnitude < 3f && ball.velocity.magnitude >1f)
        //        //{
        //        //    SetReward(0.1f);
        //        //}

        //        if ((float)Mathf.Round(ball.angularVelocity.magnitude * 100f) / 100f == 0.0f)
        //        {
        //            SetReward(-0.3f);
        //            EndEpisode();
        //        }
        //    }
        //    else
        //    {
        //        //if (ball.velocity.magnitude >= 3f)
        //        //{
        //        //    SetReward(0.1f);
        //        //}            

        //        if ((float)Mathf.Round((float)Mathf.Round(ball.velocity.magnitude) * 10f) / 10f == 0.0f) // 각속도<-> 속도로 바꿨음! 소수 둘째자리/ 소수 첫째자리
        //        {
        //            //Debug.Log("Goal");
        //            SetReward(-0.5f);
        //            EndEpisode();
        //        }
        //    }



        //}
        //else if(Vector3.Distance(HoledPlane.position, ball.position) >= 0.5f && Vector3.Distance(HoledPlane.position, ball.position) < 2f)
        //{

        //    //SetReward(0.192f);
        //    //if (ball.velocity.magnitude <= 2f)
        //    //{
        //    //    SetReward(0.05f);
        //    //}
        //    //else if (ball.velocity.magnitude >= 3f && ball.velocity.magnitude < 4f)
        //    //{
        //    //    SetReward(0.03f);
        //    //}

        //    //else
        //    //{
        //    //    if ((float)Mathf.Round(ball.angularVelocity.magnitude * 100f) / 100f == 0.0f)
        //    //    {
        //    //        //Debug.Log("Goal");
        //    //        SetReward(-0.2f);
        //    //        EndEpisode();
        //    //    }
        //    //}
        //    if ((float)Mathf.Round(ball.angularVelocity.magnitude * 100f) / 100f == 0.0f)
        //    {
        //        //Debug.Log("Goal");
        //        SetReward(-0.2f);
        //        EndEpisode();
        //    }

        //}
        //else if (Vector3.Distance(HoledPlane.position, ball.position) < 0.5f)
        //{
        //    //SetReward(0.5f);

        //    //if (ball.velocity.magnitude <= 1f)
        //    //{
        //    //    SetReward(0.35f);
        //    //}

        //    if ((float)Mathf.Round(ball.angularVelocity.magnitude * 100f) / 100f == 0.0f)
        //    {
        //        //Debug.Log("Goal");
        //        SetReward(50.0f);
        //        EndEpisode();
        //    }
        //}

    }

    private void myInit()
    {
        ball.transform.position = Vector3.zero;

        thetaX = Random.Range(-90.00f, 90.00f);
        thetaY = 0;
        thetaZ = 0;
        temp = thetaX;

        Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);

        ballX = Random.Range(-8.7f, 8.7f); // the Random position of Z from the ball
        ballY = 1.25f + 0.9f;
        ballZ = Random.Range(-8.7f, 8.7f) - 30.5f;

        alpha = thetaZ * Mathf.PI / 180f;
        beta = thetaY * Mathf.PI / 180f;
        gamma = thetaX * Mathf.PI / 180f;

        ball.transform.position = new Vector3(ballX, ballY * Mathf.Cos(gamma) - ballZ * Mathf.Sin(gamma) - 0.9f * (1f + Mathf.Sin(gamma)), ballY * Mathf.Sin(gamma) + ballZ * Mathf.Cos(gamma) + 30.5f); //yaw

        thetaX = 0;
        thetaY = 0;
        thetaZ = Random.Range(-90.00f, 90.00f);

        stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);

        ballX = ball.transform.position.x;
        ballY = ball.transform.position.y;
        ballZ = ball.transform.position.z;

        alpha = thetaZ * Mathf.PI / 180f;
        beta = thetaY * Mathf.PI / 180f;
        gamma = thetaX * Mathf.PI / 180f;

        ball.transform.position = new Vector3(ballX * Mathf.Cos(alpha) - ballY * Mathf.Sin(alpha), ballX * Mathf.Sin(alpha) + ballY * Mathf.Cos(alpha), ballZ); //roll
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;

        thetaX = temp;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal")*deltaT;
        continuousActionsOut[1] = Input.GetAxis("Vertical")*deltaT;
    }
    
}
