using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private int m_pickupScore;

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.TryGetComponent(out ShipController shipController)){
            GameEvents.Instance.AddToScore(m_pickupScore, true);
            Destroy(gameObject);
        }
    }
}
