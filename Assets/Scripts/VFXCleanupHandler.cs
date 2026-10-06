using UnityEngine;

public class VFXCleanupHandler : MonoBehaviour
{

    private float m_currentTime;
    [SerializeField] private float m_lifetime;

    // Update is called once per frame
    void Update()
    {   
        if(m_currentTime >= m_lifetime){
            GameObject.Destroy(gameObject);
        }

        m_currentTime += Time.deltaTime;
    }
}
