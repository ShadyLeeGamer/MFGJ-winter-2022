using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D RB;
    [SerializeField] private float walkSpeed = 5f, sprintSpeed;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    Vector2 moveInput;
    bool isSprinting;
    [SerializeField] float hayBarnInc, hayDec, haySprintDec;
    float hay = 1;
    float hayGainParticleEmmissionRate;

    private Animator anim;
    private SpriteRenderer SR;

    bool inBarn;
    [SerializeField] ParticleSystem hayFallParticle, hayGainParticle;
    [SerializeField] float hayFallParticleRateNormal, hayFallParticleRateSprinting;
    [SerializeField] float hayGainParticleRateNormal;

    GameUI gameUI;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        AliveCheck.changeAliveState(true);
        anim = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();

        gameUI = GameUI.Instance;
        hayGainParticleEmmissionRate = hayGainParticle.emission.rateOverTime.constant;
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
        else if (!inBarn)
        {
            isSprinting = false;
            hay -= Time.deltaTime * hayDec;
        }
        Mathf.Clamp01(hay);
        gameUI.SetPlayerStaminaBar(hay, 1);

        UpdateHayFallParticle();
        UpdateHayGainParticle();
    }

    void FixedUpdate()
    {
        moveInput *= (isSprinting ? sprintSpeed : walkSpeed) * Time.fixedDeltaTime;
        
        RB.MovePosition((Vector2)transform.position + moveInput);
        SR.sortingOrder = Mathf.CeilToInt(transform.position.y * 100) * -1;
    }

    void UpdateHayFallParticle()
    {
        ParticleSystem.EmissionModule particleEmission = hayFallParticle.emission;
        particleEmission.rateOverTime = !inBarn && hay > 0 ? (isSprinting ? hayFallParticleRateSprinting
                                                                          : hayFallParticleRateNormal)
                                                           : 0;
    }
    void UpdateHayGainParticle()
    {
        ParticleSystem.EmissionModule particleEmission = hayGainParticle.emission;
        particleEmission.rateOverTime = inBarn && hay < 1 ? hayGainParticleRateNormal : 0;
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
                inBarn = true;
                hay += Time.deltaTime * hayBarnInc;
                Mathf.Clamp01(hay);
                gameUI.SetPlayerStaminaBar(hay, 1);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hay"))
            inBarn = false;
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
