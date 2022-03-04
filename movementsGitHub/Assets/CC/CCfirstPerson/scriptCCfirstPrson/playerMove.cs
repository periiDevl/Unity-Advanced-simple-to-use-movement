using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class playerMove : MonoBehaviour
{
   

    [HideInInspector] public bool moving;
    [Header("ground movement")]
    [SerializeField] CharacterController controller;
    [HideInInspector] public float speed = 12f;

    [SerializeField] float maxSpeed = 10;
    [SerializeField] float minSpeed = 3;
    float maxSpeedd;
    [Range(0, 10f)]
    public float acceleration = 1;
    [Range(0, 1)]
    public float accelerationTime = 0.5f;
    public AnimationCurve movementCurve;
    [SerializeField] float moumentumDrag = 3f;
    public float groundcheckOffset = 2;
    public float groundDISTENCE = 0.4f;
    public LayerMask groundMask;
    float bmaxSpeed;

    [Header("camera")]

    public float mosuseSence = 100f;
    private Vector3 startPos;
    public Transform cam;

    float xRotateion = 0f;


    [Header("crouching")]
    
    public bool crouching;
    [SerializeField] float crouvhingSpeed;
    private float slideingSpeed;
    [SerializeField] float slideBuffer;
    private float localScale;
    private float halfLocalScale;
    public float myAng = 0.0f;
    [SerializeField] float slideMaxSpeed;


    [Header("airMovement")]
    public float JumpForce = 3f;
    float BjumpForce;

    [Range(1, 5)]
    public int jumps = 2;
    int bJumps;
    [SerializeField] float airMult = 2;

    public float gravity = -9.81f;
    bool isMoreOneThenOneJumps;

    public float DragSpeed;
    private float BdragSpeed;
    public float dragMini;
    public float DragAdder = 0.5f;

    float spee;

    bool disableInput = false;

    Vector3 velocity;
    bool isGrounded;
    float bGrav;


    [Header("game feel")]
    [SerializeField] bool haveDash;
    [SerializeField] bool slowDownNearWall = true;
    [SerializeField] bool exrtraJumpNearAWall;
    

    //wall climb option
    enum WallRunType { none, nomral, fun }
    [SerializeField] WallRunType wallRunType;
    [SerializeField] bool vaulting = true;
    [SerializeField] bool cyoteTime = true;


    bool camValtReady;
    bool objValtReady;
    bool canValt = true;

    [SerializeField] bool lockCursor = true;
    [Header("valting")]
    public bool useSkey = false;
    [SerializeField] bool speedBostWhenValing = true;
    [SerializeField] float valtSpeed = 10;
    [SerializeField] float upVatforce = 5;
    [SerializeField] float range = 1;
    [SerializeField] Vector3 Upoffset;
    [SerializeField] Vector3 Downoffset;

    [Header("cyoteTime")]
    [SerializeField] float timetoJumpTime = 0.5f;
    [HideInInspector] public float mayJump;


    [Header("wallclimb")]
    [SerializeField, Range(0, 500)] float WallMaxSpeed;
    [SerializeField] bool usingGroundLayer;
    [SerializeField] bool disableMovementAfterLeavingWall = true;
    [SerializeField, Range(0, 25)] float wallacceloration;
    [SerializeField] float wallJumpsGravity = 1;
    [SerializeField] LayerMask wallMask;
    [SerializeField] float cameraAngle = 0.1f;
    [SerializeField] float wallRunJumpForce;
    [SerializeField] float camFov;
    [SerializeField] float wallcamfov;
    [SerializeField] float wallRunfovTime;
    [SerializeField] float camTiltTime;
    [SerializeField] private float wallDistance = .5f;
    [SerializeField] float whileWallRunningBuffer = 2;
    public float tilt { get; private set; }
    public bool wallLeft = false;
    public bool wallRight = false;
    [SerializeField] Camera cameraComponent;


    [Header("DashFunction")]
    public float dashLenth = 1;
    public float DashColldown;
    public float dashSpeed;

    bool ableDash = true;
    public bool isDashing;
    
    public int dashAttempts;
    private float dashStartTime;

    public Vector3 movementVector = Vector3.zero;







    [Header("debuffs")]
    [SerializeField] float nwalldst = 1;
    bool nwall;
    bool justNearwall;
    bool beforeWall = true;
    [SerializeField] float nwalldst_ver = 1;
    bool nwall_ver;
    [SerializeField] Vector3 VerOffset;
    bool alertMove;
    private float t;
    private float dirX = 0f;
    private float dirZ = 0f;
    private float bminSpeed;
    private bool slideing = false;



    // Start is called before the first frame update
    void Start()
    {
        BjumpForce = JumpForce;
        bminSpeed = minSpeed;
        BdragSpeed = DragSpeed;
        if (jumps > 1)
        {
            jumps--;

            isMoreOneThenOneJumps = true;

        }
        else if (jumps == 1)
        {
            jumps--;
            isMoreOneThenOneJumps = false;

        }
        bJumps = jumps;
        bGrav = gravity;
        bmaxSpeed = maxSpeed;
        maxSpeedd = maxSpeed / 5;
        speed = minSpeed;

        startPos = cam.localPosition;

        controller.stepOffset = 0;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        localScale = transform.localScale.y;
        halfLocalScale = localScale / 2f;

    }

    // Update is called once per frame
    void Update()
    {

        Movement();

        HandleSpeed();
        airMovement();
        lookWithCam();
        optionsmanager();




        if (cyoteTime)
        {
            if (isGrounded) { mayJump = timetoJumpTime; }

            mayJump -= Time.deltaTime;

        }
        else
        {
            mayJump = 0;
        }

    }


    void optionsmanager()
    {
        if (slowDownNearWall)
        {
            WallDetection();
        }
        if (!isGrounded && vaulting)
        {
            valting();
        }

        if (wallRunType != WallRunType.none)
        {
            wallClimb();

        }
        if (haveDash)
        {
            HandleDash();
        }
        CalAngleandCrouch();
    }

    void CalAngleandCrouch()
    {
        if (!slideing && crouching)
        {
            JumpForce = 0;
        } else
        {
            JumpForce = BjumpForce;
        }
        if (slideing)
        {
            
            
            
            maxSpeed = slideMaxSpeed;
            speed = maxSpeed;

        }
        if (!slideing)
        {
            minSpeed = bminSpeed;
            slideingSpeed = slideingSpeed + Time.deltaTime * 50;
            if (slideingSpeed > 0)
            {
                slideingSpeed = 0;
                velocity.x = slideingSpeed;
                velocity.z = slideingSpeed;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
        if (!crouching)
        {

            //slideingSpeed = 0;
            if (!wallLeft && !wallRight)
            {
                gravity = bGrav;
            }
        }
        if (slideing && Input.GetKeyDown(KeyCode.Space))
        {
            crouching = false;
        }

        
        if (crouching)
        {
            if (isGrounded && !slideing)
            {
                speed = crouvhingSpeed;
            }
            transform.localScale = new Vector3(localScale, halfLocalScale, localScale);
            print(slideingSpeed);
            if (isGrounded && crouching)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + new Vector3(0, -.001f, 0), Vector3.down, out hit, controller.height / 2) && crouching)
                {
                    if (-Vector3.Angle(hit.normal, Vector3.up) < myAng && isGrounded && crouching)
                    {
                        slideing = true;

                        

                        if (!Input.GetKeyDown(KeyCode.Space))
                        {
                            Vector3 newPos = new Vector3(0, -200000 * Time.deltaTime * slideingSpeed, 0);
                            Mathf.Lerp(transform.position.y, newPos.y, 0);
                            velocity = -hit.transform.forward * -slideingSpeed;
                            //velocity = hit.normal.normalized + new Vector3(0, 0, -slideingSpeed);
                            gravity = -10000000;
                            
                            slideingSpeed -= slideBuffer * Time.deltaTime;
                            //velocity.y -= 1000 * Time.deltaTime;
                            

                        }
                        else
                        {
                            if (!wallLeft || !wallRight)
                            {
                                gravity = bGrav;
                            }
                            
                            minSpeed = bminSpeed;
                        }

                        //speed = 0;
                        //minSpeed = 0;

                    }
                    else
                    {
                        slideing = false;

                        minSpeed = bminSpeed;

                        if (!wallLeft || !wallRight)
                        {
                            gravity = bGrav;
                        }
                        //velocity.x = 0;
                        //velocity.z = 0;


                    }
                }


            }
        }
        else
        {
            transform.localScale = new Vector3(localScale, localScale, localScale);
            
            
            slideing = false;
        }
    }

    void BringInput()
    {
        disableInput = false;
    }

    void HandleDash()
    {
        

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            if (dashAttempts > 0 && ableDash)
            {
                 OnStartDash();
            }
        }

        if (isDashing)
        {
            if (Time.time - dashStartTime <= dashLenth)
            {
                if (movementVector.Equals(Vector3.zero))
                {

                    controller.Move(transform.forward * dashSpeed * Time.deltaTime);
                } else
                {
                    
                    controller.Move(movementVector.normalized * dashSpeed * Time.deltaTime);  
                    
                }
            }
            else
            {
                OnEndDash();
            }
        } 
    }

    

    void OnStartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashAttempts += 1;
    }

    void OnEndDash()
    {
        ableDash = false;
        dashStartTime = 0;
        isDashing = false;
        if (!IsInvoking("BringDash")) {
            Invoke("BringDash", DashColldown);
        }
    }

    void BringDash()
    {
        ableDash = true;
        
    }
    void wallClimb()
    {
        if (isGrounded || slideing) { return; }
        RaycastHit leftWallHit;
        RaycastHit rightWallHit;
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallDistance, wallMask);
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallDistance, wallMask);
        cam.Rotate(0, 0, tilt);




        if (Input.GetKeyDown(KeyCode.Space) && moving)
        {
            if (wallLeft)
            {
                disableInput = true;
                if (!IsInvoking("BringInput") && !isGrounded && disableMovementAfterLeavingWall)
                {
                    Invoke("BringInput", 5f);
                }
                moving = true;
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                gravity = bGrav;
                
                controller.Move(wallRunJumpDirection * wallRunJumpForce);
                

                
            }
            else if (wallRight)
            {
                disableInput = true;
                if (!IsInvoking("BringInput") && !isGrounded && disableMovementAfterLeavingWall)
                {
                    Invoke("BringInput", 5);
                }
                moving = true;
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                gravity = bGrav;
                
                controller.Move(wallRunJumpDirection * wallRunJumpForce);
                
            }
        } else if (moving)
        {
            gravity = bGrav;
        }



        if (wallRight)
        {
            Vector3 wallRunJumpDirection = transform.forward + leftWallHit.normal;
            

            controller.Move(wallRunJumpDirection * whileWallRunningBuffer);

            gravity = wallJumpsGravity;

            if (usingGroundLayer)
            {
                slowDownNearWall = false;
            }
            if (wallRunType == WallRunType.fun)
            {
                jumps = bJumps;
            }
            if (WallMaxSpeed >= maxSpeed)
            {
                speed = maxSpeed;
                maxSpeed = maxSpeed + wallacceloration;
            }

            maxSpeedd = maxSpeed;



            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, wallcamfov, wallRunfovTime * Time.deltaTime);
            tilt = Mathf.Lerp(tilt, -cameraAngle, camTiltTime * Time.deltaTime);
        }
        else if (wallLeft)
        {
            Vector3 wallRunJumpDirection = transform.forward + rightWallHit.normal;
            controller.Move(wallRunJumpDirection * whileWallRunningBuffer);
            gravity = wallJumpsGravity;
            if (usingGroundLayer)
            {
                slowDownNearWall = false;
            }

            if (wallRunType == WallRunType.fun)
            {

                jumps = bJumps;
            }
            if (WallMaxSpeed >= maxSpeed)
            {
                speed = maxSpeed;
                maxSpeed = maxSpeed + wallacceloration;
            }

            maxSpeedd = maxSpeed;




            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, wallcamfov, wallRunfovTime * Time.deltaTime);
            tilt = Mathf.Lerp(tilt, cameraAngle, camTiltTime * Time.deltaTime);

        }
        if (!wallRight && !wallLeft)
        {
            

            gravity = bGrav;


            if (usingGroundLayer)
            {
                slowDownNearWall = true;
            }

            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, camFov, wallRunfovTime * Time.deltaTime);
            tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);




        }


    }
    void WallDetection()
    {
        if (speed < minSpeed)
        {
            maxSpeed = bmaxSpeed;
            speed = minSpeed;
        }
        nwall = Physics.CheckSphere(transform.position, nwalldst, groundMask);

        if (nwall && beforeWall && exrtraJumpNearAWall)
        {
            justNearwall = true;
            jumps++;
            beforeWall = false;
            if (!IsInvoking("backwall"))
            {
                Invoke("backwall", 0.6f);
            }
        }



        if (nwall && speed != minSpeed)
        {
            maxSpeed = bmaxSpeed;
            speed = minSpeed;
        }



    }
    void backwall()
    {
        if (!nwall)
        {
            beforeWall = true;
        }
    }
    void addSpeed()
    {
        if (maxSpeedd <= maxSpeed)
        {
            maxSpeedd++;
        }
    }
    void airMovement()
    {

        //airMovement
        if (mayJump > 0 || isGrounded)
        {
            jumps = bJumps;
        }
        if (!isGrounded)
        {
            maxSpeedd *= airMult * acceleration;
        }
    }
    void Movement()
    {
        Vector3 GCoffset = new Vector3(transform.position.x, transform.position.y - groundcheckOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(GCoffset, groundDISTENCE, groundMask);
        if (isGrounded)
        {
            BringInput();
        }
        if (isGrounded && velocity.y < 0)
        {
            //add curve
            velocity.y = -2;
        }
        if (disableInput == false)
        {

            float z = Input.GetAxis("Vertical");
            float x = Input.GetAxis("Horizontal");
            movementVector = transform.right * x + transform.forward * z;
            movementVector.Normalize();
            if (!slideing)
            {
                controller.Move(movementVector * speed * Time.deltaTime);
            }
            
            

        } else
        {
            float z = Input.GetAxis("Vertical");
            
            Vector3 move = transform.forward * z;
            move.Normalize();
            if (!slideing)
            {
                controller.Move(move * speed * Time.deltaTime);
            }
        }




        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0 && isMoreOneThenOneJumps)
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isMoreOneThenOneJumps && mayJump > 0)
        {
            velocity.y = Mathf.Sqrt(JumpForce * -2f * gravity);

        }







        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
            Phyisics();
        

    }

    void Phyisics()
    {
        CanDrag();














    }



    void CanDrag()
    {
        
            if (spee < DragSpeed && moving && speed != minSpeed)
            {
                spee = spee + DragAdder * Time.deltaTime;
            }



            if (Input.GetKey(KeyCode.W))
            {
                dirZ = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dirZ = -1;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                dirZ = 0;
            }

            if (Input.GetKey(KeyCode.D))
            {
                dirX = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                dirX = -1;
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                dirX = 0;
            }




            //  if (keepDrag)
            //{

            if (!moving && spee > 0)
            {
                spee = spee - dragMini * Time.deltaTime;
            }

            if (spee <= 0)
            {
                spee = 0;
            }


            Vector3 move = transform.right * dirX + transform.forward * dirZ;

            if (!moving && !slideing && !crouching)
            {
                controller.Move(move * spee);
            }





        

        //   }
    }

    void HandleSpeed()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }

        if (moving)
        {

        }
        //drag
        if (!moving)
        {
            speed = minSpeed;
            maxSpeed = bmaxSpeed;
        }
        if (x! > 0 || x! < 0 || z! > 0 || z! < 0 && moving)
        {
            alertMove = true;
        }
        else
        {
            alertMove = false;
        }
        if (x > 0 || x < 0 || z > 0 || z < 0)
        {
            moving = true;

            if (speed <= maxSpeed)
            {
                //curve
                if (speed < maxSpeed)
                {
                    speed = movementCurve.Evaluate(t);
                    t = t + acceleration * Time.deltaTime;


                }
                //  t += Time.deltaTime;
                //  speed += acceleration;
                if (!IsInvoking("addSpeed"))
                {
                    Invoke("addSpeed", accelerationTime);
                }
            }

        }
        else
        {
            moving = false;
            t = minSpeed;
        }

    }


    void Jump()
    {
        jumps--;
        velocity.y = Mathf.Sqrt(JumpForce * -2f * gravity);

    }
    void lookWithCam()
    {
        float mouseX = Input.GetAxis("Mouse X") * mosuseSence * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mosuseSence * Time.deltaTime;

        xRotateion -= mouseY;
        xRotateion = Mathf.Clamp(xRotateion, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotateion, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
    void valting()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (canValt && moving)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Downoffset, transform.forward, out hit, range, groundMask))
            {
                objValtReady = true;
            }
            else
            {
                objValtReady = false;
            }
            RaycastHit camhit;

            if (Physics.Raycast(transform.position + Upoffset, transform.forward, out camhit, range))
            {
                //if it hits the ready will be false
                camValtReady = false;
            }
            else
            {
                camValtReady = true;
            }

            if (camValtReady && objValtReady)
            {
                jumps = bJumps;
                controller.Move(Vector3.forward.normalized * valtSpeed * Time.deltaTime);
                controller.Move(Vector3.up.normalized * upVatforce * Time.deltaTime);
                camValtReady = false;
                objValtReady = false;
                if (!useSkey)
                {
                    canValt = false;
                } else
                {
                    if (!Input.GetKey(KeyCode.S))
                    {
                        canValt = true;
                    }
                }

                if (speedBostWhenValing)
                {
                    speed = maxSpeed;
                }
                if (!IsInvoking("StartValtD") && !useSkey)
                {
                    Invoke("StartValtD", 0.23f);
                }
            }
        }
    }
    void StartValtD()
    {
        canValt = true;
    }
    private void OnDrawGizmos()
    {
        if (vaulting)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + Upoffset, transform.forward);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Downoffset, transform.forward);
        }
        Gizmos.color = Color.cyan;
        Vector3 GCoffset = new Vector3(transform.position.x, transform.position.y - groundcheckOffset, transform.position.z);
        Gizmos.DrawSphere(GCoffset, groundDISTENCE);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, nwalldst);
        Gizmos.color = Color.grey;
        Gizmos.DrawRay(transform.position, -transform.right);
        Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + VerOffset, nwalldst_ver);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, wallDistance);


    }


    



}
