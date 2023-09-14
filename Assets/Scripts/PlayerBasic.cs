using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBasic : MonoBehaviour
{
    #region Variable
    public float lrPos; //플레이어의 좌우 칸마다의 이동 값
    public float lrMoveDelay; //플레이어의 좌우 이동 딜레이
    public int lrMoveFrame; //플레이어의 좌우 이동 프레임
    public int mapSpace = 5; // 맵의 좌우 칸 수(무조건 홀수)

    [System.Serializable]
    public class PlayerStatus{ //플레이어 스테이터스 클래스
        public int level; //플레이어 레벨
        public float speed; //플레이어 기본 속도
        public float acceleration; //플레이어 돌진 속도
        public int maxHealthPoint; //플레이어 최대 체력
        public int maxMagicPoint; //플레이어 최대 마나
        public int armor; //플레이어 방어력
        public int magicResitant; //플레이어 마법저항력
        public int attackDamage; //플레이어 공격력
        public int magicDamage; //플레이어 마법공격력
    }
    public PlayerStatus playerStatus; //플레이어 스테이터스
    
    protected int curHealthPoint; //플레이어 현재 체력
    protected int curMagicPoint; //플레이어 현재 마나
    protected float curSpeed; //플레이어의 현재 이동 속도
    
    protected float[] lrSpace; //좌우 이동 위치
    protected int lrIndex; //현재 좌우 위치
    protected int curMoveWay; //현재 이동 방향(0: 앞, -1: 좌, 1: 우)
    protected float rushTime; //돌진을 진행한 시간
    protected Animator anim;
    protected Transform meshTransform; //플레이어 메쉬의 트랜스폼

    #endregion

    #region Awake, Start

    private void Awake(){
        PlayerBasicInit(); 
    }

    protected void PlayerBasicInit(){
        lrSpace = new float[mapSpace];
        curHealthPoint = playerStatus.maxHealthPoint;
        curMagicPoint = playerStatus.maxMagicPoint;
        curSpeed = 0;
        anim = transform.GetChild(0).GetComponent<Animator>();
        meshTransform = transform.GetChild(0).GetComponent<Transform>();
        rushTime = 0;
    }

    private void Start(){
        LRInit();
    }

    //좌우 움직임 위치 설정
    protected void LRInit(){
        int temp = lrSpace.Length / 2; 
        float space = 0 - (lrPos * temp);
        lrIndex = temp;
        curMoveWay = 0;

        for(int i=0; i<lrSpace.Length; ++i){
            lrSpace[i] = space;
            space += lrPos;
        }
    }

    #endregion
    
    private void FixedUpdate() {
        Move();
        MoveAnimation();
    }

    #region FixedUpdate

    //↑: 가속, ↓: 멈춤, →: 오른쪽으로 한 칸 이동, ←: 왼쪽으로 한 칸 이동
    protected void Move(){
        float hor = Input.GetAxisRaw("Horizontal");
        int lrTemp = lrIndex + (int)hor;

        //이동 및 가속
        if(!Input.GetKey(KeyCode.DownArrow) && !ObstacleFCheck()){
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
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + curSpeed);

        //좌우 이동
        if(hor != 0 && lrTemp >= 0 && lrTemp < lrSpace.Length && curMoveWay == 0){
            if(!ObstacleLRCheck(lrIndex, lrTemp)){
                StartCoroutine(LRMove(lrIndex, lrTemp));
                lrIndex = lrTemp;
            }            
        }
    }

    //좌우 이동 중 이동하려는 방향에 레이를 쏴서 장애물이 있는지 확인
    protected bool ObstacleLRCheck(int start, int end){
        int way = start > end ? -1 : 1;
        if(Physics.BoxCast(transform.position, transform.lossyScale * gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, 
        transform.right * way, out RaycastHit hit, transform.rotation, lrPos)){
            if(hit.collider.tag == "Obstacle"){
                return true;
            }
        }

        return false;
    }

    //전방이 막혀 있는지 확인
    protected bool ObstacleFCheck(){
        if(Physics.BoxCast(transform.position, transform.lossyScale * gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, 
        transform.forward, out RaycastHit hit, transform.rotation, 0.5f)){
            if((hit.collider.tag == "Obstacle" && rushTime <= 0.8f) || hit.collider.tag == "Background") {
                return true;
            }
        }
        return false;
    }

    //좌우 이동에 약간의 딜레이 설정
    protected IEnumerator LRMove(int start, int end){
        float gap = (lrSpace[end] - lrSpace[start]) / lrMoveFrame; //프레임 당 이동 값
        gameObject.transform.position = new Vector3(lrSpace[lrIndex], gameObject.transform.position.y, gameObject.transform.position.z);
        curMoveWay = start > end ? -1 : 1;
        for(int i=0; i<lrMoveFrame; ++i){
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + gap, gameObject.transform.position.y, gameObject.transform.position.z);
            yield return new WaitForSeconds(lrMoveDelay/lrMoveFrame);
        }
        curMoveWay = 0;
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
    void RotateWay(float animSpeed){
        float rot;
        if(animSpeed < 0.5f) rot = 90;
        else if(animSpeed == 0.5f) rot = 40;
        else rot=25;
        rot*=curMoveWay;
        meshTransform.localRotation = Quaternion.Euler(0, rot, 0);
    }
    
    #endregion

    private void OnCollisionEnter(Collision other) {
        //돌진 중 장애물과 부딪히면 데미지를 받거나 입음
        if(other.gameObject.tag == "Obstacle"){
            if(curSpeed == playerStatus.acceleration && rushTime > 0.8f){
                rushTime = 0;
                ObstacleBasic obstacleBasic = other.transform.GetComponentInParent<ObstacleBasic>();
                obstacleBasic.RushDamaged(playerStatus.armor, playerStatus.acceleration);
                RushDamaged(obstacleBasic.obstacleStatus.armor);
            }
        }
    }

    protected void RushDamaged(int armor){

    }

    protected void HPDecrese(int dmg){

    }
}
