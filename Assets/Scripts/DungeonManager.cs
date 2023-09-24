using System.Collections;
using System.Collections.Generic;
using System.IO;
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
//장애물 위치 z(start + 10 ~ end-20), 세로 1칸 당 z: 5, 가로 1칸당 x: 1.6
public class DungeonManager : MonoBehaviour
{
    int dungeonLength; //던전의 세로 길이(길이 = 플로어의 scale z값 * 10) (이 길이를 참고하여 적절한 길이의 배경 터레인을 선택)
    int dungeonWidth; //던전의 가로 길이(5 or 7)
    List<int>[] dungeonInfo; //던전의 정보(장애물 발생, 몬스터, 스폰 위치 및 정보)
    public TextAsset[] RockDungeonStage; //바위맵 던전 스테이지 텍스트 파일
    List<DefineObstacles.Data> obstacleDatas; //장애물 종류에 따른 데이터

    Stack<GameObject>[] curObstacleObjects; //현재 맵에 맞는 풀리용 오브젝트
    Stack<GameObject>[] rockObstacleObjects; //장애물 풀링용 오브젝트

    GameObject[] curObstaclePrefabs; //현재 맵에 맞는 장애물 프리팹 모음
    public GameObject[] rockObstaclePrefabs; //장애물 프리팹 모음

    void Awake(){
        DungeonManagerInit();
    }

    void DungeonManagerInit(){
        rockObstacleObjects = new Stack<GameObject>[rockObstaclePrefabs.Length]; //프리팹 수 만큼 생성
        for(int i=0; i<rockObstacleObjects.Length; ++i) rockObstacleObjects[i] = new Stack<GameObject>();
    }

    //다른 스크립트에서 던전의 종류, 스테이지 레벨을 받아 던전 시작
    public void DungeonStart(string dungeonKind, int stageLevel){
         ReadStageFile(dungeonKind, stageLevel);
         LoadDungeonObstacleData(dungeonKind);
         CreateDungeon();
    }

    //던전 종류와 스테이지 레벨을 받아 던전 파일을 고르고 그 파일의 정보를 읽기
    void ReadStageFile(string dungeonKind, int stageLevel){
        //텍스트 파일 인식
        TextAsset txtFile;
        switch(dungeonKind){
            case "Rock":
                txtFile = RockDungeonStage[stageLevel];
                curObstaclePrefabs = rockObstaclePrefabs;
                curObstacleObjects = rockObstacleObjects;
                break;
            default:
                txtFile = null;
                break;
        }

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

                dungeonInfo = new List<int>[(dungeonLength - 30) / 5]; //맵의 앞 뒤 끝 부분은 장애물 생성 X
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
    }

    //던전 종류에 따른 장애물 데이터 로드
    void LoadDungeonObstacleData(string dungeonKind){
        DefineObstacles deOb = new DefineObstacles();
        obstacleDatas = deOb.LoadJsonFile<DefineObstacles.Data>(dungeonKind + deOb.filePath, deOb.basicfileName);
    }

    //텍스트 파일을 바탕으로 장애물 배치
    void CreateDungeon(){
        //장애물을 놓을 위치
        float x; //가로 (1칸당 1.6)
        float z = 10; //세로 (1칸당 10)

        for(int i=0; i<dungeonLength; ++i, z+=10){
            x = 3.2f;
            for(int j=0; j<dungeonWidth; ++j, x-=1.6f){
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
            curob = curObstacleObjects[num].Pop().GetComponent<ObstacleBasic>();
        }
        else{ 
            curob = Instantiate(curObstaclePrefabs[num]).GetComponent<ObstacleBasic>();
        }
        curob.ObstacleBasicInit(this, obstacleDatas[index], new Vector3(x, obstacleDatas[index].appearheight, z));

    }

    public void DeleteObstacle(ObstacleBasic obsB){

    }

}
