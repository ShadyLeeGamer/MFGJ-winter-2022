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

    private float moveSpeed = 5f;
    private float fleespeed = 10f;
    private float power = 1;
    private int braveness;
    
    [SerializeField] private AttackType attackType;
    [SerializeField] private LayerMask layers;
    private int _braveness;
    private Animator anim;
    Vector2 targetDir;
    
    [Header("BalancingCurves")]
    [SerializeField] private AnimationCurve fleeTime;
    [SerializeField] private AnimationCurve scaretimeOverBraveness, bravenessOverWave, speedOverWave, powerOverWave;
    [SerializeField] float fleeSpeedMultiplier;

    private List<Vector3> obstacleAvoidanceList = new List<Vector3>();
    bool obstacleAvoidance;
    
    
    float scareTime;
    [Header("Audio")]
    [SerializeField] AudioClip[] spawnSFX;
    [SerializeField] AudioClip[]  scaredSFX;
    AudioStation audioStation;


    // Start is called before the first frame update
    void Start()
    {
        state = crowState.moving;
        targetPosition = target.transform;
        _braveness = braveness;
        scareTime = scaretimeOverBraveness.Evaluate(braveness);
        anim = gameObject.GetComponentInChildren<Animator>();

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
        var wave = controller.wave;

        moveSpeed = speedOverWave.Evaluate(wave);
        braveness = Mathf.FloorToInt(bravenessOverWave.Evaluate(wave));
        power = powerOverWave.Evaluate(wave);
        fleespeed = moveSpeed * fleeSpeedMultiplier;
        audioStation = AudioStation.Instance;
        audioStation.StartNewRandomSFXPlayer(spawnSFX, default, null, 0.8f, 1.2f, true);
        
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
        moveEnemy(movePosition, moveSpeed);
        
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


        moveEnemy(movePosition, fleespeed);
        
        

        transform.position = Vector3.MoveTowards(transform.position, movePosition, fleespeed * Time.deltaTime);
        updateAnimator((movePosition - transform.position).normalized, false);

        if (_braveness <= 0)
        {
            if (transform.position == movePosition || !GetComponentInChildren<Renderer>().isVisible) 
            {
                currentController.RemoveEnemy();

                Destroy(gameObject);
            }
        }
        
    }


    void moveEnemy(Vector3 movetarget, float speed)
    {
        if(attackType == AttackType.ground)
        {
            if (!obstacleAvoidance)
            {
                Vector2 movedirection = (movetarget - transform.position).normalized;


                RaycastHit2D hit = Physics2D.Raycast(transform.position, movedirection, 1, layers);

                if (hit.collider != null)
                {
                    //set values
                    obstacleAvoidance = true;
                    var col = hit.collider.GetComponent<BoxCollider2D>();
                    var angle = (hit.point - (Vector2)transform.position).normalized;
                    var centre = (Vector2)hit.collider.transform.position - col.offset;
                    var offset = 0.5f;
                    Vector2[] corners = new Vector2[4];

                    //tl
                    corners[0] = new Vector2(centre.x + ((col.size.x / 2 + offset) * -1), centre.y + (col.size.y / 2) + offset);
                    //tr
                    corners[1] = new Vector2(centre.x + (col.size.x / 2) + offset, centre.y + (col.size.y / 2) + offset);
                    //bl
                    corners[3] = new Vector2(centre.x + ((col.size.x / 2 + offset) * -1), centre.y + ((col.size.y / 2 + offset) * -1));
                    //br
                    corners[2] = new Vector2(centre.x + (col.size.x / 2) + offset, centre.y + ((col.size.y / 2 + offset) * -1));

                    float distance = 10000;
                    int playerIndex = 0;
                    int finalIndex = 0;
                    for (int i = 0; i < corners.Length; i++)
                    {
                        var _distance = Vector2.Distance(corners[i], transform.position);
                        if (_distance < distance)
                        {
                            distance = _distance;
                            playerIndex = i;
                        }
                    }
                    distance = 1000000;
                    for (int i = 0; i < corners.Length; i++)
                    {
                        var _distance = Vector2.Distance(corners[i], movetarget);
                        if (_distance < distance)
                        {
                            distance = _distance;
                            finalIndex = i;
                        }
                    }
                    if(playerIndex == finalIndex)
                    {
                        obstacleAvoidanceList.Add(corners[playerIndex]);
                        movetarget = corners[playerIndex];
                    }
                    else
                    {
                        switch(Mathf.Abs(playerIndex - finalIndex))
                        {
                            case 1:
                                obstacleAvoidanceList.Add(corners[playerIndex]);
                                obstacleAvoidanceList.Add(corners[finalIndex]);
                                break;
                            case 2:
                                if ((playerIndex - finalIndex) > 0)
                                {
                                    obstacleAvoidanceList.Add(corners[playerIndex]);
                                    playerIndex++;
                                    if (playerIndex >= corners.Length)
                                        playerIndex = 0;
                                    obstacleAvoidanceList.Add(corners[playerIndex]);
                                }
                                else
                                {
                                    obstacleAvoidanceList.Add(corners[playerIndex]);
                                    obstacleAvoidanceList.Add(corners[finalIndex]);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                if(transform.position == obstacleAvoidanceList[0])
                {
                    obstacleAvoidanceList.RemoveAt(0);
                    if(obstacleAvoidanceList.Count <= 0)
                    {
                        obstacleAvoidance = false;
                    }
                    else
                    {
                        movetarget = obstacleAvoidanceList[0];
                    }
                }
                else
                {
                    movetarget = obstacleAvoidanceList[0];
                }
            }
            
        }
        transform.position = Vector3.MoveTowards(transform.position, movetarget, speed * Time.deltaTime);
        updateAnimator((movePosition - transform.position).normalized, false);
    }


    public void ScareCrow(Vector3 playerPosition)
    {
        
        var runawaydirection = playerPosition - transform.position;
        runawaydirection *= -1;
        if (state != crowState.scared)
        {
            if (attackType == AttackType.ground)
            {
                scareTime -= Time.fixedDeltaTime;
                if(scareTime <= 0)
                {
                    audioStation.StartNewRandomSFXPlayer(scaredSFX, transform.position, transform, 0.8f, 1.2f);

                    state = crowState.scared;

                    target.killed -= killedPlant;
                    _braveness--;
                    if (_braveness > 0)
                    {
                        StartCoroutine(fleeForTime());
                    }
                    movePosition = transform.position + (runawaydirection.normalized * 20);
                }
            }
            else
            {
                
                
                audioStation.StartNewRandomSFXPlayer(scaredSFX, transform.position, transform, 0.8f, 1.2f);

                state = crowState.scared;

                target.killed -= killedPlant;
                _braveness--;
                if (_braveness > 0)
                {
                    StartCoroutine(fleeForTime());
                }
                movePosition = transform.position + (runawaydirection.normalized * 20);


            }
        }
            
        
    }

    private IEnumerator fleeForTime()
    {
        float time = fleeTime.Evaluate(braveness);
        yield return new WaitForSeconds(time);
        GetNewTarget(currentController.getNewTarget());
        scareTime = scaretimeOverBraveness.Evaluate(braveness);

    }


    private void updateAnimator(Vector3 angle, bool attacking)
    {
        
            anim.SetBool("Attacking", attacking);

            var x = Mathf.Abs(angle.x);
            var y = Mathf.Abs(angle.y);


            if (attacking)
            {
                anim.SetFloat("X", angle.x);
                anim.SetFloat("Y", 0f);
            }
            else
            {
                if (x >= y)
                {
                    anim.SetFloat("X", angle.x);
                    anim.SetFloat("Y", 0f);
                }
                else
                {
                    if (x < y)
                    {
                        anim.SetFloat("Y", angle.y);
                        anim.SetFloat("X", 0f);
                    }
                }

            
        }        

    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crop"))
        {
            if(collision.GetComponent<CropController>() == target)
            {
                state = crowState.attacking;
                updateAnimator((movePosition - transform.position).normalized, true);
            }
        }
    }
    
}
