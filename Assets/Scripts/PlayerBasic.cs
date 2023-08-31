using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasic : MonoBehaviour
{
    #region Variable
    public float lrPos; //플레이어의 좌우 칸마다의 이동 값
    public float lrMoveDelay; //플레이어의 좌우 이동 딜레이
    public int mapSpace = 5; // 맵의 좌우 칸 수(무조건 홀수)

    public float speed; //플레이어 기본 속도
    public float acceleration; //플레이어 돌진 속도
    public int health; //플레이어 체력
    public int armor; //플레이어 방어력
    public int magicResitant; //플레이어 마법저항력
    public int attackDamage; //플레이어 공격력
    public int magicDamage; //플레이어 마법공격력
    

    Rigidbody rigid;
    float[] lrSpace; //좌우 이동 위치
    int lrIndex; //현재 좌우 위치
    bool lrMoving; //현재 좌우 이동 중인지 확인

    #endregion

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        lrSpace = new float[mapSpace];
    }

    void Start(){
        LRInit();
    }

    //좌우 움직임 위치 설정
    void LRInit(){
        int temp = lrSpace.Length / 2; 
        float space = 0 - (lrPos * temp);
        lrIndex = temp;
        lrMoving = false;

        for(int i=0; i<lrSpace.Length; ++i){
            lrSpace[i] = space;
            space += lrPos;
        }
    }

    private void FixedUpdate() {
        Move();
    }

    //↑: 가속, ↓: 멈춤, →: 오른쪽으로 한 칸 이동, ←: 왼쪽으로 한 칸 이동
    void Move(){
        float hor = Input.GetAxisRaw("Horizontal");

        //이동 및 가속
        if(!Input.GetKey(KeyCode.DownArrow)){
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + speed);
            if(Input.GetKey(KeyCode.UpArrow)){
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + acceleration);
            }
        }

        //좌우 이동
        if(hor != 0 && lrIndex + hor >= 0 && lrIndex + hor < lrSpace.Length && !lrMoving){
            lrIndex += (int)hor;
            StartCoroutine(LRMove());
        }
    }

    //좌우 이동에 약간의 딜레이 설정
    IEnumerator LRMove(){
        gameObject.transform.position = new Vector3(lrSpace[lrIndex], gameObject.transform.position.y, gameObject.transform.position.z);
        lrMoving = true;
        yield return new WaitForSeconds(lrMoveDelay);
        lrMoving = false;
    }
}
