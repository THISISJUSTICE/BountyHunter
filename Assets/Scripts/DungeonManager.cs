using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*텍스트 파일 양식
상위 폴더 제목: 던전의 종류(dungeonKind)
텍스트 파일 제목: 스테이지 레벨(dungeonLevel)
첫 줄: 던전의 세로 길이(dungeonLength), 가로 길이
본문: 장애물 위치 정보(dungeonInfo), 맵의 세로 칸: (길이/5)칸 - 6, 가로 칸: 5 or 7
-> 0: 빈 곳, 1~n: 장애물 종류 별로 생성, 99: 가로 칸에 랜덤으로 장애물 생성
*/

//플로어 100 = 터레인 1000(+10 끝부분 막기), 플로어의 길이는 10의 배수
//장애물 위치 z(start + 10 ~ end-20), 세로 1칸 당 z: 5, 가로 1칸당 x: 1.6
public class DungeonManager : MonoBehaviour
{
    int dungeonLength; //던전의 세로 길이(길이 = 플로어의 scale z값 * 10) (이 길이를 참고하여 적절한 길이의 배경 터레인을 선택)
    int dungeonWidth; //던전의 가로 길이(5 or 7)
    List<int>[] dungeonInfo; //던전의 정보(장애물 발생, 몬스터, 스폰 위치 및 정보)
    public TextAsset[] RockDungeonStage;
    List<DefineObstacles.Data> datas;

    void Start(){
        DungeonManagerInit();
    }

    void DungeonManagerInit(){
        datas = new List<DefineObstacles.Data>();
    }

    //다른 스크립트에서 던전의 종류, 스테이지 레벨을 받아 던전 시작
    public void DungeonStart(string dungeonKind, int stageLevel){
         ReadStageFile(dungeonKind, stageLevel);
         CreateDungeon();
    }

    //던전 종류와 스테이지 레벨을 받아 던전 파일을 고르고 그 파일의 정보를 읽기
    void ReadStageFile(string dungeonKind, int stageLevel){
        //텍스트 파일 인식
        TextAsset txtFile;
        switch(dungeonKind){
            case "Rock":
                txtFile = RockDungeonStage[stageLevel];
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

    void LoadDungeonObstacleData(string dungeonKind){
        
    }

    void CreateDungeon(){
        for(int i=0; i<dungeonLength; ++i){
            for(int j=0; j<dungeonWidth; ++j){
                if(dungeonInfo[i][j] == 0) continue;

            }
        }
    }

}
