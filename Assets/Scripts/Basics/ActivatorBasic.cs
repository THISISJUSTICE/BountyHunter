using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//몬스터, 플레이어 등의 움직이는 오브젝트의 기본
public class ActivatorBasic : ObjectsBasic
{
    public int moveFrame; //포지션 및 로테이션 변환 프레임
    protected int mapSpace; // 맵의 좌우 칸 수(무조건 홀수)
    protected float[] lrSpace; //좌우 이동 시 포지션 값
    protected int lrIndex; //현재 좌우 위치
    protected Animator anim; //애니메이터
    protected Transform meshTransform; //오브젝트의 메쉬의 트랜스폼
    protected Rigidbody rigid; //리지드바디

    protected void ActivatorInit(){
        meshTransform = transform.GetChild(0).GetComponent<Transform>();
        anim = meshTransform.GetComponent<Animator>();
    }

    protected void LRInit(){
        mapSpace = GameManager.Inst.curDungeonInfo[0].Count; 
        lrSpace = new float[mapSpace];
        int temp = lrSpace.Length / 2; 
        float space = 0 - (Common.floorHorizontal * temp);
        lrIndex = temp;

        for(int i=0; i<lrSpace.Length; ++i){
            lrSpace[i] = space;
            space += Common.floorHorizontal;
        }
    }

    //이동 방향에 장애물이 있는지 확인(방향, 크기, 길이, 아무것도 감지되지 않을 때)
    protected bool ObstacleCheck(Vector3 end, float rayRadius, float rayLen, bool noHitReturn = false){
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Radar"));
        if(Physics.BoxCast(transform.position, new Vector3(1,1,1) * rayRadius, end, out RaycastHit hit, Quaternion.identity, rayLen, layerMask)){
            if(TagCheck(hit.collider.tag, end)){
                return true;
            }
        }
        else{
            return noHitReturn;
        }
        return false;
    }

    //좌우 이동
    protected virtual IEnumerator LRMove(int start, int end, float delay){
        float gap = 0;

        try{ //배열의 범위를 벗어나는 예외 대비
            gap = (lrSpace[end] - lrSpace[start]) / (float)moveFrame / Mathf.Abs(start - end); //프레임 당 이동 값
        
        }
        catch{
            Debug.Log($"end: {end}, start: {start}");
        }

        for(int i=0; i<moveFrame * Mathf.Abs(start - end); ++i){
            transform.position = new Vector3(transform.position.x + gap, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(delay/(float)moveFrame);
        }
        
    }

    //장애물 확인 함수에서 특정 태그가 맞는지 확인, 일부 자식은 태그와 특정 조건이 있기에 virtual 선언
    protected virtual bool TagCheck(string tag, Vector3 way){
        if(tag == "Obstacle") return true;
        return false;
    }

    #region Override
    
    protected override void DamagedEffect(Vector3 dmgPos){} //자식에서 구현
    protected override int ReturnStatus(string kind){return 0;} //자식에서 구현
    protected override int CalculateDamage(int attackDamage, int magicDamage){return 0;} //자식에서 구현

    //체력이 0이 되어 파괴
    protected override void Death(){
        
    }

    #endregion
}
