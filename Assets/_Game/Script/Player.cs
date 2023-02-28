using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;

    private float horizontal;

    private string currentAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void fixedUpdate()
    {
        isGrounded = checkGrounded();

        //-1 -> 0 -> 1
        horizontal = Input.GetAxisRaw("Horizontal");
        //vertical = Input.GetAxisRaw("Vertical");

        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }
            //jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }

            //run
            if (Mathf.Abs(horizontal) >0.1f)
            {
                changeAnimator("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                Attack();
            }

            //throw
            if (Input.GetKeyDown(KeyCode.V) && isGrounded)
            {
                Throw();
            }
        }

        //check falling
        if (isGrounded && rb.velocity.y < 0)
        {
            changeAnimator("fall");
            isJumping = false;
        }

        //Moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            changeAnimator("run");
            rb.velocity = new Vector2(horizontal * Time.fixedDeltaTime * speed, rb.velocity.y);
            //horizontal > 0 -> tra ve 0, neu horizontal <= 0 -> tra ve 180
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
            //transform.localScale = new Vector3(horizontal, 1, 1);
        }
        else if (isGrounded)
        {
            changeAnimator("idle");
            rb.velocity = Vector2.zero;
        }
    }

    private bool checkGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

        // return hit.collider != null
    }

    private void Attack ()
    {
        changeAnimator("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }
    
    private void Throw ()
    {
        changeAnimator("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void ResetAttack()
    {
        changeAnimator("idle");
        isAttack = false;
    }
    
    private void Jump()
    {
        isJumping = true;
        changeAnimator("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }
    private void changeAnimator (string animatorName) 
    { 
        if(currentAnimator != animatorName)
        {
            animator.ResetTrigger(animatorName);
            currentAnimator = animatorName;
            animator.SetTrigger(currentAnimator);
        }
    }
}
