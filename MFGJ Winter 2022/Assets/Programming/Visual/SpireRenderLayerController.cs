using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpireRenderLayerController : MonoBehaviour
{
    private SpriteRenderer SR;
    [SerializeField] bool moves;
    [SerializeField] Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        
        if(!TryGetComponent<SpriteRenderer>(out SR))
        {
            SR = GetComponentInChildren<SpriteRenderer>();
        }

        SR.sortingOrder = Mathf.CeilToInt((transform.position.y + offset.y) * 100) * -1;
            
    }

    // Update is called once per frame
    void Update()
    {
        if (moves)
        {
            SR.sortingOrder = Mathf.CeilToInt((transform.position.y + offset.y) * 100) * -1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)offset, 0.5f);
    }
}
