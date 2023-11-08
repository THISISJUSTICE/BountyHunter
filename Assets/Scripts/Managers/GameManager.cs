using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //싱글톤 패턴으로 정의
    #region Singleton
    private static GameManager instance = null;

    void SingletonInit(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static GameManager Inst{
        get{
            return instance;
        }
    }
    #endregion
    
    public List<int>[] curDungeonInfo; //현재 진행중인 던전의 장애물 정보(0: 아무것도 없음, 1: 장애물 존재)

    private void Awake() {
        SingletonInit();
    }

    
}
