using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private GameObject weaponPickupPrefab;
    [SerializeField] private GameObject ammoPickupPrefab;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = 
        new Dictionary<GameObject, Queue<GameObject>>();
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeNewPool(weaponPickupPrefab);
        InitializeNewPool(ammoPickupPrefab);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab); //缓存池字典无缓存池
        }
        if (poolDictionary[prefab].Count == 0)
            CreateObject(prefab);  //缓存池无对象
        GameObject gameObject = poolDictionary[prefab].Dequeue();
        gameObject.SetActive(true);
        gameObject.transform.parent = null;
        return gameObject;
    }

    private IEnumerator DelayReturn(GameObject gameObject,float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(gameObject);
    }

    public void DelayReturnObject(GameObject gameObject, float delay) => StartCoroutine(DelayReturn(gameObject, delay));

    public void ReturnToPool(GameObject gameObject)
    {
        GameObject originalPrefab = gameObject.GetComponent<PooledObject>().originalPrefab;
        poolDictionary[originalPrefab].Enqueue(gameObject);

        gameObject.SetActive(false);
        gameObject.transform.parent = transform;
    }
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateObject(prefab);
        }
    }

    private void CreateObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }
}
