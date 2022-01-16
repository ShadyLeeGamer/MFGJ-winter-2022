using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropController : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float regenPSec;
    [SerializeField] float AutoHealDelay;
    public bool alive = true;
    float _health;
    float healthStore;
    bool attacked;
    public delegate void killCrop();
    float timer;
    public event killCrop killed;
    [SerializeField] Gradient healthcolors;
    SpriteRenderer SR;

    // Start is called before the first frame update
    void Start()
    {
        _health = health;
        alive = true;
        SR = gameObject.GetComponentInChildren<SpriteRenderer>();
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
                    SR.color = healthcolors.Evaluate(_health / health);
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
        SR.color = healthcolors.Evaluate(_health / health);
        if(_health < 0)
        {
            alive = false;
            
            killed?.Invoke();
            EnemySpawnController.s.removePlant(this);
        }
        
    }
}
