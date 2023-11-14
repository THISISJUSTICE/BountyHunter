using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Nightmare 몬스터 스크립트
public class MonsterNightmare : MonsterBasic
{
    public GameObject[] claws; //ClawAttack에 사용할 손톱 이펙트
    
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
    }

    //할퀴기 공격
    IEnumerator ClawAttack(){
        Debug.Log("ClawAttack");
        anim.Play("ClawAttack");
        isAttack = true;

        yield return new WaitForSeconds(0.055f);

        for(int i=0; i<3; ++i) claws[i].SetActive(true);

        yield return new WaitForSeconds(0.065f);

        for(int i=0; i<3; ++i) claws[i].SetActive(false);
        
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
