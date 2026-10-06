using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BoundedEntity : MonoBehaviour
{

    protected SpriteRenderer m_spriteRenderer;
    protected Collider2D m_collider;
    protected Rigidbody2D m_rigidbody;
    [SerializeField] private Rect bounds;
    [SerializeField] protected int m_currentHealth;
    [SerializeField] protected int m_maxHealth = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    protected void SpawnVFX(GameObject vfxPrefab){
        GameObject vfx = GameObject.Instantiate(vfxPrefab);
        vfx.transform.position = transform.position;
    }

    void Awake(){
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_collider = gameObject.GetComponent<Collider2D>();
        m_rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void LateUpdate()
    {
        Vector2 position = m_rigidbody != null ? m_rigidbody.position : (Vector2)transform.position;
        if(bounds.Contains(position)){
            return;
        }

        if (position.x < bounds.xMin){
            position.x = bounds.xMax;
        }
        if (position.x > bounds.xMax){
            position.x = bounds.xMin;
        }
        if(position.y < bounds.yMin){
            position.y = bounds.yMax;
        }
        if(position.y > bounds.yMax){
            position.y = bounds.yMin;
        }

        // Keep the Rigidbody2D in sync — setting transform.position alone
        // can desync physics and miss trigger overlaps.
        if (m_rigidbody != null)
        {
            m_rigidbody.position = position;
        }
        else
        {
            transform.position = position;
        }
    }

    protected virtual void OnEnable(){
        ResetHealth();
    }

    protected virtual void OnDisable(){
        // GameEvents.Instance.onRetry -= OnRetry;
    }

    protected void LoseHealth(){
        m_currentHealth--;

        if(m_currentHealth <= 0){
            OnDie();
        }
    }

    protected void ResetHealth(){
        m_currentHealth = m_maxHealth;
    }

    protected virtual void OnDie(){

    }
}
