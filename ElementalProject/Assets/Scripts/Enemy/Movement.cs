using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rd;
    public float moveSpeed = 6;
    
    Vector2 vel;

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        vel = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Virtical")).normalized * moveSpeed;
    }

    private void FixedUpdate()
    {
        rd.MovePosition(rd.position + vel * Time.fixedDeltaTime);
    }
}
