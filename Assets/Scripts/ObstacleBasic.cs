using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

//장애물 오브젝트의 기본
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
    public bool appearing; //현재 등장하고 있는지 확인
    int[] curCoor; //던전 내에서의 좌표
    bool isAppear; //장애물이 등장했는지 확인
    BoxCollider obstacleColi; //장애물의 충돌

    MeshRenderer meshRenderer;
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        obstacleColi = transform.GetChild(0).GetComponent<BoxCollider>();
        curCoor = new int[2];
        
    }

    public void ObstacleBasicInit(DefineObstacles.Data deObData, Vector3 appearPos, DungeonKind dungeonKind, int x, int y){
        isAppear = false;
        meshRenderer.enabled = false;
        obstacleColi.enabled = false;
        this.appearPos = appearPos;
        this.dungeonKind = dungeonKind;
        curCoor[0] = x;
        curCoor[1] = y;

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
        appearing = false;
        GameManager.Inst.curDungeonInfo[curCoor[0]][curCoor[1]] = 1;
        yield return new WaitForSeconds(appearWaitTime);
        meshRenderer.enabled = true;
        appearing = true;
        for(int i=0; i<appearFrame; ++i){
            transform.position += (appearPos - waitPos) / appearFrame;
            if(i==appearFrame-appearFrame/3) obstacleColi.enabled = true;
            yield return new WaitForSeconds(appearSpeed);
        } 
        appearing = true;
    }

    //이펙트를 큐에 보관(0: 피격 이펙트, 1: 파괴 이펙트)
    public IEnumerator EffectDisappear(ParticleSystem effect, int effectKind){
        yield return new WaitForSeconds(1);
        switch(effectKind){
            case 0:
                ObjectManager.Inst.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Push(effect);
                break;
            case 1:
                ObjectManager.Inst.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Push(effect);
                break;

        }
        if(effectKind == 0)
            ObjectManager.Inst.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Push(effect);
        
        effect.gameObject.SetActive(false);
    }

    
    //파괴되었을 때 이펙트 생성
    void CollapseEffect(){
        ParticleSystem effect;
        try{ //장애물이 사라진 뒤 실행하는 오류 대비
            if(ObjectManager.Inst.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Count > 0){
            effect = ObjectManager.Inst.obstacleEffects.collapseEffectObjects[(int)dungeonKind].Pop();
            }
            else {
                effect = Instantiate(ObjectManager.Inst.obstacleEffects.collapseEffectPrefabs[(int)dungeonKind]);
            }
            
            effect.gameObject.SetActive(true);
            effect.transform.position = transform.position;
            StartCoroutine(EffectDisappear(effect, 1));
        }
        catch{
        }
    }

    #region Override

    //특정 스테이터스 반환
    protected override int ReturnStatus(string kind){
        switch(kind){
            case "armor":
                return obstacleStatus.armor;
            case "maxHealthPoint":
                return obstacleStatus.maxHealthPoint;
        }

        return 0;
    }

    //받는 피해량 계산
    protected override int CalculateDamage(int attackDamage, int magicDamage){
        int dmg = (int)Mathf.Ceil((float)attackDamage/obstacleStatus.armor) + (int)Mathf.Ceil((float)magicDamage/obstacleStatus.magicRegistant);
        return dmg * 5;
    }

    //피해를 입었을 때 효과
    protected override void DamagedEffect(Vector3 dmgPos){
        ParticleSystem effect;
        try{ //장애물이 사라진 뒤 실행하는 오류 대비
            if(ObjectManager.Inst.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Count > 0){
            effect = ObjectManager.Inst.obstacleEffects.damagedEffectObjects[(int)dungeonKind].Pop();
            }
            else {
                effect = Instantiate(ObjectManager.Inst.obstacleEffects.damagedEffectPrefabs[(int)dungeonKind]);
            }
            
            dmgPos.z = (transform.position.z - dmgPos.z)/2 + transform.position.z;
            effect.gameObject.SetActive(true);
            effect.transform.position = dmgPos;
            StartCoroutine(EffectDisappear(effect, 0));
        }
        catch{
        }
    }

    //체력이 0이 되어 파괴
    protected override void Death(){
        meshRenderer.enabled = false;
        obstacleColi.enabled = false;
        GameManager.Inst.curDungeonInfo[curCoor[0]][curCoor[1]] = 0;
        CollapseEffect();
    }

    #endregion
}
