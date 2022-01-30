using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject Cow;
    [SerializeField] private GameObject birdFast;
    
    private GameObject[] cropObjects;
    private int enemiesAlive;
    private int totalEnemies;

    private List<CropController> activeCrops = new List<CropController>();
    private List<CropController> deadCrops = new List<CropController>();
    [SerializeField] private float spawnsPSec;
    float timer;

    public bool inWave;
    public bool waveOver;

    public int wave { get; private set;}
    public int birdSpawnsThisWave, cowSpawnsThisWave, birdSpeedThisWave;

    [Header("waveBalancing")]
    [SerializeField] AnimationCurve birdsAmountScaling;
    [SerializeField] AnimationCurve cowAmountScaling;
    //[SerializeField] AnimationCurve birdsFastAmountScaling;
    [SerializeField] AnimationCurve spawnRateScaling;

    [SerializeField] Vector2 borders;
    Vector2 _borders;
    [SerializeField]
    AudioClip gameTrack, gameOverTrack;
    AudioStation audioStation;

    GameUI gameUI;

    [SerializeField] FarmBell farmBell;

    #region singleton
    public static EnemySpawnController s;
    private void Awake()
    {
        s = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cropObjects = GameObject.FindGameObjectsWithTag("Crop");
        foreach(var crop in cropObjects)
        {
            activeCrops.Add(crop.GetComponent<CropController>());
        }
        wave++;
        inWave = true;
        waveOver = false;
        birdSpawnsThisWave = Mathf.FloorToInt(birdsAmountScaling.Evaluate(wave));
        //cowSpawnsThisWave = Mathf.FloorToInt(cowAmountScaling.Evaluate(wave));
        
        totalEnemies = birdSpawnsThisWave + cowSpawnsThisWave;
        enemiesAlive = totalEnemies;
        audioStation = AudioStation.Instance;
        audioStation.StartNewMusicPlayer(gameTrack, true);
        _borders = borders / 2;
        gameUI = GameUI.Instance;
        setUI();
    }

    void setUI()
    {
        gameUI.SetCropsRemainingBar(activeCrops.Count, cropObjects.Length);
        gameUI.SetCropEatersRemainingBar(enemiesAlive, totalEnemies);
        gameUI.SetCurrentWaveDisplay(wave);
    }    

    // Update is called once per frame
    void Update()
    {
        if (AliveCheck.alive && inWave && !waveOver)
        {
            if (timer <= 0)
            {
                spawnEnemy();
                timer = 1 / spawnsPSec;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        
    }

    void spawnEnemy()
    {
        int i = Random.Range(0, 2);

        switch (i)
        {
            case 1:
                SpawnBird();
                break;
            case 2:
                SpawnCow();
                break;
            /*case 3:
                SpawnBirdSpeed();
                break;*/
        }
    }

    void SpawnBird()
    {
        if (birdSpawnsThisWave > 0)
        {
            birdSpawnsThisWave--;
            SpawnBird(bird);
        }
        else
        {
            if (cowSpawnsThisWave > 0)
            {
                cowSpawnsThisWave--;
                SpawnBird(Cow);
            }
            else
            {
                if (birdSpeedThisWave > 0)
                {
                    birdSpeedThisWave--;
                    SpawnBird(birdFast);
                }
                else
                {
                    NextWave();
                }
            }
        }
    }

    void SpawnBirdSpeed()
    {
        if (birdSpeedThisWave > 0)
        {
            birdSpeedThisWave--;
            SpawnBird(birdFast);
        }
        else
        {
            if (cowSpawnsThisWave > 0)
            {
                cowSpawnsThisWave--;
                SpawnBird(Cow);
            }
            else
            {
                if (birdSpawnsThisWave > 0)
                {
                    birdSpawnsThisWave--;
                    SpawnBird(bird);
                }
                else
                {
                    NextWave();
                }
            }
        }
    }

    void SpawnCow()
    {
        if (cowSpawnsThisWave > 0)
        {
            cowSpawnsThisWave--;
            SpawnBird(Cow);
        }
        else
        {
            if (birdSpawnsThisWave > 0)
            {
                birdSpawnsThisWave--;
                SpawnBird(bird);
            }
            else
            {
                if (birdSpeedThisWave > 0)
                {
                    birdSpeedThisWave--;
                    SpawnBird(birdFast);
                }
                else
                {
                    NextWave();
                }
            }
        }
    }



    public void NextWave()
    {
        inWave = false;
        resetPlants();
        spawnsPSec = spawnRateScaling.Evaluate(wave);
        StartCoroutine(waveTimer());
    }


    void resetPlants()
    {
        foreach (var crop in activeCrops)
        {
            crop.ClearList();
        }
    }


    private IEnumerator waveTimer()
    {
        while(enemiesAlive > 0 || waveOver)
        {
            yield return null;
        }
        //yield return new WaitForSeconds(5);
        if (AliveCheck.alive)
        {
            print(wave);
            wave++;
            inWave = true;
            birdSpawnsThisWave = Mathf.FloorToInt(birdsAmountScaling.Evaluate(wave));
            cowSpawnsThisWave = Mathf.FloorToInt(cowAmountScaling.Evaluate(wave));
           // birdSpeedThisWave = Mathf.FloorToInt(birdsFastAmountScaling.Evaluate(wave));
            totalEnemies = birdSpawnsThisWave + cowSpawnsThisWave + birdSpeedThisWave;
            enemiesAlive = totalEnemies;
            setUI();
        }
    }

    void AddCoins()
    {
        int amount = activeCrops.Count;
        ShopCurrencyController.instance.AddCoins(amount);
    }

    public void removePlant(CropController target)
    {
        activeCrops.Remove(target);
        deadCrops.Add(target);
        setUI();

        if (activeCrops.Count == 0)
        {
            AliveCheck.changeAliveState(false);

            audioStation.StartNewMusicPlayer(gameOverTrack, false);
        }
    }

    public bool CheckForDeadPlants()
    {
        if(deadCrops.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RevivePlant()
    {
        int target = Random.Range(0, deadCrops.Count);
        var plant = deadCrops[target];
        Debug.Log(plant);
        deadCrops.Remove(plant);
        activeCrops.Add(plant);
        plant.revivePlant();
        setUI();
    }

    void SpawnBird(GameObject objectToSpawn)
    {
        var spawnPisition = CalculateSpawnPosition();
        int target = Random.Range(0, activeCrops.Count);
        var bird = Instantiate(objectToSpawn, spawnPisition, Quaternion.Euler(Vector3.zero));
        bird.GetComponent<CrowController>().startCrow(activeCrops[target], this);
    }


    public void RemoveEnemy()
    {
        enemiesAlive--;
        setUI();

        if (enemiesAlive <= 0)
            WaveOver();
    }

    void WaveOver()
    {
        waveOver = true;
        AddCoins();
        farmBell.SetTextActive(true);
    }

    public CropController getNewTarget()
    {
        int target = Random.Range(0, activeCrops.Count);
        if(activeCrops.Count > 0)
        {
            return activeCrops[target];
        }
        return null;
        
    }

    Vector3 CalculateSpawnPosition()
    {
        Vector3 position;
        
        
        position = Random.insideUnitCircle * _borders;

        int x = 1;
        if(position.x < 0)
        {
            x = -1;
        }
        int y = 1;
        if(position.y < 0)
        {
            y = -1;
        }

        position += new Vector3(_borders.x * x, _borders.y * y);
        
        return position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, borders);
    }
}