using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D RB;
    [SerializeField]private float moveSpeed = 5f;
    Vector2 moveInput;
    Vector2 screenBorders;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
        moveInput = moveInput.normalized;
        
        
    }

    private void FixedUpdate()
    {
        moveInput *= moveSpeed * Time.fixedDeltaTime;
        
        RB.MovePosition((Vector2)transform.position + moveInput);
        
    }

    
}
