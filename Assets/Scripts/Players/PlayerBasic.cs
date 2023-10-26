using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

//ObstacleCheck 함수 통일하기

//플레이어 오브젝트의 기본
public class PlayerBasic : ActivatorBasic
{
    #region Variable
    public float lrMoveDelay; //플레이어의 좌우 이동 딜레이
    public PlayerStatus playerStatus; //플레이어 스테이터스
    public Transform weaponPos; //무기를 잡고 있는 위치
    protected int curMagicPoint; //플레이어 현재 마나
    protected float curSpeed; //플레이어의 현재 이동 속도
    protected float rushTime; //돌진을 진행한 시간
 

    protected delegate void skill(); //플레이어마다 가지고 있는 스킬 매개함수
    protected float slowDown; //슬로우 효과를 받을 때 감소비율
    public bool noMove; //이동 불가
    public bool noLRMove; //좌우 이동 불가

    #endregion

    #region Awake, Start

    private void Awake(){
        PlayerBasicInit(); 
    }

    protected void PlayerBasicInit(){
        ActivatorInit();
        lrIndex = lrSpace.Length / 2;
        curHealthPoint = playerStatus.maxHealthPoint;
        curMagicPoint = playerStatus.maxMagicPoint;
        curSpeed = 0;
        rushTime = 0;
        slowDown = 1;
        noMove = false;
        noLRMove = false;
    }

    private void Start(){
        LRInit();
    }

    #endregion
    
    private void FixedUpdate() {
        Move();
        MoveAnimation();
    }

    #region FixedUpdate

    //↑: 가속, ↓: 멈춤, →: 오른쪽으로 한 칸 이동, ←: 왼쪽으로 한 칸 이동
    protected void Move(){
        if(noMove) return;

        float hor = Input.GetAxisRaw("Horizontal");
        int lrTemp = lrIndex + (int)hor;
        int way = lrIndex > lrTemp ? -1 : 1;

        //이동 및 가속
        if(!Input.GetKey(KeyCode.DownArrow) && !ObstacleCheck(0, gameObject.GetComponent<CapsuleCollider>().radius * 0.5f, 0.5f)){
            curSpeed = playerStatus.speed;
            if(Input.GetKey(KeyCode.UpArrow)){
                curSpeed = playerStatus.acceleration;
                rushTime += 0.02f;
            }
            else rushTime = 0;
        }
        else {
            curSpeed = 0;
            rushTime = 0;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + curSpeed * slowDown);

        if(noLRMove) return;
        //좌우 이동
        if(hor != 0 && lrTemp >= 0 && lrTemp < lrSpace.Length && curMoveWay == 0){
            if(!ObstacleCheck(way, gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, floorHorizontal)){
                StartCoroutine(LRMove(lrIndex, lrTemp, lrMoveDelay));
                lrIndex = lrTemp;
            }            
        }
    }

    // //좌우 이동 중 이동하려는 방향에 레이를 쏴서 장애물이 있는지 확인
    // protected bool ObstacleLRCheck(int start, int end){
    //     int way = start > end ? -1 : 1;
    //     if(Physics.BoxCast(transform.position, transform.lossyScale * gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, 
    //     transform.right * way, out RaycastHit hit, transform.rotation, floorHorizontal)){
    //         if(hit.collider.tag == "Obstacle"){
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    // //전방이 막혀 있는지 확인
    // protected bool ObstacleFCheck(){
    //     if(Physics.BoxCast(transform.position, transform.lossyScale * gameObject.GetComponent<CapsuleCollider>().radius * 0.5f, 
    //     transform.forward, out RaycastHit hit, transform.rotation, 0.5f)){
    //         if((hit.collider.tag == "Obstacle" && rushTime <= 0.8f) || hit.collider.tag == "Background") {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // //좌우 이동에 약간의 딜레이 설정
    // protected IEnumerator LRMove(int start, int end){
    //     float gap = (lrSpace[end] - lrSpace[start]) / lrMoveFrame; //프레임 당 이동 값
    //     gameObject.transform.position = new Vector3(lrSpace[lrIndex], gameObject.transform.position.y, gameObject.transform.position.z);
    //     curMoveWay = start > end ? -1 : 1;
    //     for(int i=0; i<lrMoveFrame; ++i){
    //         gameObject.transform.position = new Vector3(gameObject.transform.position.x + gap, gameObject.transform.position.y, gameObject.transform.position.z);
    //         yield return new WaitForSeconds(lrMoveDelay/lrMoveFrame);
    //     }
    //     curMoveWay = 0;
    // }

    protected override bool TagCheck(string tag, int way)
    {   
        if(tag == "Background" || rushTime <= 0.8f || way != 0){
            return base.TagCheck(tag, way);
        }
        return false;
    }

    //이동 애니메이션
    protected void MoveAnimation(){
        float animSpeed;
        if(curSpeed == 0) animSpeed = 0;
        else if(curSpeed == playerStatus.speed) animSpeed = 0.5f;
        else animSpeed = 1;
        RotateWay(animSpeed);
        if(curMoveWay != 0) animSpeed = 1;
        anim.SetFloat("MoveSpeed", animSpeed);
    }

    //현재 좌우 이동 방향과 속도에 따라 플레이어 메쉬의 각도 변환(90, 45, 25)
    protected void RotateWay(float animSpeed){
        float rot;
        if(animSpeed < 0.5f) rot = 90;
        else if(animSpeed == 0.5f) rot = 40;
        else rot=25;
        rot*=curMoveWay;
        meshTransform.localRotation = Quaternion.Euler(0, rot, 0);
    }

    protected void Attack(skill[] skills){
        if(curMoveWay != 0) return;
        if(Input.GetKey(KeyCode.A)){
            skills[0]();
        }
        if(Input.GetKey(KeyCode.Q)){
            skills[1]();
        }
        if(Input.GetKey(KeyCode.W)){
            skills[2]();
        }
        if(Input.GetKey(KeyCode.E)){
            skills[3]();
        }
        if(Input.GetKey (KeyCode.R)){
            skills[4]();
        }
    }
    
    #endregion

    private void OnCollisionEnter(Collision other) {
        ObstacleCollisionCheck(other);
    }

    protected void ObstacleCollisionCheck(Collision other){
        //돌진 중 장애물과 부딪히면 데미지를 받거나 입음
        if(other.gameObject.tag == "Obstacle"){
            Debug.Log("충돌");
            if(curSpeed == playerStatus.acceleration && rushTime > 0.8f){
                rushTime = 0;
                
                try{ //장애물이 파괴되는 동시에 부딪히는 경우 대비
                    ObstacleBasic obstacleBasic = other.transform.GetComponentInParent<ObstacleBasic>();
                    obstacleBasic.Rushed(playerStatus.armor, playerStatus.acceleration);
                    StartCoroutine(BeSlowed(0.1f, 0.4f));
                    RushDamaged(obstacleBasic.obstacleStatus.armor);
                }
                catch{
                    return;
                }
            }
        }
    }

    //슬로우 효과를 당했을 때
    public IEnumerator BeSlowed(float time, float rate){
        slowDown = rate;
        yield return new WaitForSeconds(time);
        slowDown = 1;
    }

    //돌진 시 입는 데미지
    protected void RushDamaged(int armor){

    }

    //체력 감소
    protected void HPDecrese(int dmg){

    }
}
