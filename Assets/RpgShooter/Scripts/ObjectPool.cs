using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> bulletPool;
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
        bulletPool = new Queue<GameObject>();
        CreateInitialPool();
    }

    public GameObject GetBullet()
    {
        if(bulletPool.Count == 0)
            CreateBullet();
        GameObject bullet = bulletPool.Dequeue();
        bullet.SetActive(true);
        bullet.transform.parent = null;
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        print("开始返回缓存池");
        bulletPool.Enqueue(bullet);
        Debug.Log("缓存池数量" + bulletPool.Count);
        bullet.SetActive(false);
        bullet.transform.parent = transform;
    }
    private void CreateInitialPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateBullet();
        }
    }

    private void CreateBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform);
        newBullet.SetActive(false);
        bulletPool.Enqueue(newBullet);
    }
}
