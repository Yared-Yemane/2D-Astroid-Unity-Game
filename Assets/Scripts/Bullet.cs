using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float m_speed;
    [SerializeField] private float maximumLifeTime;

    [SerializeField] private GameObject m_bulletHitVFX;
    [SerializeField] private float currentLifeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * m_speed * Time.deltaTime;
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maximumLifeTime)
        {
            // SpawnBulletHitVFX();
            Destroy(gameObject);
        }    

    }

    private void SpawnBulletHitVFX(){
        GameObject hit = GameObject.Instantiate(m_bulletHitVFX);
        hit.transform.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<AstroidController>() != null){
            SpawnBulletHitVFX();
            Destroy(gameObject);
        }
    }
}
