using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum crowState {moving, attacking, scared }
public enum AttackType {ground, air}
public class CrowController : MonoBehaviour
{
    private crowState state;
    private CropController target;
    private Transform targetPosition;
    private Vector3 movePosition;
    private EnemySpawnController currentController;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fleespeed = 10f;
    [SerializeField] private float power = 1;
    [SerializeField] private int braveness;
    [SerializeField] private AnimationCurve fleeTime;
    [SerializeField] private AttackType attackType;
    private int _braveness;
    private Animator anim;
    Vector2 targetDir;
    // Start is called before the first frame update
    void Start()
    {
        state = crowState.moving;
        targetPosition = target.transform;
        _braveness = braveness;
        if(attackType == AttackType.air)
        {
            anim = gameObject.GetComponentInChildren<Animator>();
        } 
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

    public void startCrow(CropController newtarget, EnemySpawnController controller)
    {
        target = newtarget;
        target.killed += killedPlant;
        currentController = controller;
    }

    void GetNewTarget(CropController newtarget)
    {
        target = newtarget;
        if (target != null)
        {
           
            target.killed += killedPlant;
            targetPosition = target.transform;
            state = crowState.moving;
        }
        else
        {
            _braveness = 0;
        }
        
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
        GetNewTarget(currentController.getNewTarget());
        ScareCrow(targetPosition.position);
    }
    
    void scaredCrow()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, movePosition, fleespeed * Time.deltaTime);
        if(_braveness <= 0)
        {
            if (transform.position == movePosition)
            {
                currentController.RemoveEnemy();

                Destroy(gameObject);
            }
        }
        
    }

    public void ScareCrow(Vector3 playerPosition)
    {
        var runawaydirection = playerPosition - transform.position;
        runawaydirection *= -1;
       
        state = crowState.scared;

        target.killed -= killedPlant;
        _braveness--;
        if(_braveness > 0)
        {
            StartCoroutine(fleeForTime());
        }
        
        movePosition = transform.position + (runawaydirection.normalized * 20);
    }

    private IEnumerator fleeForTime()
    {
        float time = fleeTime.Evaluate(braveness);
        yield return new WaitForSeconds(time);
        GetNewTarget(currentController.getNewTarget());
        

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
