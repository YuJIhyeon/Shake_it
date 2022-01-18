using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using UnityEditor;
public class FileOut : MonoBehaviour
{
    string filePath;
    float period = 0;
    float period_time = 0.02f;

    float dist = 0f;
    float sec = 0f;

    FileStream fs = null;
    StreamWriter writer = null;

    public GameObject beam;
    public Rigidbody ball;

    public bool NewTrial = true;


    void Start()
    {
        period = 0;
        
    }

    
    void Update()
    {

        if (NewTrial)
        {
            filePath = "./SimData/" + DateTime.Now.ToString("MMdd_HHmm_ss") + "_Flat.txt"; //Flat or Curved
            fs = new FileStream(filePath, FileMode.Create);
            writer = new StreamWriter(fs);
            sec = 0f;
        }
        NewTrial = false;

        period += Time.deltaTime;

        if (period >= period_time)
        {
            sec += period_time;
            period = 0f;
            dist = Vector3.Distance(beam.transform.position, ball.position);
            sec = Mathf.Round(sec * 100) / 100f;
            writer.Write(sec + "\t" + dist.ToString() + "\n");
        }
        
    }
}
