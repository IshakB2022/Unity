using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    public float speed = 0.1f;
    public Camera cam;
    private bool isGrounded, jump;
    private Vector3 groundContactNormal;
    public float jumpForce = 10.0f;
    public MouseLook mouseLook = new MouseLook();



   
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        mouseLook.Init(transform, cam.transform);

    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
        //CheckGround();
        if (Input.GetKeyDown(KeyCode.Space) && !jump)
        {
            jump = true;

           
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        float x = Input.GetAxis("Horizontal") ;
        float z = Input.GetAxis("Vertical") ;


        if (Mathf.Abs(x) > float.Epsilon || Mathf.Abs(z) > float.Epsilon && isGrounded)
        {
            Vector3 desiredMove = cam.transform.forward * z + cam.transform.right * x;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;
            desiredMove = desiredMove * speed;  


            if (rigidbody.velocity.sqrMagnitude < speed * speed)
            {
                rigidbody.AddForce(desiredMove * 0.1f, ForceMode.Impulse);
            }
        }

        if (isGrounded)
        {
            rigidbody.drag = 5f;
            if (jump)
            {
                rigidbody.drag = 0;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
                rigidbody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);

            }

            if(Mathf.Abs(x) < float.Epsilon && Mathf.Abs(z) <float.Epsilon && rigidbody.velocity.magnitude < 1f)
            {
                rigidbody.Sleep();
            }
        }


        jump = false;


    }

    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down, out hit,
        ((capsuleCollider.height / 2f) - capsuleCollider.radius) + 0.01f, Physics.AllLayers, QueryTriggerInteraction.Ignore)){
            isGrounded = true;
            groundContactNormal = hit.normal;


        }
        else
        {
            isGrounded = false;
            groundContactNormal = Vector3.up;
        }
    }

   

 private void RotateView()  {         //avoids the mouse looking if the game is effectively paused         if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;          // get the rotation before it's changed         float oldYRotation = transform.eulerAngles.y;          mouseLook.LookRotation(transform, cam.transform);          if (isGrounded || jump)         {             // Rotate the rigidbody velocity to match the new direction that the character is looking             Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);             rigidbody.velocity = velRotation * rigidbody.velocity;         }     } 

}
