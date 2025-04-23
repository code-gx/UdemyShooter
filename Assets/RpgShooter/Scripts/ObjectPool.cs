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
        bullet.transform.SetParent(null);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bulletPool.Enqueue(bullet);
        bullet.SetActive(false);
        bullet.transform.SetParent(transform);
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
