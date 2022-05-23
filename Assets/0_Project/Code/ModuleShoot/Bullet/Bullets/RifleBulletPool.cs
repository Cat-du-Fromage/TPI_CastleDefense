using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
/*
namespace KaizerWald
{
    public class RifleBulletPool : MonoBehaviour
    {
        [SerializeField] private BulletComponent prefab;
        
        public IObjectPool<BulletComponent> BulletPool;

        private List<BulletComponent> activeBullets;

        private Transform cacheBullet;
        private BulletData cachedData;

        private void Awake()
        {
            BulletPool = new ObjectPool<BulletComponent>(CreateToken, OnGet, OnRelease, OnDestroyObject);
            activeBullets = new List<BulletComponent>(2);
        }

        private void Update()
        {
            if (activeBullets.Count == 0) return;

            for (int i = 0; i < activeBullets.Count; i++)
            {
                if (!activeBullets[i].data.IsLoaded) continue;
                if ((activeBullets[i].transform.position - activeBullets[i].data.StartPosition).sqrMagnitude > activeBullets[i].data.MaxRange)
                {
                    BulletPool.Release(activeBullets[i]);
                }
            }
        }

        private BulletComponent CreateToken()
        {
            BulletComponent token = Instantiate(prefab).GetComponent<BulletComponent>();
            return token;
        }

        private void OnGet(BulletComponent bullet)
        {
            bullet.gameObject.SetActive(true);
            activeBullets.Add(bullet);
        }
        
        private void OnRelease(BulletComponent bullet)
        {
            bullet.gameObject.SetActive(false);
            activeBullets.Remove(bullet);
        }

        private void OnDestroyObject(BulletComponent bullet)
        {
            Destroy(bullet.gameObject);
        }

        public void ReleaseAll(ref List<BulletComponent> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                BulletPool.Release(objects[i]);
            }
            objects.Clear();
        }
    }
}
*/