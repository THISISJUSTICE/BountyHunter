using System.Collections;
using System.Collections.Generic;
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
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    public static ObjectManager Instace{
        get{
            return instance;
        }
    }
    #endregion

    [System.Serializable]
    public class DungeonObjects{ //던전의 장애물 오브젝트 모음
        public ObstacleBasic[] rockObstaclePrefabs; //바위맵 장애물 프리팹 모음
        public Stack<ObstacleBasic>[] rockObstacleObjects; //바위맵 장애물 풀링용 오브젝트

        public DungeonObjects(){
            //rockObstacleObjects = new Stack<ObstacleBasic>[rockObstaclePrefabs.Length];
            rockObstacleObjects = new Stack<ObstacleBasic>[5];
            for(int i=0; i<rockObstacleObjects.Length; ++i){
                rockObstacleObjects[i] = new Stack<ObstacleBasic>();
            }
        }
    }
    public DungeonObjects dungeonObjects;

    [System.Serializable]
    public class PlayerObjects{ //플레이어 총알, 무기 등의 오브젝트 모음
        public SoldierGun[] pistolPrefabs; //군인 총 프리팹 모음
        public Stack<SoldierGun>[] pistolObjects; //군인 총 풀리용 오브젝트
        public GunBullet[] bulletPrefabs; //총알 프리팹 모음 (다양한 총알)
        public Stack<GunBullet>[] bulletObjects; //총알 풀랑용 오브젝트

        public PlayerObjects(){
            //bulletObjects = new Stack<GunBullet>[bulletPrefabs.Length];
            bulletObjects = new Stack<GunBullet>[1];
            for(int i=0; i<bulletObjects.Length; ++i){
                bulletObjects[i] = new Stack<GunBullet>();
            }
            //pistolObjects = new Stack<SoldierGun>[pistolPrefabs.Length];
            pistolObjects = new Stack<SoldierGun>[1];
            for(int i=0; i<bulletObjects.Length; ++i){
                pistolObjects[i] = new Stack<SoldierGun>();
            }
        }
    }
    public PlayerObjects playerObjects;

    void ObjectInit(){
        Instace.dungeonObjects = new DungeonObjects();
        Instace.playerObjects = new PlayerObjects();
    }

    private void Awake() {
        SingletonInit();   
        ObjectInit();
    }

    private void Start() {
        
    }

}
