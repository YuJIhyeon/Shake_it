using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class BallTracer : MonoBehaviour
{
    WebCamTexture _webCamTexture;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCamTexture = new WebCamTexture(devices[2].name);
        _webCamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTexture = _webCamTexture;
    }
}
