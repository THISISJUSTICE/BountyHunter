using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

//오브젝트 풀링용
public class ObjectManager : MonoBehaviour
{
    //싱글톤 패턴으로 정의
    #region Singleton
    private static ObjectManager instance = null;

    void SingletonInit(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static ObjectManager Instance{
        get{
            return instance;
        }
    }
    #endregion

    [System.Serializable]
    public class DungeonObjects{ //던전의 장애물 오브젝트 모음
        ObstacleBasic[] rockObstaclePrefabs; //바위맵 장애물 프리팹 모음
        Queue<ObstacleBasic>[] rockObstacleObjects; //바위맵 장애물 풀링용 오브젝트

        public void Init(){
            rockObstaclePrefabs = Resources.LoadAll<ObstacleBasic>("Prefabs/Obstacles/RockDungeonObstacles");
            rockObstacleObjects = new Queue<ObstacleBasic>[rockObstaclePrefabs.Length];
            for(int i=0; i<rockObstacleObjects.Length; ++i){
                rockObstacleObjects[i] = new Queue<ObstacleBasic>();
            } 
        }

        public ObstacleBasic[] ReturnPrefabs(DungeonKind dungeonKind){
            switch(dungeonKind){
                case DungeonKind.Rock:
                    return rockObstaclePrefabs;
                
            }
            return null;
        }

        public Queue<ObstacleBasic>[] ReturnObjects(DungeonKind dungeonKind){
            switch(dungeonKind){
                case DungeonKind.Rock:
                    return rockObstacleObjects;
                
            }

            return null;
        }

        public void UpdateQueue(DungeonKind dungeonKind, Queue<ObstacleBasic>[] curQueue){
            switch(dungeonKind){
                case DungeonKind.Rock:
                    rockObstacleObjects = curQueue;
                    break;
                
            }
        }
    }
    public DungeonObjects dungeonObjects;

    [System.Serializable]
    public class ObstacleEffects{ //장애물 오브젝트의 이펙트 모음
        public ParticleSystem[] damagedEffectPrefabs; //장애물이 피해를 입을 때 이펙트 프리팹
        public Queue<ParticleSystem>[] damagedEffectObjects; //피해 이펙트 오브젝트
        public ParticleSystem[] collapseEffectPrefabs; //장애물이 파괴될 때 이펙트
        public Queue<ParticleSystem>[] collapseEffectObjects; //파괴 이펙트 오브젝트

        public void Init(){
            damagedEffectPrefabs = Resources.LoadAll<ParticleSystem>("Prefabs/Obstacles/Effects/DamagedEffects");
            damagedEffectObjects = new Queue<ParticleSystem>[damagedEffectPrefabs.Length];

            collapseEffectPrefabs = Resources.LoadAll<ParticleSystem>("Prefabs/Obstacles/Effects/CollapseEffects");
            collapseEffectObjects = new Queue<ParticleSystem>[collapseEffectPrefabs.Length];

            for(int i=0; i<damagedEffectObjects.Length; ++i){
                damagedEffectObjects[i] = new Queue<ParticleSystem>();
            }

            for(int i=0; i<collapseEffectObjects.Length; ++i){
                collapseEffectObjects[i] = new Queue<ParticleSystem>();
            }
        }
    }
    public ObstacleEffects obstacleEffects;

    [System.Serializable]
    public class PlayerObjects{ //플레이어 총알, 무기 등의 오브젝트 모음
        public SoldierGun[] pistolPrefabs; //군인 총 프리팹 모음
        public Queue<SoldierGun>[] pistolObjects; //군인 총 풀리용 오브젝트
        public GunBullet[] bulletPrefabs; //총알 프리팹 모음 (다양한 총알)
        public Queue<GunBullet>[] bulletObjects; //총알 풀랑용 오브젝트

        public void Init(){
            pistolPrefabs = Resources.LoadAll<SoldierGun>("Prefabs/Weapons/Soldier_Weapon/Pistols");
            pistolObjects = new Queue<SoldierGun>[pistolPrefabs.Length];
            for(int i=0; i<pistolObjects.Length; ++i){
                pistolObjects[i] = new Queue<SoldierGun>();
            }

            bulletPrefabs = Resources.LoadAll<GunBullet>("Prefabs/Weapons/Soldier_Weapon/Bullets");
            bulletObjects = new Queue<GunBullet>[bulletPrefabs.Length];
            for(int i=0; i<bulletObjects.Length; ++i){
                bulletObjects[i] = new Queue<GunBullet>();
            }
        }
    }
    public PlayerObjects playerObjects;

    void ObjectInit(){
        dungeonObjects = new DungeonObjects();
        dungeonObjects.Init();
        playerObjects = new PlayerObjects();
        playerObjects.Init();
        obstacleEffects = new ObstacleEffects();
        obstacleEffects.Init();
    }

    private void Awake() {
        SingletonInit();   
        ObjectInit();
    }

}
