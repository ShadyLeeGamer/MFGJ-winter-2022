using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropController : MonoBehaviour
{
    [SerializeField] float health;
    public bool alive = true;
    float _health;

    public delegate void killCrop();

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

    public void attackCrop(float damage)
    {
        _health -= damage * Time.deltaTime;
        
        SR.color = healthcolors.Evaluate(_health / health);
        if(_health < 0)
        {
            alive = false;
            killed?.Invoke();
            EnemySpawnController.s.removePlant(this);
        }
        
    }
}
