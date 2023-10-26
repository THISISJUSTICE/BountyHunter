using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//자유 이동 구현
//어그로 시 이동
//등장, 공격, 사망
public class MonsterBasic : ActivatorBasic
{
    public MonsterStatus monsterStatus; //몬스터 스테이터스
    public int frame; //이동 및 방향 전환의 프레임
    public float turnTime; //방향 전환에 걸리는 시간
    protected bool aggro; //어그로가 끌렸는지 확인
    protected float time; //흐르는 시간
    protected float waitTime; //대기 시간
    protected int curWay; //현재 몬스터가 바라보는 방향(-90: 왼쪽, 180: 정면, 90: 오른쪽)
    
    private void Awake() {
        Init();
    }

    void Init(){
        ActivatorInit();
    }

    private void Start() {
        MonsterBasicInit();
    }

    public void MonsterBasicInit(){
        aggro = false;
        time = 0;
        waitTime = 0;
        curWay = 180;
        meshTransform.rotation = Quaternion.Euler(new Vector3(0, curWay, 0));
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
        int behavior = Random.Range(0, 10);

        if(behavior>=0 && behavior <= 3) {
            //Idle
        }
        else if(behavior > 3 && behavior <= 5) {
            //StartCoroutine(Move(-90)); //왼쪽으로 이동
        }
        else if(behavior > 5 && behavior <= 7) {
            //StartCoroutine(Move(90)); //오른쪽으로 이동
        }
        else{
            //StartCoroutine(Move(-180));//정면으로 이동
        }

        return Random.Range(0.1f, 4); //대기 시간
    }

    //지정된 방향으로 이동
    protected IEnumerator Move(int way){
        if(!ObstacleCheck(way)){
            StartCoroutine(Turn(way));
            int dist;
            
            yield return new WaitForSeconds(0);
        }
        else{ //장애물이 있으면 가만히 대기
            //Idle
        }
    }

    //나아갈 방향에 장애물이 있는지 체크
    protected bool ObstacleCheck(int way){
        Vector3 shotWay;

        if(way == -90) shotWay = Vector3.right * -1; //왼쪽
        else if(way == 90) shotWay = Vector3.right; //오른쪽
        else shotWay = Vector3.forward; //정면
        
        if(Physics.BoxCast(transform.position, transform.lossyScale * gameObject.GetComponent<CapsuleCollider>().radius * 0.9f, 
        shotWay, out RaycastHit hit, transform.rotation, floorVertical)){
            if(hit.collider.tag == "Obstacle" || hit.collider.tag == "Monster"){
                return true;
            }
        }
        return false;
    }

    //이동하려는 방향으로 회전(-90: 왼쪽, 180: 정면, 90: 오른쪽)
    protected IEnumerator Turn(int way){
        if(curWay != way) {
            float turnRot = (float)(curWay - way)/frame;
            
            for(int i=0; i<frame; ++i){
                meshTransform.rotation = Quaternion.Euler(new Vector3(0, meshTransform.rotation.y + turnRot, 0));
                yield return new WaitForSeconds(turnTime/frame);
            }
            meshTransform.rotation = Quaternion.Euler(new Vector3(0, curWay, 0));
        }
        //방향 전환 애니메이션
    }

}
