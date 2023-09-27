using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using UnityEditor.Timeline.Actions;

public class DefineObstacles : MonoBehaviour
{
    [System.Serializable]
    public class Data{ //장애물 공통 데이터
        public int prefabKind; //원본 장애물의 프리팹 종류
        public int[] area; //차지하는 칸 수 가로, 세로
        public Vector3 waitPos; //장애물이 대기 중일 때의 포지션 x,y,z
        public float appearWaitTime; //장애물이 나타날 때 대기 시간
        public float appearSpeed; //장애물이 나타나는 속도
        public float appearheight; //장애물이 나타날 때의 높이
        public Vector3 objScale; //오브젝트 스케일 x, y, z
        public Vector3 colPos; //충돌 콜라이더 포지션 x,y,z
        public Vector3 colScale; //충돌 콜라이더 스케일 x, y, z
        public int maxHealthPoint; //장애물의 최대 최력
        public int armor; //장애물의 방어력
        public int magicRegistant; //장애물의 마법 저항력
    }
    public Data[] rockObstacleData;
    public string basicfileName{ //파일 명 앞에 장애물의 종류 붙이기
        get{ return "ObstacleData"; }
    }
    public string filePath{ //파일 경로
        get{ return "C:/Users/yulop/RPGGame/BountyHunter/Assets/MapInfos/DungeonObstaclesKind"; }
    }

    
    private void Awake() {
        SaveData(rockObstacleData, "Rock" + basicfileName);
    }
        

    //json 파일 저장하기
    void SaveData(Data[] data, string fileName)
    {
        //클래스를 문자열로 된 Json 데이터로 변환
        string ToJsonData = "";
        for(int i=0; i<data.Length; ++i){
            ToJsonData += JsonUtility.ToJson(data[i]);
            if(i<data.Length-1) ToJsonData += "\n";
        }
        
        //Json 파일을 생성하고 저장(파일이 이미 있으면 덮어쓰기)
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", filePath, fileName), FileMode.Create);
        byte[] dataByte = Encoding.UTF8.GetBytes(ToJsonData);
        fileStream.Write(dataByte, 0, dataByte.Length);
        fileStream.Close();
    }
    
}
