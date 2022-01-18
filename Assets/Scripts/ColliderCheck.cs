using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{    
    public bool isCollision = false;

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌감지"  );
        isCollision = true;
    }
   
}
