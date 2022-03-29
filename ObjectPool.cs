using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPool : MonoBehaviour
{
    public static objectPool Instance; // singleton

    public GameObject bulletPrefab;
    public GameObject missilePrefab;

    const int numBullets = 15;
    const int numMissiles = 5;

    GameObject[] bulletPool;
    GameObject[] missilePool;

    private void Awake()
    {
        Instance = this;

        bulletPool =  new GameObject[numBullets];
        missilePool = new GameObject[numMissiles];

        generateInstance();
    }

    void generateInstance()
    {
        for (int i = 0; i < numBullets; i++)
        {
            bulletPool[i] = Instantiate(bulletPrefab, new Vector3(0,0,0), Quaternion.identity);
            bulletPool[i].SetActive(false);
            bulletPool[i].transform.parent = this.transform;
        }

        for (int i = 0; i < numMissiles; i++)
        {
            missilePool[i] = Instantiate(missilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            missilePool[i].SetActive(false);
            missilePool[i].transform.parent = this.transform;
        }
    }

    public static GameObject getBullet()
    {
        for (int i = 0; i < numBullets; i++)
        {
            if (!Instance.bulletPool[i].activeSelf)
                return Instance.bulletPool[i];
        }

        return null;
    }

    public static GameObject getMissile()
    {
        for (int i = 0; i < numMissiles; i++)
        {
            if (!Instance.missilePool[i].activeSelf)
                return Instance.missilePool[i];
        }

        return null;
    }
}
