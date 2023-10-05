using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

//장애물은 플레이어의 방어력이 더 높거나 높은 속도 혹은 데미지를 가지면 부술 수 있음
public class ObstacleBasic : MonoBehaviour
{
    public Vector3 waitPos; //장애물이 대기 중일 때의 위치
    public Vector3 appearPos; //장애물이 나타날 때의 위치
    public float appearSpeed; //장애물이 나타나는 속도
    public float appearWaitTime; //장애물이 나타날 때 대기 시간
    public int appearFrame; //장애물이 나타나는 프레임
    public int hpDecreseRate; //장애물의 체력을 특정 비율로 나누기
    bool isAppear; //장애물이 등장했는지 확인
    BoxCollider obstacleColi; //장애물의 충돌

    public Status obstacleStatus; //장애물 스테이터스
    
    int curHealthPoint; //장애물 현재 체력
    int curHPDecreseIndex; //장애물 현재 체력의 남은 비율
    MeshRenderer meshRenderer;
    DungeonManager dungeonManager;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        obstacleColi = transform.GetChild(0).GetComponent<BoxCollider>();
    }

    public void ObstacleBasicInit(DungeonManager dungeonManager, DefineObstacles.Data deObData, Vector3 appearPos){
        gameObject.SetActive(true);
        isAppear = false;
        meshRenderer.enabled = false;
        this.dungeonManager = dungeonManager;
        this.appearPos = appearPos;
        curHPDecreseIndex = 1;

        //장애물 데이터 입력
        waitPos = new Vector3(appearPos.x + deObData.waitPos.x, deObData.waitPos.y, appearPos.z + deObData.waitPos.z);
        appearSpeed = deObData.appearSpeed;
        appearWaitTime = deObData.appearWaitTime;
        transform.position = waitPos;
        transform.localScale = deObData.objScale;
        obstacleColi.center = deObData.colPos;
        obstacleColi.size = deObData.colScale;
        //OnCollisionEnter onCollisionEnter = new OnCollisionEnter();
        
        //스테이터스 입력
        obstacleStatus.maxHealthPoint = deObData.maxHealthPoint;
        obstacleStatus.armor = deObData.armor;
        obstacleStatus.magicRegistant = deObData.magicRegistant;
        
        curHealthPoint = obstacleStatus.maxHealthPoint;
    }

    //정해진 범위에 플레이어가 들어오면 장해물 등장
    private void OnTriggerEnter(Collider other) {
        if((other.tag == "Chaser" || other.tag == "Player") && !isAppear) StartCoroutine(Appear());
    }

    //장애물 등장
    IEnumerator Appear(){
        isAppear = true;
        yield return new WaitForSeconds(appearWaitTime);
        meshRenderer.enabled = true;        
        for(int i=0; i<appearFrame; ++i){
            transform.position += (appearPos - waitPos) / appearFrame;
            yield return new WaitForSeconds(appearSpeed);
        } 
    }

    //돌진 시 받는 데미지 계산
    public void RushDamaged(int armor, float acceleration){
        Debug.Log("충돌");
        if(obstacleStatus.armor / 2 < armor){
            HPDecrese(armor / obstacleStatus.armor * (int)(acceleration * 20));
        }
    }

    //피해를 받을 때 실행
    public void Attacked(int attackDamage, int magicDamage){
        int dmg = obstacleStatus.CalculateDamage(attackDamage, magicDamage);
        if(dmg > 0){
            HPDecrese(dmg);
        }
    }

    //피해를 입었을 때 효과
    void DamagedEffect(){
        Debug.Log("데미지 이펙트");
    }

    //받은 데미지를 바탕으로 체력 감소
    void HPDecrese(int dmg){
        curHealthPoint -= dmg;
        if(curHealthPoint <= obstacleStatus.maxHealthPoint / hpDecreseRate * (hpDecreseRate - curHPDecreseIndex)){
            DamagedEffect();
            curHPDecreseIndex = hpDecreseRate - (curHealthPoint * hpDecreseRate / obstacleStatus.maxHealthPoint);
        }
        if(curHealthPoint <= 0) Death();
    }

    //체력이 0이 되어 파괴
    void Death(){
        DestroyEffect();
        gameObject.SetActive(false);
    }

    void DestroyEffect(){

    }
}
