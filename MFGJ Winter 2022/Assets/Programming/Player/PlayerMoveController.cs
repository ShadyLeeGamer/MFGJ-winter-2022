using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode startWaveKey = KeyCode.Z;
    private Rigidbody2D RB;
    [SerializeField] private float walkSpeed = 5f, sprintSpeed;
    Vector2 moveInput;
    bool isSprinting;
    [SerializeField] float hayBarnInc, hayDec, haySprintDec;
    float hay = 1;
    float hayGainParticleEmmissionRate;
    bool pressedNextWave;
    private Animator anim;
    private SpriteRenderer SR;

    bool inBarn;
    [SerializeField] ParticleSystem hayFallParticle, hayGainParticle;
    [SerializeField] float hayFallParticleRateNormal, hayFallParticleRateSprinting;
    [SerializeField] float hayGainParticleRateNormal;

    GameUI gameUI;

    [SerializeField] Color haylessColour;
    [SerializeField] GameObject scareCircle;

    public static PlayerMoveController Instance { get; private set; }

    PersistentData persistentData;

    Joystick joystick;
    public bool sprintMobileButtonPress, interactMobileButtonPress;

    public bool SprintInput => Input.GetKey(sprintKey) || sprintMobileButtonPress;
    public bool InteractInput => Input.GetKeyDown(startWaveKey) || interactMobileButtonPress;

    bool canRingFarmBell, canBuy;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        AliveCheck.changeAliveState(true);
        anim = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();

        gameUI = GameUI.Instance;
        hayGainParticleEmmissionRate = hayGainParticle.emission.rateOverTime.constant;

        persistentData = PersistentData.Instance;

        joystick = Joystick.Instance;
    }

    void Update()
    {
        if (AliveCheck.alive)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            if (joystick)
            {
                moveInput.x += joystick.Horizontal;
                moveInput.y += joystick.Vertical;
                Mathf.Clamp01(moveInput.x);
                Mathf.Clamp01(moveInput.y);
            }
        }

        anim.SetFloat("X", moveInput.x);
        anim.SetFloat("Y", moveInput.y);
        moveInput = moveInput.normalized;

        if (SprintInput)
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
        if (InteractInput)
        {
            pressedNextWave = true;

        }
        else
        {
            pressedNextWave = false;
        }
        Mathf.Clamp01(hay);
        gameUI.SetPlayerStaminaBar(hay, 1);

        UpdateHayFallParticle();
        UpdateHayGainParticle();

        UpdateColour();
    }

    void LateUpdate()
    {
        interactMobileButtonPress = false;
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
        particleEmission.rateOverTime = inBarn && hay < 1 && !isSprinting ? hayGainParticleRateNormal : 0;
    }

    void UpdateColour()
    {
        SR.color = Color.Lerp(haylessColour, Color.white, hay);

        scareCircle.SetActive(hay > 0);
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
        if (collision.CompareTag("Hay"))
        {
            if (hay < 1 && !isSprinting)
            {
                inBarn = true;
                hay += Time.deltaTime * hayBarnInc;
                Mathf.Clamp01(hay);
                gameUI.SetPlayerStaminaBar(hay, 1);
            }
        }
        if (collision.GetComponent<FarmBell>() && pressedNextWave)
            collision.GetComponent<FarmBell>().Ring();
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

    public void SprintMobileButtonHold()
    {
        sprintMobileButtonPress = true;
    }

    public void SprintMobileButtonRelease()
    {
        sprintMobileButtonPress = false;
    }

    public void InteractMobileButtonTap()
    {
        interactMobileButtonPress = true;
    }

    public void InteractMobileButtonRelease()
    {
        interactMobileButtonPress = false;
    }
}


public static class AliveCheck
{
    public static bool alive { private set; get; } = true;
    public delegate void deathVoid();
    public static event deathVoid DeathTrigger;


    

    public static void changeAliveState(bool state)
    {
        alive = state;
        if (!alive)
        {
            DeathTrigger?.Invoke();
        }
    }
}
