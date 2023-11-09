using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

//자유 이동 구현
//어그로 시 이동
//등장, 공격, 사망
//몬스터의 세로 위치는 장애물의 세로 위치에 floorVerticle/2를 더한 값

//몬스터 오브젝트의 기본
public class MonsterBasic : ActivatorBasic
{
    public MonsterStatus monsterStatus; //몬스터 스테이터스
    public float frontLength; //전방 확인을 위한 레이의 길이
    public float turnSpeed; //초당 회전하는 각도
    public int moveRange; //어그로가 끌리지 않는 경우 몬스터가 현재 위치에서 움직일 수 있는 최대 이동 범위
    protected bool aggro; //어그로가 끌렸는지 확인
    protected float time; //흐르는 시간
    protected float waitTime; //대기 시간
    bool isMoving; //현재 움직이는 상태인지 확인
    int curVerCoor; //현재 세로 좌표
    int[] targetCoor; //목표 좌표

    private void Awake() {
        Init();
    }

    void Init(){
        ActivatorInit();
        rigid = GetComponent<Rigidbody>();
        targetCoor = new int[2];
    }

    private void Start() {
        MonsterBasicInit(2, 6, 2, 1);
    }

    public void MonsterBasicInit(int curPos, int x, int z, float height){
        LRInit();
        aggro = false;
        isMoving = false;
        lrIndex = curPos;
        time = 0;
        waitTime = 0;
        transform.rotation = Quaternion.Euler(0,180,0);

        curVerCoor = x;
        lrIndex = z;

        transform.position = new Vector3(floorHorizontal*2 - lrIndex*floorHorizontal, height, 10 + curVerCoor*floorVertical + floorVertical/2);
    }

    private void FixedUpdate() {
        TimeFlow();
    }

    //시간의 흐름
    protected void TimeFlow(){
        if(aggro){ //어그로가 끌렸을 때
            anim.SetFloat("MoveSpeed", 1);
            return;
        }

        time += Time.deltaTime;  

        if(time >= waitTime){
            anim.SetFloat("MoveSpeed", 0);
            isMoving = false;
            time = 0;
            waitTime = Think();
        }

    }

    //어그로가 끌리지 않은 상황에서 다음 행동을 랜덤하게 결정(대기 시간 반환)
    protected float Think(){
        int behavior = Random.Range(0, 5);

        if(behavior < 2) { //Idle
            return Random.Range(1, 4);
        }
        else { //Move
            //랜덤으로 목표 좌표를 설정
            do{
                targetCoor[0] = Random.Range(moveRange * -1, moveRange);
                targetCoor[0] += curVerCoor;
            }while(targetCoor[0] < 2 || targetCoor[0] >= GameManager.Inst.curDungeonInfo.Count());
            do{
                targetCoor[1] = Random.Range(0, GameManager.Inst.curDungeonInfo[0].Count());
            }while(targetCoor[0] == curVerCoor && targetCoor[1] == lrIndex);

            isMoving = true;

            StartCoroutine(GoTarget());
            return 100;
        }
    }

    //목표한 좌표로 이동
    protected IEnumerator GoTarget(){
        int next;

        // Debug.Log($"최초 GoTarget target: [{targetCoor[0]}, {targetCoor[1]}], cur: [{curVerCoor}, {lrIndex}]");

        while(targetCoor[0] != curVerCoor){ //세로 칸이 같을 때까지 반복
            next = SearchRoad();
            
            if(next == -20){
                break;
            }
            else if(next != 0){ //칸이 다르면 가로 이동
                StartCoroutine(HorizontalMove(next));
                yield return new WaitForSeconds(turnSpeed + monsterStatus.speed*Mathf.Abs(next) + 1);                
            }

            //세로 이동---------------------------------------
            if(targetCoor[0] > curVerCoor) {
                StartCoroutine(Turn(0));
                next = 1;
                yield return new WaitForSeconds(turnSpeed + 1);
            }
            else{
                StartCoroutine(Turn(180));
                next = -1;
                yield return new WaitForSeconds(turnSpeed + 1);
            }
            
            StartCoroutine(Move(floorVertical, 1));
            curVerCoor += next;
            yield return new WaitForSeconds(monsterStatus.speed);
        }

        if(targetCoor[1] != lrIndex){
            next = targetCoor[1] - lrIndex;
            StartCoroutine(HorizontalMove(next));
            yield return new WaitForSeconds(turnSpeed + monsterStatus.speed*Mathf.Abs(next) + 1);
        }

        // Debug.Log($"최종 GoTarget target: [{targetCoor[0]}, {targetCoor[1]}], cur: [{curVerCoor}, {lrIndex}], curPos: [{transform.position.x}, {transform.position.z}]");
        waitTime = time + 1;
    }

