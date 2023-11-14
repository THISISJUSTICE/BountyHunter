using System;
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
    protected float slowDown; //슬로우 효과를 받을 때 감소비율
    protected bool noMove; //이동 불가
    protected bool noLRMove; //좌우 이동 불가

    float curSpeed; //플레이어의 현재 이동 속도
    int curMoveWay; //현재 이동 방향 (왼쪽, 정면, 오른쪽): (플레이어: -1, 0 ,1)
    float rushTime; //돌진을 진행한 시간

    #endregion

    #region Awake, Start 

    private void Awake() {
        ActivatorInit();
    } 
    
    private void Start(){
        PlayerBasicInit(); 
    }
    protected void PlayerBasicInit(){
        LRInit();
        lrIndex = lrSpace.Length / 2;
        curHealthPoint = playerStatus.maxHealthPoint;
        curMagicPoint = playerStatus.maxMagicPoint;
        curSpeed = 0;
        curMoveWay = 0;
        rushTime = 0;
        slowDown = 1;
        noMove = false;
        noLRMove = false;
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
        if(!Input.GetKey(KeyCode.DownArrow) && !ObstacleCheck(Vector3.forward, gameObject.GetComponent<CapsuleCollider>().radius * 0.5f, 0.5f)){
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
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + curSpeed * slowDown);

        if(noLRMove) return;
        //좌우 이동
        if(hor != 0 && lrTemp >= 0 && lrTemp < lrSpace.Length && curMoveWay == 0){
            if(!ObstacleCheck(Vector3.right * way, gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, Common.floorHorizontal)){
                StartCoroutine(LRMove(lrIndex, lrTemp, lrMoveDelay));
                lrIndex = lrTemp;
            }            
        }
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

    //Q, W, E, R, A 버튼을 누름으로서 공격 혹은 스킬 발동
    protected void Attack(Action[] skills){
        if(curMoveWay != 0) return; //좌우 이동 중엔 사용 불가
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

    //돌진 중 장애물과 부딪히면 데미지를 받거나 입음
    protected void ObstacleCollisionCheck(Collision other){
        
        if(other.gameObject.tag == "Obstacle"){
            if(curSpeed == playerStatus.acceleration && rushTime > 0.8f){
                rushTime = 0;
                
                try{ //장애물이 파괴되는 동시에 부딪히는 경우 대비
                    ObstacleBasic obstacleBasic = other.transform.GetComponentInParent<ObstacleBasic>();
                    obstacleBasic.Rushed(playerStatus.armor, playerStatus.acceleration);
                    StartCoroutine(BeSlowed(0.1f, 0.4f)); //부딪히면 약간의 슬로우
                    Rushed(obstacleBasic.obstacleStatus.armor, playerStatus.acceleration); //플레이어도 부딪히면 데미지를 입음
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

    #region Override

    //공격을 받을 때 해당 위치에 피해 이펙트 생성
    protected override void DamagedEffect(Vector3 dmgPos){

    }

    protected override IEnumerator LRMove(int start, int end, float delay)
    {
        curMoveWay = start > end ? -1 : 1;
        StartCoroutine(base.LRMove(start, end, delay));
        yield return new WaitForSeconds(delay * Mathf.Abs(start - end));
        curMoveWay = 0;
    }

    //특정 스테이터스 반환
    protected override int ReturnStatus(string kind){
        switch(kind){
            case "armor":
                return playerStatus.armor;
            case "maxHealthPoint":
                return playerStatus.maxHealthPoint;
        }

        return 0;
    }

    //받는 피해량 계산
    protected override int CalculateDamage(int attackDamage, int magicDamage){
        int dmg = (int)Mathf.Ceil((float)attackDamage/playerStatus.armor) + (int)Mathf.Ceil((float)magicDamage/playerStatus.magicRegistant);
        return dmg * 5;
    }

    //장애물 확인 함수에서 특정 태그가 맞는지, 현재 돌진 중인지 확인
    protected override bool TagCheck(string tag, Vector3 way)
    {   
        if(tag == "Monster" && rushTime <= 0.8f) return true;
        if(tag == "Background" || rushTime <= 0.8f || way != Vector3.forward){
            return base.TagCheck(tag, way);
        }
        return false;
    }

    #endregion
}
