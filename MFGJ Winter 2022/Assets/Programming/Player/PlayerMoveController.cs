using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D RB;
    [SerializeField]private float moveSpeed = 5f;
    Vector2 moveInput;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        AliveCheck.changeAliveState(true);
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("X", moveInput.x);
        anim.SetFloat("Y", moveInput.y);
        moveInput = moveInput.normalized;
        
        
    }

    private void FixedUpdate()
    {
        moveInput *= moveSpeed * Time.fixedDeltaTime;
        
        RB.MovePosition((Vector2)transform.position + moveInput);
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crow"))
        {
            collision.GetComponent<CrowController>().ScareCrow(transform.position);
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