    //가로 이동
    IEnumerator HorizontalMove(int next){
        StartCoroutine(Turn((float)next/Mathf.Abs(next) * 90 * -1));
        yield return new WaitForSeconds(turnSpeed + 1);

        StartCoroutine(LRMove(mapSpace - lrIndex - 1, mapSpace - lrIndex - next - 1, monsterStatus.speed));
        lrIndex += next;
        yield return new WaitForSeconds(monsterStatus.speed*Mathf.Abs(next));
    }

    //가장 가까운 좌표에 장애물이 있는지 확인
    int SearchRoad(){
        int add = 0;
        if(targetCoor[0] > curVerCoor) add = 1;
        for(int i=0; i<mapSpace; ++i){ //가장 가까운 장애물이 없는 칸 탐색
            if(lrIndex + i < mapSpace &&
            GameManager.Inst.curDungeonInfo[curVerCoor + add][lrIndex+i] == 0){
                return i;
            }
            if(lrIndex - i >= 0 && GameManager.Inst.curDungeonInfo[curVerCoor + add][lrIndex-i] == 0){
                return i*-1;
            }
        }
        return -20; //지나갈 수 없음
    }

    //전방으로 이동(1칸 당 거리, 이동하는 칸)
    protected IEnumerator Move(float dist, int moveFloor){
        float gap = dist*moveFloor/(float)moveFrame;
        float wfs = monsterStatus.speed / (float)moveFrame;

        anim.SetFloat("MoveSpeed", 0.5f);
        for(int i=0; i<moveFrame*moveFloor; ++i){
            transform.Translate(0, 0, gap);
            yield return new WaitForSeconds(wfs);
        }

        anim.SetFloat("MoveSpeed", 0);
    }

    //목표한 각도가 되도록 회전(전후좌우: 0, 180, -90, 90)
    protected IEnumerator Turn(float aimRot){
        float goal = (aimRot + 360) % 360;
        float curRot = (transform.eulerAngles.y+360)%360;

        if(curRot != goal){ //회전할 필요가 있는지 확인
            float angle = goal - curRot;
            float reverseAngle = angle + 360 * (angle / Mathf.Abs(angle) * -1);
            float animSpeed;
            float gap;

            //더 짧은 각도의 회전을 계산
            if(Mathf.Abs(angle) > Mathf.Abs(reverseAngle)){
                angle = reverseAngle;
            }
            
            gap = angle/(float)moveFrame;
            animSpeed = Mathf.Abs(angle)/90 * 0.5f;

            anim.SetFloat("MoveWay", angle / Mathf.Abs(angle) * animSpeed);
            for(int i=0; i<moveFrame; ++i){
                transform.Rotate(0, gap,0);
                yield return new WaitForSeconds(turnSpeed/moveFrame);
            }
            anim.SetFloat("MoveWay", 0);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, goal, transform.eulerAngles.z);  
        }        
    }

    void AggroSetActive(Vector3 target){
        aggro = true;
        StopAllCoroutines();
    }

    #region Override

    protected override bool TagCheck(string tag, Vector3 way)
    {   
        if(tag == "Background") return true;
        return base.TagCheck(tag, way);
    }

    //필요한 스테이터스를 반환
    protected override int ReturnStatus(string kind){
        switch(kind){
            case "armor":
                return monsterStatus.armor;
            case "maxHealthPoint":
                return monsterStatus.maxHealthPoint;
        }
        return 0;
    }

    //받는 피해량 계산
    protected override int CalculateDamage(int attackDamage, int magicDamage){
        int dmg = (int)Mathf.Ceil((float)attackDamage/monsterStatus.armor) + (int)Mathf.Ceil((float)magicDamage/monsterStatus.magicRegistant);
        return dmg * 5;
    }

    protected override IEnumerator LRMove(int start, int end, float delay)
    {
        anim.SetFloat("MoveSpeed", 0.4f);
        StartCoroutine(base.LRMove(start, end, delay));
        yield return new WaitForSeconds(delay * Mathf.Abs(start - end) + 0.1f);
        anim.SetFloat("MoveSpeed", 0);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            Debug.Log("AggroOn");
        }
    }

    //체력이 0이 되어 파괴
    // protected override void Death(){
    //     meshRenderer.enabled = false;
    //     obstacleColi.enabled = false;
    //     CollapseEffect();
    // }

    #endregion
}
