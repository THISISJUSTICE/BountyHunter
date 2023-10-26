using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

//장애물은 플레이어의 방어력이 더 높거나 높은 속도 혹은 데미지를 가지면 부술 수 있음
public class ObstacleBasic : ObjectsBasic
{
    DungeonKind dungeonKind; //던전 종류
    int kind; //장애물의 종류
    public Status obstacleStatus; //장애물 스테이터스
    public Vector3 waitPos; //장애물이 대기 중일 때의 위치
    public Vector3 appearPos; //장애물이 나타날 때의 위치
    public float appearSpeed; //장애물이 나타나는 속도
    public float appearWaitTime; //장애물이 나타날 때 대기 시간
    public int appearFrame; //장애물이 나타나는 프레임
    bool isAppear; //장애물이 등장했는지 확인
    BoxCollider obstacleColi; //장애물의 충돌

    MeshRenderer meshRenderer;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        obstacleColi = transform.GetChild(0).GetComponent<BoxCollider>();
    }

    public void ObstacleBasicInit(DefineObstacles.Data deObData, Vector3 appearPos, DungeonKind dungeonKind){
        isAppear = false;
        meshRenderer.enabled = false;
        this.appearPos = appearPos;
        this.dungeonKind = dungeonKind;
        obstacleColi.enabled = true;

        //장애물 데이터 입력
        waitPos = new Vector3(appearPos.x + deObData.waitPos.x, deObData.waitPos.y, appearPos.z + deObData.waitPos.z);
        kind = deObData.prefabKind;
        appearSpeed = deObData.appearSpeed;
        appearWaitTime = deObData.appearWaitTime;
        transform.position = waitPos;
        transform.localScale = deObData.objScale;
        obstacleColi.center = deObData.colPos;
        obstacleColi.size = deObData.colScale;
        
        //스테이터스 입력
        obstacleStatus.maxHealthPoint = deObData.maxHealthPoint;
        obstacleStatus.armor = deObData.armor;
        ownArmor = obstacleStatus.armor;
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

    // //돌진을 받을 시
    // public void Rushed(int armor, float acceleration){
    //     HPDecrese(RushDamaged(obstacleStatus.armor, armor, acceleration));
    // }

    //피해를 받을 때 실행(오브젝트의 위치 반환)
    public void Attacked(int attackDamage, int magicDamage, Vector3 dmgPos){
        int dmg = obstacleStatus.CalculateDamage(attackDamage, magicDamage);
        if(dmg > 0){
            HPDecrese(dmg);
            
            if(dmg > obstacleStatus.maxHealthPoint / 50) DamagedEffect(dmgPos);
        }
    }

    //피해를 입었을 때 효과
    void DamagedEffect(Vector3 dmgPos){
        ParticleSystem effect;
        try{ //장애물이 사라진 뒤 실행하는 오류 대비
            if(ObjectManager.Instance.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Count > 0){
            effect = ObjectManager.Instance.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Pop();
            }
            else {
                effect = Instantiate(ObjectManager.Instance.obstacleEffects.damagedEffectPrefabs[(int)dungeonKind]);
            }
            
            dmgPos.z = (transform.position.z - dmgPos.z)/2 + transform.position.z;
            effect.gameObject.SetActive(true);
            effect.transform.position = dmgPos;
            StartCoroutine(EffectDisappear(effect, 0));
        }
        catch{
        }
    }

    //이펙트를 큐에 보관(0: 피격 이펙트, 1: 파괴 이펙트)
    public IEnumerator EffectDisappear(ParticleSystem effect, int effectKind){
        yield return new WaitForSeconds(1);
        switch(effectKind){
            case 0:
                ObjectManager.Instance.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Push(effect);
                break;
            case 1:
                ObjectManager.Instance.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Push(effect);
                break;

        }
        if(effectKind == 0)
            ObjectManager.Instance.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Push(effect);
        
        effect.gameObject.SetActive(false);
    }


    //받은 데미지를 바탕으로 체력 감소
    void HPDecrese(int dmg){
        curHealthPoint -= dmg;
        if(curHealthPoint <= 0) Death();
    }

    //체력이 0이 되어 파괴
    protected override void Death(){
        meshRenderer.enabled = false;
        obstacleColi.enabled = false;
        CollapseEffect();
    }

    void CollapseEffect(){
        ParticleSystem effect;
        try{ //장애물이 사라진 뒤 실행하는 오류 대비
            if(ObjectManager.Instance.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Count > 0){
            effect = ObjectManager.Instance.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Pop();
            }
            else {
                effect = Instantiate(ObjectManager.Instance.obstacleEffects.collapseEffectPrefabs[(int)dungeonKind]);
            }
            
            effect.gameObject.SetActive(true);
            effect.transform.position = transform.position;
            StartCoroutine(EffectDisappear(effect, 1));
        }
        catch{
        }
    }
}
