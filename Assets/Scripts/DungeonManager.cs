using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

// 할 일
// - 장애물 소환
// - 몬스터 소환진 소환


/*텍스트 파일 양식
상위 폴더 제목: 던전의 종류(dungeonKind)
텍스트 파일 제목: 스테이지 레벨(dungeonLevel)
첫 줄: 던전의 세로 길이(dungeonLength), 가로 길이
본문: 장애물 위치 정보(dungeonInfo), 맵의 세로 칸: (길이/5)칸 - 6, 가로 칸: 5 or 7
-> 0~n: 장애물 종류 별로 생성, 98: 가로 칸에 랜덤으로 장애물 생성(없는 것도 포함), 99: 빈 곳
*/

//플로어 100 = 터레인 1000(+10 끝부분 막기), 플로어의 길이는 10의 배수
//장애물 위치 z(start + 10 ~ end-20), 세로 1칸 당 z: 3.5, 가로 1칸당 x: 1.6
public class DungeonManager : MonoBehaviour
{
    #region Variable
    const float floorHorizontal = 1.8f; //가로 1칸 당 길이
    const float floorVertical = 3.5f; //세로 1칸 당 길이
    int dungeonLength; //던전의 세로 길이(길이 = 플로어의 scale z값 * 10) (이 길이를 참고하여 적절한 길이의 배경 터레인을 선택)
    int dungeonWidth; //던전의 가로 길이(5 or 7)
    List<int>[] dungeonInfo; //던전의 정보(장애물 발생, 몬스터, 스폰 위치 및 정보)
    List<DefineObstacles.Data> obstacleDatas; //장애물 종류에 따른 데이터
    Queue<ObstacleBasic>[] curObstacleObjects; //현재 맵에 맞는 풀링용 오브젝트
    ObstacleBasic[] curObstaclePrefabs; //현재 맵에 맞는 장애물 프리팹 모음

    #region StageObstacles

    public TextAsset[] RockDungeonStage; //바위맵 던전 스테이지 텍스트 파일
    public TextAsset RockObstaclesData; //바위맵 장애물 정보 텍스트 파일

    #endregion

    #endregion

    private void Start() {
        DungeonStart(DungeonKind.Rock, 0);
    }

    //다른 스크립트에서 던전의 종류, 스테이지 레벨을 받아 던전 시작
    public void DungeonStart(DungeonKind dungeonKind, int stageLevel){
         ReadStageFile(dungeonKind, stageLevel);
         obstacleDatas = LoadDungeonObstacleData(dungeonKind);
         CreateDungeon();
    }

    //던전 종류와 스테이지 레벨을 받아 던전 파일을 고르고 그 파일의 정보를 읽기
    void ReadStageFile(DungeonKind dungeonKind, int stageLevel){
        //텍스트 파일 인식
        TextAsset txtFile;
        txtFile = RockDungeonStage[stageLevel];
        curObstaclePrefabs = ObjectManager.Instance.dungeonObjects.ReturnPrefabs(dungeonKind);
        curObstacleObjects = ObjectManager.Instance.dungeonObjects.ReturnObjects(dungeonKind);
        

        StringReader strRea = new StringReader(txtFile.text);
        string line;
        int index = 0; //dungeonInfo 리스트의 배열 인덱스
        bool first = true; //첫번째 줄인지 확인하는 변수

        //텍스트 파일 읽기
        while(strRea != null){
            line = strRea.ReadLine();

            if(line == null) break;

            if(first){
                first = false;
                dungeonLength = int.Parse(line.Split(',')[0]);
                dungeonWidth = int.Parse(line.Split(',')[1]);

                dungeonInfo = new List<int>[(int)Math.Truncate((dungeonLength - 30) / floorVertical)]; //맵의 앞 뒤 끝 부분은 장애물 생성 X
                for(int i=0; i<dungeonInfo.Length; ++i){
                    dungeonInfo[i] = new List<int>();
                }
            }
            else{
                for(int i=0; i<dungeonWidth; ++i){
                    dungeonInfo[index].Add(int.Parse(line.Split(',')[i]));
                }
                ++index;
            }
        }
        Debug.Log("Lenght: " + dungeonInfo.Length);
    }

    //던전 종류에 따른 장애물 데이터 로드
    List<DefineObstacles.Data> LoadDungeonObstacleData(DungeonKind dungeonKind){
        TextAsset obda;
        List<DefineObstacles.Data> jsonList = new List<DefineObstacles.Data>();

        switch(dungeonKind){
            case DungeonKind.Rock:
                obda = RockObstaclesData;
                break;
            default:
                obda = null;
                break;
        }

        StringReader strRea = new StringReader(obda.text);
        string line;
        
        while(strRea != null){
            line = strRea.ReadLine();
            if(line == null) break;
            jsonList.Add(JsonUtility.FromJson<DefineObstacles.Data>(line));
        }

        return jsonList;
    }

    //텍스트 파일을 바탕으로 장애물 배치
    void CreateDungeon(){
        //장애물을 놓을 위치
        float x; //가로
        float z = 10; //세로

        for(int i=0; i<dungeonInfo.Length; ++i, z+=floorVertical){
            x = floorHorizontal * 2;
            for(int j=0; j<dungeonInfo[0].Count; ++j, x-=floorHorizontal){
                if(dungeonInfo[i][j] == 99) continue;
                CreateObstacle(dungeonInfo[i][j], x, z);
            }
        }
    }

    //풀링된 오브젝트가 없으면 생성
    void CreateObstacle(int index, float x, float z){
        ObstacleBasic curob;
        int num = obstacleDatas[index].prefabKind;
        if(curObstacleObjects[num].Count > 0){
            curob = curObstacleObjects[num].Dequeue();
            curob.gameObject.SetActive(true);
        }
        else{ 
            curob = Instantiate(curObstaclePrefabs[num]);
        }
        curob.ObstacleBasicInit(this, obstacleDatas[index], new Vector3(x, obstacleDatas[index].appearheight, z));
    }

    public void DeleteObstacle(ObstacleBasic obsB, int kind){
        curObstacleObjects[kind].Enqueue(obsB);
        obsB.gameObject.SetActive(false);
    }

    public void DungeonEnd(DungeonKind dungeonKind){
        ObjectManager.Instance.dungeonObjects.UpdateQueue(dungeonKind, curObstacleObjects);
    }

}
