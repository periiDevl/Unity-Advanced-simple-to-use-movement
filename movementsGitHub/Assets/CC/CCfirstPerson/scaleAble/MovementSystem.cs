using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementSystem : MonoBehaviour
{
    private CharacterController controller;
    
    [HideInInspector] public AnimationCurve AccelerationMovementCurve;
    [HideInInspector] public float TimeToDecelerate;


    [HideInInspector] public AnimationCurve DecelerationCurve;
    private float speed = 12f;

    private Vector3 GroundCheckOffset;
    private float GroundDistance = 0;
    [HideInInspector] public LayerMask GroundLayerMask;

   [HideInInspector] public float JumpForce = 3f;

    public float slideSpeed;
    Vector3 velocity;
    bool isGrounded;

    [HideInInspector] public float Gravity = -9.81f;

    [Header("camera")]
    public float camFallBuffer = 4;
    
    public float mouseSpeed = 100f;
    private Vector3 startPos;
    Transform cam;

    float xRot = 0f;

    private float t;
    private float d;
    
    bool moving;

    private float dirX = 0f;
    private float dirZ = 0f;

    private float dirXx = 0f;
    private float dirZz = 0f;

    private float bGrav;

    
    //slide
    public float slopeSpeed;
    public float SlopeLimit = 45;
    private Vector3 hitPointNormal;
    private bool isSliding
    {
        get
        {
            if (isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f) && !Input.GetKey(KeyCode.LeftControl))
            {
                hitPointNormal = slopeHit.normal;
               
                return Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit;
                
            }
            else
            {
                return false;
            }
            
        }
    }

    private bool isSlidingAndC
    {
        get
        {
            if (isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f) && Input.GetKey(KeyCode.LeftControl))
            {
                hitPointNormal = slopeHit.normal;

                return Vector3.Angle(hitPointNormal, Vector3.up) > 0;

            }
            else
            {
                return false;
            }

        }
    }




    Vector3 movementVector;
    [Header("debugging")]
    [SerializeField] bool showKeys = false;
    // Start is called before the first frame update
    void Start()
    {
        SetValues();
    }

    void SetValues()
    {
         
        
        bGrav = Gravity;

        Cursor.lockState = CursorLockMode.Locked;
        //adding a camera and setting it at the right position
        var cameraObj = new GameObject("players_camera");


        cameraObj.AddComponent<Camera>();
       
        cam = cameraObj.transform;
        cam.position = new Vector3(transform.position.x, transform.localScale.y, transform.position.z);
        
        Keyframe keyframe = new Keyframe(0, 0);
        AccelerationMovementCurve.MoveKey(0, keyframe);


        TimeToDecelerate = TimeToDecelerate * 10;
        //locks the mouse
        //adding the controller
        controller = gameObject.AddComponent<CharacterController>();
        //collision detection
        GroundDistance = transform.localScale.z / 2;
        controller.stepOffset = 0;
        GroundCheckOffset = new Vector3(0, -transform.localScale.y, 0);

        if (TimeToDecelerate <= 0)
        {
            TimeToDecelerate = 0.00000000000000000000000000000000000001f;
        }
    }

    // Update is called once per frame
    void Update()
    {

        
        GetInput();
        lookWithCam();
        
        controller.slopeLimit = SlopeLimit;
        if (isSliding)
        {
            
            velocity += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        } else
        {
            //complite later
            if (velocity.x > 0)
            {
                velocity.x -= Time.deltaTime * slopeSpeed * 2; ;
            } else if (velocity.x < 0)
            {
                velocity.x += Time.deltaTime * slopeSpeed * 2;
            }

            if (velocity.z > 0)
            {
                velocity.z -= Time.deltaTime * slopeSpeed * 2;
            } else if (velocity.z < 0)
            {
                velocity.z += Time.deltaTime * slopeSpeed * 2;
            }
            
        }

        if (isSlidingAndC)
        {
            velocity += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slideSpeed; 
        }
        else
        {
            //complite later
            if (velocity.x > 0)
            {
                velocity.x -= Time.deltaTime * slopeSpeed * 5; ;
            }
            else if (velocity.x < 0)
            {
                velocity.x += Time.deltaTime * slopeSpeed * 5;
            }

            if (velocity.z > 0)
            {
                velocity.z -= Time.deltaTime * slopeSpeed * 5;
            }
            else if (velocity.z < 0)
            {
                velocity.z += Time.deltaTime * slopeSpeed * 5;
            }

        }




    }

   

    void GetInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //slopy
        if (Input.GetKey(KeyCode.W))
        {
            dirZ = .1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dirZ = -.1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            dirZ = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dirX = .1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dirX = -.1f;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            dirX = 0;
        }

        
        //normal
        if (Input.GetKey(KeyCode.W))
        {
            dirZz = .1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dirZz = -.1f;
        }
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            dirZz = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dirXx = .1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dirXx = -.1f;
        }
        else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            dirXx = 0;
        }


        CallInputBasedMovement(dirX, dirZ, dirXx, dirZz);
    }

    void CallInputBasedMovement(float xSlippery, float zSlippery, float x, float z)
    {
        GroundMovement(xSlippery, zSlippery);
        HandleSpeed(x, z, xSlippery, zSlippery);
       
        
    }

    void GroundMovement(float x, float z)
    {
        isGrounded = Physics.CheckSphere(transform.position + GroundCheckOffset, GroundDistance, GroundLayerMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        movementVector = transform.right * x + transform.forward * z;
        controller.Move(movementVector * speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpForce * -2f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleSpeed(float x, float z, float xSlip, float zSlip)
    {

        //Vector3 lastPosition

        if (x > 0 || x < 0 || z > 0 || z < 0)
        {
            d = 0;
            moving = true;
            speed = AccelerationMovementCurve.Evaluate(t);
            t = t + Time.deltaTime;
        }
        else
        {



            speed = DecelerationCurve.Evaluate(d);
            d = d + Time.deltaTime;
            t = 0;
           // Vector3 movementSlipperyVector = transform.right * xSlip + transform.forward * zSlip;
           // controller.Move(movementSlipperyVector * speed);
            moving = false;




        }
        Keyframe SecondDesKey = new Keyframe(TimeToDecelerate, 0);
        DecelerationCurve.MoveKey(1, SecondDesKey);

        Keyframe speedDesKey = new Keyframe(0, speed);
        DecelerationCurve.MoveKey(0, speedDesKey);
    }

    void lookWithCam()
    {
        

        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        cam.transform.SetParent(this.transform);

        

        

        
    }

     
   
    


    //Gizmos Ui
    private void OnDrawGizmos()
    {
        if (!isGrounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + GroundCheckOffset, GroundDistance);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + GroundCheckOffset, GroundDistance);
        }
    }

    private void OnGUI()
    {

        if (showKeys)
        {
            if (Input.GetKey(KeyCode.A))
            {

                GUI.color = Color.green;
                GUI.Box(new Rect(15, 60, 30, 30), "A");
            }
            else
            {

                GUI.color = Color.red;
                GUI.Box(new Rect(15, 55, 30, 30), "A");
            }

            if (Input.GetKey(KeyCode.D))
            {

                GUI.color = Color.green;
                GUI.Box(new Rect(100, 60, 30, 30), "D");
            }
            else
            {

                GUI.color = Color.red;
                GUI.Box(new Rect(100, 55, 30, 30), "D");
            }

            if (Input.GetKey(KeyCode.W))
            {

                GUI.color = Color.green;
                GUI.Box(new Rect(60, 10, 30, 30), "W");
            }
            else
            {

                GUI.color = Color.red;
                GUI.Box(new Rect(60, 15, 30, 30), "W");
            }

            if (Input.GetKey(KeyCode.S))
            {

                GUI.color = Color.green;
                GUI.Box(new Rect(60, 60, 30, 30), "S");
            }
            else
            {

                GUI.color = Color.red;
                GUI.Box(new Rect(60, 55, 30, 30), "S");
            }
            GUI.color = Color.cyan;
            GUI.TextArea(new Rect(15, 500, 30, 30), speed.ToString());
            
        }

    }
}
