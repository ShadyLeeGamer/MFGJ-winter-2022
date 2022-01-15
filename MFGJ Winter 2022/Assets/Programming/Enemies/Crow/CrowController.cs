using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum crowState {moving, attacking, scared }
public class CrowController : MonoBehaviour
{
    private crowState state;
    private CropController target;
    private Transform targetPosition;
    private Vector3 movePosition;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fleespeed = 10f;
    [SerializeField] private float power = 1;
    // Start is called before the first frame update
    void Start()
    {
        state = crowState.moving;
        targetPosition = target.transform;
    }




    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case crowState.moving:
                moveCrow();
                break;
            case crowState.attacking:
                attackCrop();
                break;
            case crowState.scared:
                scaredCrow();
                break;
        }
    }

    public void startCrow(CropController newtarget)
    {
        target = newtarget;
        target.killed += killedPlant;
    }

    void moveCrow()
    {
        movePosition = targetPosition.position;
        transform.position = Vector3.MoveTowards(transform.position, movePosition, moveSpeed * Time.deltaTime);
    }

    void attackCrop()
    {
        target.attackCrop(power);
        
    }

    void killedPlant()
    {
        ScareCrow(targetPosition.position);
    }
    
    void scaredCrow()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, movePosition, fleespeed * Time.deltaTime);
        if(transform.position == movePosition)
        {
            
            
            Destroy(gameObject);
        }
    }

    public void ScareCrow(Vector3 playerPosition)
    {
        var runawaydirection = playerPosition - transform.position;
        runawaydirection *= -1;
        state = crowState.scared;

        target.killed -= killedPlant;
        
        movePosition = transform.position + runawaydirection * 500;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crop"))
        {
            if(collision.GetComponent<CropController>() == target)
            {
                state = crowState.attacking;
            }
        }
    }
}
