using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoment : MonoBehaviour
{
    public float speedMovement = 5f;
    public Rigidbody2D rb;
    private Vector2 _movement;
    public Animator animator;

    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + _movement * speedMovement * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal"); 
        if (_movement.x < 0) 
        { 
            animator.SetBool("moving", true);
            Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                
        }
        else if (_movement.x > 0) 
        { 
            animator.SetBool("moving", true);
            Player.transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
        }
        else
        {
            animator.SetBool("moving", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("shooting", true);
        }
        else
        {
            animator.SetBool("shooting", false);
        }
        
    }
}
