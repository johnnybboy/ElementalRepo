using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeManager : MonoBehaviour
{
    public float startDelay = 0f;
    public float stopTime = 2f;
    public float smoothing = 1f;

    private Transform[] points;
    private int pointIndex = 0;
    private Transform blade;
    private bool isBusy = false;
    private bool firstTrigger = true;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)   //check to make sure there are points to work with
        {
            points = new Transform[transform.childCount - 1];   //-1 to ignore the blade child[0]
            blade = transform.GetChild(0);

            for (int i = 1; i < transform.childCount; i++)  //start at 1 to skip the blade child[0]
            {
                points[i - 1] = transform.GetChild(i).transform;
            }
            //start coroutine
            StartCoroutine(MoveSawblade());
        }
    }

    private void Update()
    {
        //start coroutine if it isn't already started
        if (!isBusy)
        {
            StartCoroutine(MoveSawblade());
        }
    }

    IEnumerator MoveSawblade()
    {
        isBusy = true;
        if (firstTrigger)   //delays the start of the loop for staggering traps
        {
            yield return new WaitForSeconds(startDelay);
            firstTrigger = false;
        }

        //move toward point at pointIndex
        while (Vector2.Distance(blade.position, points[pointIndex].position) > 0.05f)
        {
            blade.position = Vector2.MoveTowards(blade.position, points[pointIndex].position, smoothing * Time.deltaTime);

            yield return null;
        }

        //after arriving get next point
        pointIndex++;
        if (pointIndex >= points.Length)    //return to first point if at the end
            pointIndex = 0;

        yield return new WaitForSeconds(stopTime);

        //end
        isBusy = false;
    }
}
