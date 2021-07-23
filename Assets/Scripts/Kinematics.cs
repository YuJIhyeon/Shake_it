using UnityEngine;

public class Kinematics : MonoBehaviour
{
    public Transform objBox;
    public Transform hole2DColider;
    public Transform ground2DCollider;
    public Transform GeneratedMeshCollider;

    public float initialScale = 1.6f;
    Mesh GeneratedMesh;

    private void FixedUpdate()
    {
        //if (transform.hasChanged == true)
        //{
        //    transform.hasChanged = false;
        //    hole2DColider.transform.position = new Vector2(transform.position.x, transform.position.z);
        //    ground2DCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
        //    GeneratedMeshCollider.transform.position = new Vector2(transform.position.x, transform.position.z);
        //    //MakeHole2D();
        //    //Make3DMeshCollider();
        //}
        hole2DColider.transform.position = new Vector3(objBox.position.x, objBox.position.y, objBox.position.z);
        ground2DCollider.transform.position = new Vector3(-objBox.position.x, -objBox.position.y, objBox.position.z);
        GeneratedMeshCollider.transform.position = new Vector3(-objBox.position.x, objBox.position.y, objBox.position.z);
        //Debug.Log(hole2DColider.transform.position);
        //Debug.Log(ground2DCollider.transform.position);
        //Debug.Log(GeneratedMeshCollider.transform.position);
    }

    //private void MakeHole2D()
    //{
    //    Vector2[] PointPositions = hole2DColider.GetPath(0);

    //    for (int i = 0; i < PointPositions.Length; i++)
    //    {
    //        PointPositions[i] = hole2DColider.transform.TransformPoint(PointPositions[i]);
    //    }

    //    ground2DCollider.pathCount = 2;
    //    ground2DCollider.SetPath(1, PointPositions);
    //}

    //private void Make3DMeshCollider()
    //{
    //    if (GeneratedMesh != null) Destroy(GeneratedMesh);
    //    GeneratedMesh = ground2DCollider.CreateMesh(true, true);
    //    GeneratedMeshCollider.sharedMesh = GeneratedMesh;
    //}
}
