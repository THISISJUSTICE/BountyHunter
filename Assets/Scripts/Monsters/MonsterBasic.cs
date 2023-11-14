using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random; //System, UnityEngine 사이의 Random 함수의 구분

//몬스터 오브젝트의 기본
public class MonsterBasic : ActivatorBasic
{
    public MonsterStatus monsterStatus; //몬스터 스테이터스
    public float frontLength; //전방 확인을 위한 레이의 길이
    public float turnSpeed; //초당 회전하는 각도
    public int moveRange; //어그로가 끌리지 않는 경우 몬스터가 현재 위치에서 움직일 수 있는 최대 이동 범위
    public float aggroTime; //어그로 지속 시간(플레이어가 시야 거리에 없을 때 어그로가 풀릴 때까지 걸리는 시간)

    protected bool aggro; //어그로가 끌렸는지 확인
    protected float time; //흐르는 시간
    protected float waitTime; //대기 시간
    protected PlayerBasic playerBasic; //어그로 끌린 플레이어
    protected delegate IEnumerator CoroutineDelegate();
    protected Action[] breakAtks; //장애물 등을 부술 때 공격
    protected Action[] basicAtks; //플레이어를 공격
    protected Action lethalAtk; //각 몬스터 별 필살기(없을 경우 null)
    protected CapsuleCollider closeRange; //근접 공격 범위
    protected bool isBreakAtk; //다음 행동이 breakAtk인지 확인
    protected bool isAttack; //현재 공격 중인지 확인

    int curVerCoor; //현재 세로 좌표
    int[] targetCoor; //목표 좌표
    bool isTurn; //현재 회전 중인지 확인
    Vector3 frontPos; //오브젝트 기준의 전방 위치

    // private void Awake() {
    //     Init();
    // }

    protected void Init(){
        ActivatorInit();
        rigid = GetComponent<Rigidbody>();
        targetCoor = new int[2];
        closeRange = transform.GetChild(2).GetComponent<CapsuleCollider>();
    }

    // private void Start() {
    //     MonsterBasicInit(2, 6, 2, 1);
    // }

    public virtual void MonsterBasicInit(int curPos, int x, int z, float height){
        LRInit();
        aggro = false;
        isTurn = false;
        isBreakAtk = false;
        lrIndex = curPos;
        time = 0;
        waitTime = 0;
        transform.rotation = Quaternion.Euler(0,180,0);

        curVerCoor = x;
        lrIndex = z;

        //설정한 좌표 값에 따라 포지션 값 설정
        float posx = Common.floorHorizontal*(mapSpace / 2) - lrIndex*Common.floorHorizontal;
        float posz = 10 + curVerCoor*Common.floorVertical + Common.floorVertical/2;
        transform.position = new Vector3(posx, height, posz);
    }

    // private void FixedUpdate() {
    //     TimeFlow();
    // }

    //시간의 흐름
    protected void TimeFlow(){
        time += Time.deltaTime; 

        if(aggro){ //어그로가 끌렸을 때
            if(!isAttack) //현재 공격 중이 아닐 때 움직이기
                OnAggro();
            return;
        }

        if(time >= waitTime){
            time = 0;
            anim.SetFloat("MoveSpeed", 0);
            waitTime = Think();
        }

    }

    #region MonsterAI

    //어그로가 끌리지 않은 상황에서 다음 행동을 랜덤하게 결정(대기 시간 반환)
    float Think(){
        int behavior = Random.Range(0, 5);

        if(behavior < 2) { //Idle
            float randTime = Random.Range(1, 4);

            // float posx = floorHorizontal*(mapSpace / 2) - lrIndex*floorHorizontal;
            // float posz = 10 + curVerCoor*floorVertical + floorVertical/2;
            // transform.position = new Vector3(posx, transform.position.y, posz);
            StopAllCoroutines();
            StartCoroutine(FixCoordinate(randTime));
            return randTime;
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

            StartCoroutine(GoTarget());
            return 100;
        }
    }

    //좌표에 맞는 포지션 값으로 조정
    IEnumerator FixCoordinate(float time){
        float frame = 5.0f;
        float posx = Common.floorHorizontal*(mapSpace / 2) - lrIndex*Common.floorHorizontal;
        float posz = 10 + curVerCoor*Common.floorVertical + Common.floorVertical/2;
        float gapx = (posx - transform.position.x) / frame;
        float gapz = (posz - transform.position.z) / frame;

        for(int i=0; i<(int)frame; ++i){
            transform.position = new Vector3(transform.position.x + gapx, transform.position.y, transform.position.z + gapz);    
            yield return new WaitForSeconds(time/frame);
        }
        transform.position = new Vector3(posx, transform.position.y, posz);
    }

    //목표한 좌표로 이동
    IEnumerator GoTarget(){
        int next;

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
            
            StartCoroutine(Move(Common.floorVertical));
            curVerCoor += next;
            yield return new WaitForSeconds(monsterStatus.speed);
        }

        if(targetCoor[1] != lrIndex){
            next = targetCoor[1] - lrIndex;
            StartCoroutine(HorizontalMove(next));
            yield return new WaitForSeconds(turnSpeed + monsterStatus.speed*Mathf.Abs(next) + 1);
        }

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

