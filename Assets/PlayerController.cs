using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    Rigidbody2D rb;
    public float speedMul;
    float step;
    Vector3 dir;
    Vector3 spriteDir;
    public Transform groundPoint;
    public GameObject sprite;
    public bool grounded;
    public float jumpTimer;
    public float jumpForce;
    public float takeoffForce;
    Vector3 targetDir;
    float turnSpeed;
    public float downForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // CONSTANTS
        if (grounded)
        {
            step = speedMul * (-Mathf.Abs(rb.velocity.x * .15f) + 2f) * Time.deltaTime;
        }
        else
        {
            step = speedMul / 1.1f * (-Mathf.Abs(rb.velocity.x * .15f) + 2f) * Time.deltaTime;
        }

        if (!grounded && rb.gravityScale < 2.5f)
            rb.gravityScale += Time.deltaTime * 1.2f;


        RaycastHit2D hit1 = Physics2D.Raycast(transform.position - new Vector3(0.6f, 0.0f, 0.0f), Vector2.down, 10f);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(0.6f, 0.0f, 0.0f), Vector2.down, 10f);

        Debug.DrawRay(transform.position - new Vector3(0.6f, 0.0f, 0.0f), Vector2.down * 10f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0.6f, 0.0f, 0.0f), Vector2.down * 10f, Color.red);

        Debug.DrawLine(hit1.point, hit2.point, Color.blue);

        Vector2 direction = (hit2.point - hit1.point).normalized;


        // SPRITES
        if (grounded)
        {
            targetDir = direction;
            turnSpeed = 20f;
        }
        else
        {
            targetDir = (direction + Vector2.right * (Vector2.Distance(hit1.point, transform.position) / 10f));
            //Vector3.Normalize(targetDir);


        }


        sprite.transform.right = Vector3.Slerp(sprite.transform.right, targetDir, turnSpeed);


        // HORIZONTAL MOVEMENT
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            dir = transform.right;
            spriteDir = sprite.transform.right;

            sprite.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            dir = -transform.right;
            spriteDir = -sprite.transform.right;

            sprite.transform.localScale = new Vector3(-1, 1, 1);
        }


        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            rb.AddForce(-rb.velocity);
        }
        else
        {
            rb.AddForce(dir * step / 4f + spriteDir * step);
        }

        // JUMP

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
            jumpTimer = 1.3f;
        }

        if (Input.GetButton("Jump"))
        {
            jumpTimer -= Time.deltaTime;
            rb.AddForce(transform.up * (jumpForce / 150) * jumpTimer);
        }


        if (Input.GetAxisRaw("Vertical") < 0)
        {
            //rb.AddForce(-sprite.transform.up * Mathf.Abs(rb.velocity.x) * downForce * Time.deltaTime + -transform.up * downForce * Time.deltaTime);

            float sweetSport = Mathf.Clamp(Vector2.Distance(hit1.point, transform.position), 0, 1f);
            rb.AddForce(-sprite.transform.up * Mathf.Abs(rb.velocity.x) * downForce * (sweetSport * 2f) * Time.deltaTime);
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            grounded = true;
            ContactPoint2D point = collision.GetContact(0);

            
            groundPoint.position = point.point;

            var dir = transform.position - groundPoint.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            groundPoint.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } 
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {

            //rb.mass = 0.5f;
            rb.gravityScale = 0.6f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            //rb.mass = 0.35f;

            grounded = false;

            groundPoint.position = transform.position;

            takeoffForce = Mathf.Abs(rb.velocity.x);

            groundPoint.position = transform.position;
        }
    }

}
