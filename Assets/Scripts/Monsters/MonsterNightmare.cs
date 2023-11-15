using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Nightmare 몬스터 스크립트
public class MonsterNightmare : MonsterBasic
{
    public GameObject[] claws; //ClawAttack에 사용할 손톱 이펙트
    
    BoxCollider hitbox; //strikeArea의 Collider(몬스터마다 Collider의 종류가 다를 수 있음)
    
    private void Awake() {
        NightmareInit();
    }

    void NightmareInit(){
        Init(); 
        breakAtks = new Action[2];
        basicAtks = new Action[1];
        lethalAtk = null;
        
        breakAtks[0] = () => StartCoroutine(ClawAttack());
        breakAtks[1] = () => StartCoroutine(HornAttack());
        basicAtks[0] = () => StartCoroutine(BiteAttack());

        strikeArea.TagSetting(new string[] {"Obstacle", "Chaser"}, 0, 3);
        hitbox = strikeArea.GetComponent<BoxCollider>();
    }

    void HitboxSetting(Vector3 pos, Vector3 size){
        strikeArea.transform.localPosition = pos;
        hitbox.size = size;
    }

    public float ftime;
    public float stime;

    IEnumerator CloseAttacks(string animName, float time1, Action action1, float time2, Action action2){
        anim.Play(animName);
        isAttack = true;

        yield return new WaitForSeconds(time1);
        action1();

        yield return new WaitForSeconds(0.01f);
        strikeArea.gameObject.SetActive(true);

        yield return new WaitForSeconds(time2);
        action2();

        isAttack = false;
    }

    //할퀴기 공격
    IEnumerator ClawAttack(){
        Debug.Log("ClawAttack");
        anim.Play("ClawAttack");
        isAttack = true;

        // yield return new WaitForSeconds(0.055f);
        yield return new WaitForSeconds(ftime);

        for(int i=0; i<3; ++i) claws[i].SetActive(true);
        strikeArea.AddDmgSetting(1);
        HitboxSetting(new Vector3(0.5f, 2, 3), new Vector3(6, 4, 5));

        yield return new WaitForSeconds(0.01f);
        strikeArea.gameObject.SetActive(true);

        // yield return new WaitForSeconds(0.064f);
        yield return new WaitForSeconds(stime);

        for(int i=0; i<3; ++i) claws[i].SetActive(false);
        strikeArea.gameObject.SetActive(false);
        
        isAttack = false;
    }

    //박치기 공격
    IEnumerator HornAttack(){
        Debug.Log("HornAttack");
        yield return null;
    }

    //깨물기 공격
    IEnumerator BiteAttack(){
        Debug.Log("BiteAttack");
        yield return null;
    }

    private void Start() {
        MonsterBasicInit(2, 6, 2, 1);
    }

    private void FixedUpdate() {
        TimeFlow();
    }

    private void OnTriggerEnter(Collider other) {
        PlayerTriggerEnterCheck(other.gameObject.tag, other.transform);
    }

    public override void MonsterBasicInit(int curPos, int x, int z, float height)
    {
        base.MonsterBasicInit(curPos, x, z, height);
        for(int i=0; i<3; ++i){
            claws[i].SetActive(false);
        }
    }
}
