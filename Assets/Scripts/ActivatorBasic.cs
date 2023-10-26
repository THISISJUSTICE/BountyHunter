using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//몬스터, 플레이어 등의 움직이는 오브젝트의 기본
public class ActivatorBasic : ObjectsBasic
{
    public int lrMoveFrame; //플레이어의 좌우 이동 프레임
    public int mapSpace = 5; // 맵의 좌우 칸 수(무조건 홀수)
    protected float[] lrSpace; //좌우 이동 위치
    protected int lrIndex; //현재 좌우 위치
    protected int curMoveWay; //현재 이동 방향 (왼쪽, 정면, 오른쪽): (플레이어: -1, 0 ,1) (몬스터: -90, 180, 90)
    protected float floorHorizontal; //가로 1칸 당 길이
    protected float floorVertical; //세로 1칸 당 길이
    protected Animator anim;
    protected Transform meshTransform; //오브젝트의 메쉬의 트랜스폼

    protected void ActivatorInit(){
        meshTransform = transform.GetChild(0).GetComponent<Transform>();
        anim = meshTransform.GetComponent<Animator>();
        Common common = new Common();
        floorHorizontal = common.floorHorizontal;
        floorVertical = common.floorVertical;
        lrSpace = new float[mapSpace];
        LRInit();
    }

    protected void LRInit(){
        int temp = lrSpace.Length / 2; 
        float space = 0 - (floorHorizontal * temp);
        lrIndex = temp;
        curMoveWay = 0;

        for(int i=0; i<lrSpace.Length; ++i){
            lrSpace[i] = space;
            space += floorHorizontal;
        }
    }

    //이동 방향에 장애물이 있는지 확인
    protected bool ObstacleCheck(int way, float rayRadius, float rayLen){
        Vector3 rayWay; //레이의 방향

        if(way == -1) rayWay = Vector3.right * -1; //왼쪽
        else if(way == 1) rayWay = Vector3.right; //오른쪽
        else rayWay = Vector3.forward; //정면
        
        if(Physics.BoxCast(transform.position, transform.lossyScale * rayRadius, rayWay, out RaycastHit hit, transform.rotation, rayLen)){
            if(TagCheck(hit.collider.tag, way)){
                return true;
            }
        }
        return false;
    }

    //장애물 확인 함수에서 특정 태그가 맞는지 확인, 일부 자식은 태그와 특정 조건이 있기에 virtual 선언
    protected virtual bool TagCheck(string tag, int way){
        if(tag == "Obstacle" || tag == "Monster" || tag == "Background") return true;
        return false;
    }

    protected void Test(){
        Say();
    }
    protected virtual void Say(){
        Debug.Log("Activator Say");
    }

    protected IEnumerator LRMove(int start, int end, float delay){
        float gap = (lrSpace[end] - lrSpace[start]) / lrMoveFrame; //프레임 당 이동 값
        gameObject.transform.position = new Vector3(lrSpace[lrIndex], gameObject.transform.position.y, gameObject.transform.position.z);
        curMoveWay = start > end ? -1 : 1;
        for(int i=0; i<lrMoveFrame; ++i){
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + gap, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return new WaitForSeconds(delay/lrMoveFrame);
        }
        curMoveWay = 0;
    }

    //체력이 0이 되어 파괴
    protected override void Death(){
        
    }
}