    //전방으로 이동(1칸 당 거리)
    IEnumerator Move(float dist){
        float gap = dist/(float)moveFrame;
        float wfs = monsterStatus.speed / (float)moveFrame;

        anim.SetFloat("MoveSpeed", 0.5f);
        for(int i=0; i<moveFrame; ++i){
            transform.Translate(0, 0, gap);
            yield return new WaitForSeconds(wfs);
        }

        anim.SetFloat("MoveSpeed", 0);
    }

    //목표한 각도가 되도록 회전(전후좌우: 0, 180, -90, 90)
    IEnumerator Turn(float aimRot){ 
        float goal = (aimRot + 360) % 360;
        float curRot = (transform.eulerAngles.y+360)%360;

        if(curRot != goal && !isTurn){ //회전할 필요가 있는지 확인
            isTurn = true;

            float angle = goal - curRot;
            float reverseAngle = angle + 360 * (angle / Mathf.Abs(angle) * -1);
            float animSpeed;
            float gap;

            //더 짧은 각도의 회전을 계산
            if(Mathf.Abs(angle) > Mathf.Abs(reverseAngle)){
                angle = reverseAngle;
            }
            
            gap = angle/(float)moveFrame;
            if(Mathf.Abs(angle) > 120) animSpeed = 1;
            else animSpeed = 0.75f;

            anim.SetFloat("MoveWay", angle / Mathf.Abs(angle) * animSpeed);
            for(int i=0; i<moveFrame; ++i){
                transform.Rotate(0, gap,0);
                yield return new WaitForSeconds(turnSpeed/moveFrame);
            }
            anim.SetFloat("MoveWay", 0);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, goal, transform.eulerAngles.z);  
            isTurn = false;
        }        
    }

    #endregion

    // private void OnTriggerEnter(Collider other) {
    //     PlayerTriggerEnterCheck(other.gameObject.tag, other.transform);
    // }

    private void OnTriggerExit(Collider other) {
        
    }

    #region Aggro

    //몬스터의 감지 범위에 플레이어가 들어왔는지 확인(어그로 상태가 아닌 경우)
    protected void PlayerTriggerEnterCheck(string tag, Transform target){
        if(aggro) return;
        if(tag == "Chaser" || tag == "Player"){
            //목표 사이에 장애물이 없는지 한번 더 확인
            if(ObstacleCheck(target.position - transform.position, 0.1f, Vector3.Distance(target.position, transform.position), true)){
                AggroSetActive(target);
            }            
        }
    }

    //어그로 활성화(모든 진행중인 코루틴 중단)
    protected void AggroSetActive(Transform target){
        Debug.Log("어그로 활성화");
        aggro = true;
        isTurn = false;
        StopAllCoroutines();
        playerBasic = target.GetComponent<PlayerBasic>();
        anim.SetFloat("MoveWay", 0);
        anim.SetFloat("MoveSpeed", 0);
    }

    //어그로 비활성화
    IEnumerator AggroSetDeactive(Transform target){
        yield return new WaitForSeconds(aggroTime);
        aggro = false;
        waitTime = 0;
        playerBasic = null;
    }

    //어그로 상태일 때의 행동
    void OnAggro(){
        if(playerBasic != null){
            Quaternion targetRot = Quaternion.LookRotation(playerBasic.transform.position - transform.position);
            StartCoroutine(Turn(targetRot.eulerAngles.y));
            if(Mathf.Abs(transform.eulerAngles.y - targetRot.eulerAngles.y) < 10){
                if(ObstacleCheck(frontPos - transform.position, 0.2f, 6)){
                    Attack();
                }
                else{
                    transform.Translate(0,0,monsterStatus.acceleration);
                    anim.SetFloat("MoveSpeed", 1);
                }
            }
        }
    }

    protected void Attack(){
        //1/5의 확률로 필살기 발동
        if(lethalAtk != null){
            int rand = Random.Range(0, 10);
            if(rand == 0) lethalAtk();
            return;
        }

        //각 공격 함수 중 랜덤으로 선택하여 실행
        if(isBreakAtk){
            
            int rand = Random.Range(0, breakAtks.Length);
            breakAtks[rand]();
        }
        else{
            
            int rand = Random.Range(0, breakAtks.Length + basicAtks.Length);
            if(rand < breakAtks.Length) breakAtks[rand]();
            else{
                rand -= breakAtks.Length;
                basicAtks[rand]();
            }
        }
    }

    #endregion

    #region Override

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

    protected override bool TagCheck(string tag, Vector3 way)
    {
        if(aggro){
            if(tag == "Chaser" || tag == "Player"){
                isBreakAtk = false;
                return true;
            }
            else if(tag == "Obstacle"){
                isBreakAtk = true;
                return true;
            }
        }
        else if(tag == "Chaser" || tag == "Player"){
            return true;
        }
        return false;
    }



    //체력이 0이 되어 파괴
    // protected override void Death(){
    //     meshRenderer.enabled = false;
    //     obstacleColi.enabled = false;
    //     CollapseEffect();
    // }

    #endregion
}
