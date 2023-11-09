using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    
    //movement
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded = false;
    private float speed = 5f;
    private bool isFacingRight = true;
    private float HorizontalMovement, VerticalMovement;

    public bool CanMove = true;


    // Start is called before the first frame update
    void Start()
    {
        try{GameManager.instance.Player = this.gameObject;}catch{}
    }

    // Update is called once per frame
    void Update()
    {
        if(CanMove){
            // initializes movement
            HorizontalMovement = Input.GetAxisRaw("Horizontal");
            VerticalMovement = Input.GetAxisRaw("Vertical");


            //check if player is grounded
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

            // flips the players direction
            if( (isFacingRight && HorizontalMovement < 0) || (!isFacingRight && HorizontalMovement > 0))
            {
                isFacingRight = !isFacingRight;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
                
            }

            //jumping
            if(Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(transform.up * 5f, ForceMode2D.Impulse);
            }




        }
    }

    void FixedUpdate()
    {
        
            //movement
            transform.Translate(HorizontalMovement * speed * Time.fixedDeltaTime, 0f, 0f);
    }
}
