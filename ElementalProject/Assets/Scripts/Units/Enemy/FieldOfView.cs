using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    public float radius;
    public float angle;

    /*public double getRadius(double radius)
    {
        return radius
    }*/
    public Vector2 getAngle(float angleDegree)
    {
        return new Vector2(Mathf.Sin(angleDegree * Mathf.Deg2Rad), Mathf.Cos(angleDegree * Mathf.Deg2Rad));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
