using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플로어 100 = 터레인 1000(+10 끝부분 막기), 플로어의 길이는 10의 배수
//장애물 위치 z(start + 10 ~ end-20), 세로 1칸 당 z: 5, 가로 1칸당 x: 1.6
public class DungeonManager : MonoBehaviour
{
    public string dungeonKind; //던전의 종류
    public int dungeonLevel; //던전의 레벨
    public int dungeonLength; //던전의 길이(플로어의 scale.z*10)
    public List<int>[] dungeonInfo; //던전의 정보(장애물 발생, 몬스터, 스폰 위치 및 정보)

    void Start(){
        DungeonManagerInit();
    }

    void DungeonManagerInit(){
        dungeonInfo = new List<int>[(dungeonLength - 30) / 5];
    }

}
