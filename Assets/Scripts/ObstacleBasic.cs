using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//장애물은 플레이어의 방어력이 더 높거나 높은 속도 혹은 데미지를 가지면 부술 수 있음
public class ObstacleBasic : MonoBehaviour
{
    public Vector3 waitPos; //장애물이 대기 중일 때의 위치
    public Vector3 appearPos; //장애물이 나타날 때의 위치
    public float appearSpeed; //장애물이 나타나는 속도
    public int appearFrame; //장애물이 나타나는 프레임

    [System.Serializable]
    public class ObstacleStatus{ //장애물 스테이터스 클래스
        public int maxHealthPoint; //장애물 최대 체력
        public int armor; //장애물의 방어력
        public int magicRegistant; //장애물의 마법저항력
    }
    public ObstacleStatus obstacleStatus; //장애물 스테이터스
    
    int curHealthPoint; //장애물 현재 체력
    

    //정해진 범위에 플레이어가 들어오면 장해물 등장
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Chaser" || other.tag == "Player") StartCoroutine(Appear());
    }

    //장애물 등장
    IEnumerator Appear(){
        for(int i=0; i<appearFrame; ++i){
            transform.position += (appearPos - waitPos) / appearFrame;
            yield return new WaitForSeconds(appearSpeed);
        } 
    }
}
