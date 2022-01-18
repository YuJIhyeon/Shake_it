using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class test : MonoBehaviour
{
    public GameObject stage;
    public Rigidbody ball;
    public GameObject Hand;
    public Transform HoledPlane;

    float thetaX,thetaY,thetaZ;
    float ballX,ballY,ballZ;
    float alpha, beta, gamma;

    float deltaT = 0.2f;
    float temp;

    bool flag=false;
    


    void Start()
    {

        ColliderCheck Collision1 = GameObject.Find("side (2)").GetComponent<ColliderCheck>();
        ColliderCheck Collision2 = GameObject.Find("side (3)").GetComponent<ColliderCheck>();

        if (Collision1 ==true || Collision2 == true)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }

        ball.transform.position = Vector3.zero;
        
        thetaX = Random.Range(-90.00f, 90.00f);
        temp = thetaX;
        thetaY = 0;
        thetaZ = 0;

        Hand.transform.rotation = Quaternion.Euler(thetaX, 0f, 0f);
        
        ballX = Random.Range(-8.7f, 8.7f); // the Random position of Z from the ball
        ballY = 1.25f+0.9f;
        ballZ = Random.Range(-8.7f, 8.7f) - 30.5f;

        alpha = thetaZ * Mathf.PI / 180f;
        beta = thetaY * Mathf.PI / 180f;
        gamma = thetaX * Mathf.PI / 180f;

        ball.transform.position = new Vector3(ballX, ballY * Mathf.Cos(gamma) - ballZ * Mathf.Sin(gamma) -0.9f*(1f+Mathf.Sin(gamma)), ballY * Mathf.Sin(gamma) + ballZ * Mathf.Cos(gamma) + 30.5f); //yaw

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

    void Update()
    {
        ColliderCheck Collision1 = GameObject.Find("side (2)").GetComponent<ColliderCheck>();
        ColliderCheck Collision2 = GameObject.Find("side (3)").GetComponent<ColliderCheck>();
        
        if (Collision1.isCollision == true || Collision2.isCollision == true)
        {
            flag = true;
        }
        else
        {
            flag = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //thetaZ = stage.transform.localRotation.z;
            thetaZ = thetaZ + 0.5f*deltaT;
            stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //thetaZ = stage.transform.localRotation.z;
            thetaZ = thetaZ - 0.5f * deltaT;
            stage.transform.localRotation = Quaternion.Euler(0f, 0f, thetaZ);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //thetaX = Hand.transform.localRotation.x;
            thetaX = thetaX + 0.05f * deltaT;
            Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //thetaX = Hand.transform.localRotation.x;
            thetaX = thetaX - 0.05f * deltaT;
            Hand.transform.localRotation = Quaternion.Euler(thetaX, 0f, 0f);
        }

        if (flag == true)
        {
            flag = false;
            Collision1.isCollision = false;
            Collision2.isCollision = false;
            myInit();
        }

        if (ball.transform.position.y < -50)
        {
            myInit();
        }
        //Debug.Log("V" + ball.velocity.magnitude.ToString() + ", A:"+ball.angularVelocity.magnitude.ToString());
        Debug.Log(Vector3.Distance(ball.transform.position, HoledPlane.transform.position));
        //if ((float)Mathf.Round(ball.angularVelocity.z * 100f) / 100f == 0.0f)
        //{
        //    Debug.Log(Vector3.Distance(ball.transform.position, HoledPlane.transform.position));
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
}
