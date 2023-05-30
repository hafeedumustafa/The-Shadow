using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    
    //movement
    public Rigidbody2D rb;
    public float speed, initialspeed;
    public float HorizontalMovement, VerticalMovement;
    private Vector3 oldPosition;
    public Vector2 Velocity;
    public bool CanMove = true;


    // Start is called before the first frame update
    void Start()
    {
        initialspeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(CanMove) {

            HorizontalMovement = Input.GetAxisRaw("Horizontal");
            VerticalMovement = Input.GetAxisRaw("Vertical");
            if(HorizontalMovement < 0) {
                transform.localScale = new Vector3(-0.12f, 0.12f, 1);
            } else if(HorizontalMovement > 0) {
                transform.localScale = new Vector3(0.12f, 0.12f, 1);
            }


        } else {HorizontalMovement = 0f; VerticalMovement = 0f;}
        if (Input.GetKeyDown(KeyCode.LeftShift)) {speed = initialspeed + 2;}
        if (Input.GetKeyUp(KeyCode.LeftShift)) {speed = initialspeed;}
    }

    void FixedUpdate()
    {

        Velocity = new Vector2(oldPosition.x - transform.position.x, oldPosition.y - transform.position.y);
        oldPosition = transform.position;
        transform.Translate( HorizontalMovement * speed * Time.fixedDeltaTime, 0, 0);
        
    }
}
