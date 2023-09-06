using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    public int total;          // Jumlah total objek dalam pool.
    public string poolName;        // Nama untuk referensi pool.
    public GameObject poolObject;     // Objek yang akan dipool.
    public Transform poolParent;   // Transform parent untuk objek-objek yang dipool.

    private List<GameObject> objectPool = new List<GameObject>();

    void Start()
    {
        // Inisialisasi pool objek saat aplikasi dimulai.
        InitializeObjectPool();
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < total; i++)
        {
            GameObject newObj = Instantiate(poolObject);
            newObj.name = poolName + "_" + i;
            newObj.transform.SetParent(poolParent);
            newObj.SetActive(false);
            objectPool.Add(newObj);
        }
    }

    public GameObject SpawnObject(Vector3 position, Quaternion rotation)
    {
        // Cari objek yang tidak aktif dalam pool.
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                objectPool[i].transform.position = position;
                objectPool[i].transform.rotation = rotation;
                objectPool[i].SetActive(true);
                return objectPool[i];
            }
        }

        // Jika semua objek dalam pool aktif, buat objek baru jika diperlukan.
        GameObject newObj = Instantiate(poolObject, position, rotation);
        newObj.name = poolName + "_" + objectPool.Count;
        newObj.transform.SetParent(poolParent);
        objectPool.Add(newObj);
        return newObj;
    }

    public void DeactivateAllObjects()
    {
        // Nonaktifkan semua objek dalam pool.
        for (int i = 0; i < objectPool.Count; i++)
        {
            objectPool[i].SetActive(false);
        }
    }
}
