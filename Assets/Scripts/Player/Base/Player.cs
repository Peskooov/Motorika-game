using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoSingleton<Player>
{
    public event UnityAction RunEvent; // event
    public event UnityAction IdleEvent;
    public event UnityAction JumpEvent;
    public event UnityAction SlideEvent;

    [Header("Run/Slide")]
    [SerializeField] private float runSpeed = 5f; //run velocity
    [SerializeField] private float slideSpeed = 10f; // slide speed
    [SerializeField] private float slideControlTime = 0.6f; // max slide time
    [Header("Jump")]
    [SerializeField] private float maxJumpForce; // additional max jump force
    [SerializeField] private float chargeRate; // max jump time
    [Header("Damage")]
    [SerializeField] private int damage = 10;
    [Header("Raycast")]
    [SerializeField] private float raycastDistanceForward = 1.5f; // Raycast distance from player to value;
    [SerializeField] private float raycastDistanceDown = 1.5f; // Raycast distance from player to value;
    [SerializeField] private float rayPositionTop = 0.5f; // Start position Ray on Top 
    [SerializeField] private float rayPositionBot = 0.5f; // Start position Ray on Bottom 

    private Animator animator;

    private bool isGrounded; // check ground
    private bool isJumping; // check jump
    private bool isSliding; // check slide

    private float currentJumpForce = 0f;
    private float slideTime = 0;

    private Rigidbody rb;

    private Vector3 raycastDownPosition;
    private Vector3 raycastTopPosition;
    private Vector3 raycastBottomPosition;

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        animator= GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckGround();
        IsBarrier();

        if (!Input.GetKey(KeyCode.Space) && isGrounded && !Input.GetKey(KeyCode.S) && !isSliding && !IsBarrier())
            animator.SetTrigger("Run");
    }

    private void FixedUpdate()
    {
        rb.freezeRotation = true;
        transform.up = Vector3.up;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    public void Idle()
    {
        transform.Translate(Vector3.forward * 0 * Time.deltaTime);
        animator.SetTrigger("Idle");
    }

    public void Run()
    {
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);

        isSliding = false;
        isJumping = false;
    }

    public void Slide()
    {
        if (!isGrounded) return;

        animator.SetTrigger("Slide");


        if (slideTime < slideControlTime)
        {
            transform.Translate(Vector3.forward * slideSpeed * Time.deltaTime);
            slideTime += Time.deltaTime;
            isSliding = true;
        }
        else
        {
            slideTime = 0;
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        animator.SetTrigger("Jump");

        if (currentJumpForce < maxJumpForce)
        {
            rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            currentJumpForce += Time.deltaTime * chargeRate;
        }
        else
            currentJumpForce = 0;
    }

    public void Parry(float parryForce)
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        rb.AddForce(-transform.forward * parryForce, ForceMode.Impulse);
    }

    public bool IsBarrier()
    {
        raycastTopPosition = new(transform.position.x, transform.position.y  + rayPositionTop, transform.position.z);
        raycastBottomPosition = new(transform.position.x, transform.position.y + rayPositionBot, transform.position.z);
        bool hitBottom = Physics.Raycast(raycastBottomPosition, transform.forward, out _, raycastDistanceForward);
        bool hitTop = Physics.Raycast(raycastTopPosition, transform.forward, out _, raycastDistanceForward);

        if (hitBottom || hitTop)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckGround()
    {
        raycastDownPosition = new(transform.position.x, transform.position.y + rayPositionBot, transform.position.z);

        isGrounded = Physics.Raycast(raycastDownPosition, -transform.up, out _, raycastDistanceDown);
    }

    public void Fire(TurretMode mode)
    {
        return;
    }

    public bool DrawEnergy(int count)
    {
        return true;
    }

    public bool DrawAmmo(int count)
    {
        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(raycastDownPosition, -transform.up * raycastDistanceDown);
        Gizmos.DrawRay(raycastTopPosition, transform.forward * raycastDistanceForward);
        Gizmos.DrawRay(raycastBottomPosition, transform.forward * raycastDistanceForward);
    }
#endif
}