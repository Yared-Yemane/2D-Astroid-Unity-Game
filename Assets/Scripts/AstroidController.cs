using System;
using UnityEngine;

public class AstroidController : BoundedEntity
{

    [SerializeField] private float m_forcePower;
    [SerializeField] private float m_constantForcePower;
    [SerializeField] private GameObject m_deathEffect;
    [SerializeField] private int m_size;
    [SerializeField] private float m_angularPower;
    public event Action<AstroidController> OnAstroidDestroyed;
    public int Size => m_size;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rigidbody.AddForce(UnityEngine.Random.insideUnitCircle * m_rigidbody.mass * m_forcePower, ForceMode2D.Impulse);
        m_rigidbody.angularVelocity = UnityEngine.Random.Range(-m_angularPower, m_angularPower);   
    }

    // private void EnableDeathEffect(){

    // }

    private void FixedUpdate(){
        Vector3 direction = transform.up;
        m_rigidbody.AddForce((direction * m_rigidbody.mass * m_constantForcePower)*Time.deltaTime, ForceMode2D.Impulse);
        // m_rigidbody.angularVelocity = UnityEngine.Random.Range(-m_angularPower, m_angularPower);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bullet>() != null){
            LoseHealth();
        }
    }

    protected override void OnDie()
    {
        SpawnVFX(m_deathEffect);
        OnAstroidDestroyed?.Invoke(this);
    }
}
