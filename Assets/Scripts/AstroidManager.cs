using UnityEngine;
using System.Collections.Generic;  
using System.Collections;
using System.Linq;
using System.Numerics;
public class AstroidManager : MonoBehaviour
{
    [SerializeField] private int m_maximumAstroids;
    [SerializeField] private List<GameObject> m_astroidPrefabs;

    [SerializeField] private GameObject m_collectiblePrefabs;
    [SerializeField] private int []m_numOfAstroidsToSpawn;
    [SerializeField] private int m_currentAstroidCount;
    
    [SerializeField] private float m_SpawnDelay;

    [SerializeField] private Rect m_spawnArea;

    private void SpawnCollectible(UnityEngine.Vector3 position){
        if (Random.Range(0f,1f) > 0.5f){
            GameObject collectible = Instantiate(m_collectiblePrefabs, transform);
            collectible.transform.position = position;
        }
    }

    private void Start()
    {
        OnRetry();
    }

    private void OnEnable(){
        GameEvents.Instance.onRetry += OnRetry;
        GameEvents.Instance.onGameOver += OnGameOver;
    }

    private void OnDisable(){
        GameEvents.Instance.onRetry -= OnRetry;
        GameEvents.Instance.onGameOver -= OnGameOver;
    }

    private void OnRetry(){
        StopAllCoroutines();
        foreach(Transform t in transform){
            Destroy(t.gameObject);
        }
        m_currentAstroidCount = 0;
        StartCoroutine(SpawnInitialAstroids());
    }

    private void OnGameOver(){
        StopAllCoroutines();
        m_currentAstroidCount = 0;
    }
    
    private IEnumerator SpawnInitialAstroids()
    {
        while(m_currentAstroidCount < m_maximumAstroids)
        {
            yield return new WaitForSeconds(m_SpawnDelay);
            SpawnRandomAstroid(3, GetRandomSpawnPoint());   
            m_currentAstroidCount++;
        }

        GameEvents.Instance.InitialAstroidsSpawned();
    } 

    private UnityEngine.Vector2 GetRandomSpawnPoint()
    {
        int side = Random.Range(0, 4);

        float x = 0f;
        float y = 0f;

        switch(side){
            
            // Top
            case 0:
                x = Random.Range(m_spawnArea.xMin, m_spawnArea.xMax);
                y = m_spawnArea.yMax;
                break;
            // Right
            case 1:
                x = m_spawnArea.xMax;
                y = Random.Range(m_spawnArea.yMin, m_spawnArea.yMax);  
                break;
            // Bottom
            case 2:
                x = Random.Range(m_spawnArea.xMin, m_spawnArea.xMax);
                y = m_spawnArea.yMin;
                break;
            // Left
            case 3:
                x = m_spawnArea.xMin;
                y = Random.Range(m_spawnArea.yMin, m_spawnArea.yMax);
                break;
            default:
                break;
        }

        return new UnityEngine.Vector2(x, y);
    }


    private void SpawnRandomAstroid(float size, UnityEngine.Vector2 spawnPoint)
    {
        IEnumerable<GameObject> astroidPrefabs = m_astroidPrefabs.Where((astroid) => astroid.GetComponent<AstroidController>().Size == size);
        if(astroidPrefabs == null || astroidPrefabs.Count() <= 0){ return; }
        int index = Random.Range(0, astroidPrefabs.Count());
        GameObject astroidToSpawn = Instantiate(astroidPrefabs.ElementAt(index), transform);

        astroidToSpawn.transform.position = spawnPoint;

        AstroidController controller = astroidToSpawn.GetComponent<AstroidController>();
        controller.OnAstroidDestroyed += OnAstroidDestroyed;
    }

    private void OnAstroidDestroyed(AstroidController astroidObj){

        if(astroidObj == null){ return; }

        UnityEngine.Vector2 astroidPosition = astroidObj.transform.position;

        int size = astroidObj.Size;

        Destroy(astroidObj.gameObject);

        GameEvents.Instance.AddToScore(1);

        size--;

        int numToSpawn = m_numOfAstroidsToSpawn[size];

        if(size > 0){
            for(int i = 0; i < numToSpawn; i++){
                m_currentAstroidCount++;
                SpawnRandomAstroid(size, (Random.insideUnitCircle * 5f) + astroidPosition);
                SpawnCollectible(astroidPosition);
            }
            
        }
    }
}
