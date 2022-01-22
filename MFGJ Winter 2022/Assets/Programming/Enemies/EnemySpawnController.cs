using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject Cow;
    [SerializeField] private int birdsPerWave = 5;
    [SerializeField] private int cowsPerWave = 1;
    private GameObject[] cropObjects;
    private int enemiesAlive;
    

    private List<CropController> activeCrops = new List<CropController>();
    [SerializeField] private float spawnsPSec;
    float timer;

    bool inWave;

    int wave;
    int birdSpawnsThisWave, cowSpawnsThisWave;

    [Header("waveBalancing")]
    [SerializeField] AnimationCurve birdsAmountScaling;
    [SerializeField] AnimationCurve cowAmountScaling;
    [SerializeField] AnimationCurve spawnRateScaling;

    Vector2 borders;

    [SerializeField]
    AudioClip gameTrack, gameOverTrack;
    public AudioStation audioStation;

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
        CalculateBordes();
        inWave = true;
        birdSpawnsThisWave = birdsPerWave;
        cowSpawnsThisWave = cowsPerWave;
        wave++;

        audioStation = AudioStation.Instance;
        audioStation.StartNewMusicPlayer(gameTrack, true);
    }


    void CalculateBordes()
    {
        borders = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width + 0.5f, Screen.height + 0.5f));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (AliveCheck.TestForAlive() && inWave)
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
        if(i == 1)
        {
            if (birdSpawnsThisWave > 0)
            {
                birdSpawnsThisWave--;
                SpawnBird(bird);
            }
            else
            {
                if(cowSpawnsThisWave > 0)
                {
                    cowSpawnsThisWave--;
                    SpawnBird(Cow);
                }
                else
                {
                    NextWave();
                }
            }
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


    void NextWave()
    {
        inWave = false;
        
        birdSpawnsThisWave = Mathf.FloorToInt(birdsAmountScaling.Evaluate(wave));
        cowSpawnsThisWave = Mathf.FloorToInt(cowAmountScaling.Evaluate(wave));
        spawnsPSec = spawnRateScaling.Evaluate(wave);
        StartCoroutine(waveTimer());
    }

    private IEnumerator waveTimer()
    {
        while(enemiesAlive > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5);
        if (AliveCheck.TestForAlive())
        {
            wave++;
            inWave = true;
            Debug.Log("next wave");
            Debug.Log("wave " + wave);
        }
        
    }

    public void removePlant(CropController target)
    {
        activeCrops.Remove(target);
        if(activeCrops.Count == 0)
        {
            Debug.Log("lost");
            AliveCheck.changeAliveState(false);

            audioStation.StartNewMusicPlayer(gameOverTrack, false);
        }
    }

    void SpawnBird(GameObject objectToSpawn)
    {
        enemiesAlive++;
        var spawnPisition = CalculateSpawnPosition();
        int target = Random.Range(0, activeCrops.Count);
        var bird = Instantiate(objectToSpawn, spawnPisition, Quaternion.Euler(Vector3.zero));
        bird.GetComponent<CrowController>().startCrow(activeCrops[target], this);
    }


    public void RemoveEnemy()
    {
        enemiesAlive--;
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
        
        
        position = Random.insideUnitCircle * borders;

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

        position += new Vector3(borders.x * x, borders.y * y);
        
        return position;
    }
}
