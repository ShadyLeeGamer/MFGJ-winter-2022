using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropController : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float regenPSec;
    [SerializeField] float AutoHealDelay;
    [SerializeField] LifeSpites[] lifeSprites; 
    public bool alive = true;
    float _health;
    float healthStore;
    bool attacked;
    public delegate void killCrop();
    float timer;
    public event killCrop killed;
    [SerializeField] Gradient healthcolors;
    SpriteRenderer SR;
    [SerializeField] Image image;
    int index = 0;

    Canvas UI;
    [SerializeField] GameObject warningSign;
    [SerializeField] RectTransform currentWarningSign;
    GameObject player;


    Vector2 borders;

    // Start is called before the first frame update
    void Start()
    {
        _health = health;
        alive = true;
        SR = gameObject.GetComponentInChildren<SpriteRenderer>();
        SR.sortingOrder = Mathf.CeilToInt(transform.position.y * 100) * -1;
        UI = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();
        CalculateBordes();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void CalculateBordes()
    {
        var rect = UI.GetComponent<RectTransform>();
        borders = rect.sizeDelta / 2;
        
    }

    public void revivePlant()
    {
        _health = health;
        index = 0;
        updateHealth();
        alive = true;
    }


    public void Update()
    {
        if (alive)
        {
            if (healthStore > _health)
            {
                healthStore = _health;
                timer = AutoHealDelay;
            }
            else
            {
                if (attacked)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        attacked = false;
                    }
                }
                else
                {
                    _health += regenPSec * Time.deltaTime;
                    updateHealth();
                    if (_health > health)
                    {
                        _health = health;
                    }
                }
                healthStore = _health;

            }
        }
        
    }

    public void attackCrop(float damage)
    {
        _health -= damage * Time.deltaTime;
        attacked = true;
        updateHealth();


        if (!SR.isVisible)
        {
            if(currentWarningSign != null)
            {
                
                SetWarningPosition();
            }
            else
            {
                currentWarningSign = Instantiate(warningSign, Vector3.zero, Quaternion.Euler(Vector3.zero), UI.transform).GetComponent<RectTransform>();
                SetWarningPosition();
            }
        }
        else
        {
            if (currentWarningSign != null)
            {

                Destroy(currentWarningSign.gameObject);
                currentWarningSign = null;
            }
        }

        if (_health < 0)
        {
            alive = false;
            if (currentWarningSign != null)
            {

                Destroy(currentWarningSign.gameObject);
                currentWarningSign = null;
            }
            killed?.Invoke();
            EnemySpawnController.s.removePlant(this);
        }
        
    }


    void SetWarningPosition()
    {
        CalculateBordes();
        Debug.DrawLine(transform.position, player.transform.position, Color.red, 0.5f);
        var direction = (transform.position - player.transform.position).normalized;
        
        var offset = currentWarningSign.sizeDelta / 2;
        offset *= -1;
        offset += borders;

        

        var position = offset * direction;
        
        currentWarningSign.anchoredPosition = position;
    }


    void updateHealth()
    {
        float currentPercentage = (_health/health);
        
        image.fillAmount = currentPercentage;
        image.color = healthcolors.Evaluate(currentPercentage);

        if (index < lifeSprites.Length + 1)
        {
            if ((lifeSprites[index + 1].percentage) > currentPercentage)
            {
                index++;
                if (index >= lifeSprites.Length)
                {
                    index--;
                }
            }
            else
            {
                if (index > 0)
                {
                    if (lifeSprites[index - 1].percentage < currentPercentage)
                    {
                        index--;
                    }
                }

            }
        }
        
        SR.sprite = lifeSprites[index].sprite;
    }
}


[System.Serializable]
public class LifeSpites
{
    public Sprite sprite;
    public float percentage;
}