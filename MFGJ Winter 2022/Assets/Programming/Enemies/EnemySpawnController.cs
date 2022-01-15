using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject spawn;
    private GameObject[] cropObjects;
    
    private List<CropController> activeCrops = new List<CropController>();
    [SerializeField] private float spawnsPSec;
    float timer;

    Vector2 borders;

    // Start is called before the first frame update
    void Start()
    {
        cropObjects = GameObject.FindGameObjectsWithTag("Crop");
        foreach(var crop in cropObjects)
        {
            activeCrops.Add(crop.GetComponent<CropController>());
        }
        CalculateBordes();
    }


    void CalculateBordes()
    {
        borders = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width + 0.5f, Screen.height + 0.5f));
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            SpawnBird(spawn);
            timer = 1 / spawnsPSec;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }


    void SpawnBird(GameObject objectToSpawn)
    {
        var spawnPisition = CalculateSpawnPosition();
        int target = Random.Range(0, activeCrops.Count);
        var bird = Instantiate(objectToSpawn, spawnPisition, Quaternion.Euler(Vector3.zero));
        bird.GetComponent<CrowController>().startCrow(activeCrops[target]);
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
        Debug.Log(position);
        return position;
    }
}
