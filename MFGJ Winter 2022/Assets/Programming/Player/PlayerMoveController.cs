using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D RB;
    [SerializeField] private float walkSpeed = 5f, sprintSpeed;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField, Range(0, 1)] float staminaDec, staminaInc;
    float stamina = 1;
    bool isSprinting;
    Vector2 moveInput;
    private Animator anim;
    private SpriteRenderer SR;

    GameUI gameUI;


    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        AliveCheck.changeAliveState(true);
        anim = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();

        gameUI = GameUI.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("X", moveInput.x);
        anim.SetFloat("Y", moveInput.y);
        moveInput = moveInput.normalized;

        if (Input.GetKey(sprintKey))
        {
            if (stamina > 0 && moveInput != Vector2.zero)
            {
                stamina -= Time.deltaTime * staminaDec;
                isSprinting = true;
                Mathf.Clamp01(stamina);
                gameUI.SetPlayerStaminaBar(stamina, 1);
            }
            else
                isSprinting = false;
        }
        else
            isSprinting = false;
    }

    void FixedUpdate()
    {
        moveInput *= (isSprinting ? sprintSpeed : walkSpeed) * Time.fixedDeltaTime;
        
        RB.MovePosition((Vector2)transform.position + moveInput);
        SR.sortingOrder = Mathf.CeilToInt(transform.position.y * 100) * -1;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crow"))
        {
            collision.GetComponent<CrowController>().ScareCrow(transform.position);
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Crow"))
        {
            collision.GetComponent<CrowController>().ScareCrow(transform.position);
        }
        if (collision.CompareTag("Hay"))
        {
            if (stamina < 1 && !Input.GetKey(sprintKey))
            {
                stamina += Time.deltaTime * staminaInc;
                Mathf.Clamp01(stamina);
                gameUI.SetPlayerStaminaBar(stamina, 1);
            }
        }
    }
}


public static class AliveCheck
{
    static bool alive = true;
    public delegate void deathVoid();
    public static event deathVoid DeathTrigger;


    public static bool TestForAlive()
    {
        return alive;
    }

    public static void changeAliveState(bool state)
    {
        alive = state;
        if (!alive)
        {
            DeathTrigger?.Invoke();
        }
    }
}
