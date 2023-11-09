using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Transform gravityPointer;
    [SerializeField] Transform bottom;
    public SpriteRenderer outline;
    public ParticleSystem explosion;
    public GameObject trail;

    public Rigidbody2D rb;
    [SerializeField] float fieldStrength;
    Vector2 StartTouchPosition;
    bool swiping = false;
    [SerializeField] float movementForce;
    [SerializeField] float minVelocity; 
    [SerializeField] float jumpForce;
    [SerializeField] float flipJumpFactor;
    [SerializeField] float jumpTorque;
    [SerializeField] float flipTorque;
    // (magnitude of) minimum velocity horizontally to allow a jump without an impulse horizontally.
    [SerializeField] float groundDistance; 
    [SerializeField] LayerMask whatIsGround; 
    [SerializeField] bool isGrounded = false;
    [SerializeField] float coefficientFriction;
    Vector2 basisUp; // the direction which faces up in a gravity field, opposite of the direction of gravity
    Vector2 basisRight; // slightly confusing, the right in relative to the SCREEN
    public Vector2 gravimode1; // initial gravity in level
    public Vector2 gravimode2;
    Vector2 velBR;
    Vector2 velBU;
    float startTapTime;
    float endTapTime;
    float oldDeltaTapTime;
    public float doubleTapWindow;
    bool doubleTap = false;
    public float rapidClickWindow;
    [HideInInspector] public bool finishLevelAnim = false;
    [SerializeField] GameObject finishPoint;
    [SerializeField] float finishRadius;
    [SerializeField] float finishVelocity;
    bool touching = false;
    bool beginTouching = false;
    bool midTouching = false;
    bool endTouching = false;
    bool jumping = false;
    float touchDirection;




    public void changeGravityDirection(Vector2 direction)
    {
        // normalises vector
        direction = direction.normalized;
        //change direction of gravity
        gravityPointer.localPosition = direction;

        //defiine basis vectors
        basisUp = -direction;

        Vector2 perpendicularBasis = Vector2.Perpendicular(basisUp);
        // case1: perp is facing right
        if(perpendicularBasis.x > 0)
            basisRight = perpendicularBasis;
        // case2: perp is facing left
        else if(perpendicularBasis.x < 0)
            basisRight = -perpendicularBasis;
        // case3: perp is up (grav right)
        else if(perpendicularBasis.y > 0)
            basisRight = -perpendicularBasis;
        // case4: perp is down (grav left)
        else 
            basisRight = -perpendicularBasis;
    }


    void FixedUpdate()
    {
        //checks if the player is in radius of finish point
        Vector2  finishPlayerVect = -transform.position + finishPoint.transform.position;

        if(finishLevelAnim) 
        {
            rb.velocity = Vector2.Perpendicular(finishPlayerVect) * finishVelocity;
            rb.AddForce(finishPlayerVect.normalized * rb.velocity.sqrMagnitude / finishPlayerVect.magnitude);
        }

        if(!finishLevelAnim && finishPlayerVect.magnitude < finishRadius)
        {
            finishLevelAnim = true;
            rb.velocity = Vector2.Perpendicular(finishPlayerVect.normalized);
        } 

        //gravity - checks for ground and applies accerates player in direction of bottom
        Vector2 gravDirection = gravityPointer.localPosition.normalized;
        if(!finishLevelAnim){
            rb.AddForce(gravDirection * fieldStrength * Time.deltaTime * 260);
        }

        // makes sure bottom is at the bottom of the player
        float angle = -(transform.eulerAngles.z * Mathf.PI/180);

        bottom.localPosition = new Vector3(gravDirection.x*Mathf.Cos(angle)/2 - gravDirection.y*Mathf.Sin(angle)/2, gravDirection.x*Mathf.Sin(angle)/2 + gravDirection.y*Mathf.Cos(angle)/2, bottom.position.z);
         

        RaycastHit2D hit = Physics2D.Raycast(bottom.position, gravDirection, groundDistance, whatIsGround);
        isGrounded = hit;

        if(isGrounded) {
            rb.drag = coefficientFriction;
        } else{
            rb.drag = 0;
            doubleTap = false;
        }

        if(touching && beginTouching)
        {
            rb.velocity = new Vector2(0f,0f);
            rb.AddForce(touchDirection * basisRight * movementForce * Time.deltaTime * 260);
        }

        if(touching && midTouching && jumping)
        {
            jump(1,touchDirection);
            jumping = false;
        }
        if(touching && midTouching && !swiping && isGrounded)
        {
            rb.AddForce(touchDirection * basisRight * movementForce * Time.deltaTime * 260);
        }
        if(touching && endTouching)
        {
            swiping = false;
            StartTouchPosition = new Vector2(0,0);
            endTouching = false;
        }

    }

    void Update()
    {
        ///////////////// TO FIXED UPDATE
        //checks if the player is in radius of finish point
        /*Vector2  finishPlayerVect = -transform.position + finishPoint.transform.position;

        if(finishLevelAnim) ///////////////// TO FIXED UPDATE
        {
            rb.velocity = Vector2.Perpendicular(finishPlayerVect) * finishVelocity;
            rb.AddForce(finishPlayerVect.normalized * rb.velocity.sqrMagnitude / finishPlayerVect.magnitude);
        }

        if(!finishLevelAnim && finishPlayerVect.magnitude < finishRadius)
        {
            finishLevelAnim = true;
            rb.velocity = Vector2.Perpendicular(finishPlayerVect.normalized);
        } 



        //gravity - checks for ground and applies accerates player in direction of bottom
        Vector2 gravDirection = gravityPointer.localPosition.normalized;
        if(!finishLevelAnim){
            rb.AddForce(gravDirection * fieldStrength * Time.deltaTime * 260);
        } 

        // makes sure bottom is at the bottom of the player
        float angle = -(transform.eulerAngles.z * Mathf.PI/180);

        bottom.localPosition = new Vector3(gravDirection.x*Mathf.Cos(angle)/2 - gravDirection.y*Mathf.Sin(angle)/2, gravDirection.x*Mathf.Sin(angle)/2 + gravDirection.y*Mathf.Cos(angle)/2, bottom.position.z);
         

        RaycastHit2D hit = Physics2D.Raycast(bottom.position, gravDirection, groundDistance, whatIsGround);
        isGrounded = hit;

        if(isGrounded) {
            rb.drag = coefficientFriction;
        } else{
            rb.drag = 0;
            doubleTap = false;
        } */

        
        // player movement


        if (Input.touchCount > 0 && !finishLevelAnim)
        {
            touching = true;
            Touch touch = Input.touches[0];

            // player velocity in basis vectors
            velBR = basisRight * Vector2.Dot(basisRight, rb.velocity);
            velBU = basisUp * Vector2.Dot(basisUp, rb.velocity);
            

            touchDirection = 0; // whether the touch was one the first third or the third third of the screen

            if(touch.position.x < 1 * Screen.width / 3)
            {
                touchDirection =-1;
            }
            if(touch.position.x > 2 * Screen.width / 3)
            {
                touchDirection =1;
            }
            

            if(touchDirection != 0) {    

                if(touch.phase == TouchPhase.Began && isGrounded)
                {
                    beginTouching = true;
                    midTouching = false;
                    endTouching = false;

                    /*rb.velocity = new Vector2(0f,0f);
                    rb.AddForce(touchDirection * basisRight * movementForce * Time.deltaTime * 260);*/
                }
                    
                if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    beginTouching = false;
                    midTouching = true;
                    endTouching = false;
                    // checks if touch was a swipe
                    if(touch.position.y - StartTouchPosition.y > 50 && Mathf.Abs(touch.position.x - StartTouchPosition.x) < Screen.width/4  && swiping == false && isGrounded && StartTouchPosition != new Vector2(0,0))
                    {
                        swiping = true;
                        jumping = true;
                        
                        //if(Vector2.Dot(basisUp, Vector2.up) >= 0)
                        //    rb.AddTorque(-jumpTorque * touchDirection);
                        //else
                        //    rb.AddTorque(jumpTorque * touchDirection);
                        
                        /*jump(1,touchDirection);*/

                    } else if(swiping == false && isGrounded)
                    {

                        /*rb.AddForce(touchDirection * basisRight * movementForce * Time.deltaTime * 260);*/
                    }

                }

                if(touch.phase == TouchPhase.Ended) {
                    beginTouching = false;
                    midTouching = false;
                    endTouching = true;
                    /*swiping = false;
                    StartTouchPosition = new Vector2(0,0);*/

                }

            }

            


            if(touch.phase == TouchPhase.Began)
            {
                //store start pos of touch to check if it was a touch or swipe
                StartTouchPosition = touch.position;
                oldDeltaTapTime = startTapTime - endTapTime;
                endTapTime = Time.time;

            }
            if(touch.phase == TouchPhase.Ended)
            {
                startTapTime = Time.time;
            }


            
            //change gravity by swiping

            if((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && ( Mathf.Abs(touch.position.x - StartTouchPosition.x) > 70 && StartTouchPosition != new Vector2(0,0) && (touch.position.x > 2 * Screen.width / 3 || touch.position.x < 1 * Screen.width / 3  ) ) && swiping == false && isGrounded)
            {
                flip(StartTouchPosition.x-Screen.width/2);
                swiping = true;

            }
            if(touch.phase == TouchPhase.Ended) 
            {
                swiping = false;
                StartTouchPosition = new Vector2(0,0);
            }
            



            //change gravity by double tap
            if( endTapTime - startTapTime < doubleTapWindow && endTapTime > startTapTime &&  oldDeltaTapTime < rapidClickWindow && endTapTime != startTapTime && doubleTap == false && isGrounded && !(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                if(new Vector2(gravityPointer.localPosition.normalized.x,gravityPointer.localPosition.normalized.y) == gravimode1.normalized)
                {
                    changeGravityDirection(gravimode2);
                } else {
                    
                    changeGravityDirection(gravimode1); 
                }
            }

            
            



        }


        



    }

    public void flip(float swipeVal) // swipe val is touch.pos.x - half the screen width
    {
        float direction = Mathf.Abs(swipeVal)/swipeVal;

        // checks if theres not much horizontal movement to the right, and applies a force if not.
        if(velBR.magnitude < minVelocity)
        {
            rb.velocity = minVelocity * basisRight * direction + velBU;
        }


        if(new Vector2(gravityPointer.localPosition.normalized.x,gravityPointer.localPosition.normalized.y) == gravimode1.normalized)
        {
            
            rb.velocity = flipJumpFactor * minVelocity * basisRight * direction + velBU;
            rb.AddTorque(flipTorque);

            changeGravityDirection(gravimode2);
        }
        else {

            rb.velocity = flipJumpFactor * minVelocity * basisRight * direction + velBU;
            rb.AddTorque(-flipTorque);

            changeGravityDirection(gravimode1);
        }

        
    }

    void jump(float jumpFactor = 1, float direction = 0)
    {
        
        // checks if theres not much horizontal movement, and applies a force if not.
    
        rb.velocity = minVelocity * basisRight * direction + velBU;

        print("hi");
        rb.AddForce(basisUp * jumpForce * jumpFactor, ForceMode2D.Impulse);

    }


    void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "enemy")
        {
            StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        outline.enabled = false;
        explosion.Play();
        rb.simulated = false;
        trail.SetActive(false);

        yield return new WaitForSeconds(explosion.main.duration);

        transform.position = new Vector3(0,0,0);
        this.GetComponent<SpriteRenderer>().enabled = true;
        outline.enabled = true;
        rb.simulated = true;
        trail.SetActive(false);
        rb.velocity = new Vector2(0,0);
        changeGravityDirection(gravimode1);
        trail.SetActive(true);

    }

}
