using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D RB;
    [SerializeField] private float walkSpeed = 5f, sprintSpeed;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] float hayBarnInc, hayDec, haySprintDec;
    float hay = 1;
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
            if (hay > 0 && moveInput != Vector2.zero)
            {
                hay -= Time.deltaTime * haySprintDec;
                isSprinting = true;

            }
            else
                isSprinting = false;
        }
        else
        {
            isSprinting = false;
            hay -= Time.deltaTime * hayDec;
        }
        Mathf.Clamp01(hay);
        gameUI.SetPlayerStaminaBar(hay, 1);
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
            ScareCropEater(collision.GetComponent<CrowController>());
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Crow"))
            ScareCropEater(collision.GetComponent<CrowController>());
        else if (collision.CompareTag("Hay"))
        {
            if (hay < 1 && !Input.GetKey(sprintKey))
            {
                hay += Time.deltaTime * hayBarnInc;
                Mathf.Clamp01(hay);
                gameUI.SetPlayerStaminaBar(hay, 1);
            }
        }
    }

    void ScareCropEater(CrowController cropEater)
    {
        if (hay > 0)
            cropEater.ScareCrow(transform.position);
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
