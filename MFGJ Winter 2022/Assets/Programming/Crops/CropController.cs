using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        _health = health;
        alive = true;
        SR = gameObject.GetComponentInChildren<SpriteRenderer>();
        SR.sortingOrder = Mathf.CeilToInt(transform.position.y * 100) * -1;
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
        
        if (_health < 0)
        {
            alive = false;
            
            killed?.Invoke();
            EnemySpawnController.s.removePlant(this);
        }
        
    }

    void updateHealth()
    {
        //SR.color = healthcolors.Evaluate(_health / health);
        float currentPercentage = (_health/health);
        
        if(index < lifeSprites.Length + 1)
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