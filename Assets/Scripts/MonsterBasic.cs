using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//자유 이동 구현
//어그로 시 이동
//등장, 공격, 사망
public class MonsterBasic : MonoBehaviour
{
    public MonsterStatus monsterStatus; //몬스터 스테이터스
    public float floorHorizontal; //가로 1칸 당 길이
    public float floorVertical; //세로 1칸 당 길이
    protected bool aggro; //어그로가 끌렸는지 확인
    protected float time; //흐르는 시간
    protected float waitTime; //대기 시간
    protected Transform meshModel; //하위의 모델 오브젝트
    protected Animator anim;
    
    private void Awake() {
        Init();
    }

    void Init(){
        Common common = new Common();
        floorHorizontal = common.floorHorizontal;
        floorVertical = common.floorVertical;
        meshModel = transform.GetChild(0).GetComponent<Transform>();
        anim = meshModel.GetComponent<Animator>();
    }

    private void Start() {
        MonsterBasicInit();
    }

    public void MonsterBasicInit(){
        aggro = false;
        time = 0;
        waitTime = 0;
    }

    private void FixedUpdate() {
        TimeFlow();
    }

    //시간의 흐름
    protected void TimeFlow(){
        time += Time.deltaTime;
        
        if(time >= waitTime){
            time = 0;
            if(!aggro)
                waitTime = Think();
        }
    }

    //어그로가 끌리지 않은 상황에서 다음 행동 실행
    protected float Think(){
        

        return Random.Range(0.1f, 4); //대기 시간
    }

}
