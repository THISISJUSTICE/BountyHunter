using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//Nightmare 몬스터 스크립트
public class MonsterNightmare : MonsterBasic
{
    public PlayableDirector clawPlayable;
    
    BoxCollider hitbox; //strikeArea의 Collider(몬스터마다 Collider의 종류가 다를 수 있음)
    
    
    private void Awake() {
        NightmareInit();
    }

    void NightmareInit(){
        Init(); 
        breakAtks = new Action[2];
        basicAtks = new Action[1];
        lethalAtk = null;
        
        breakAtks[0] = () => StartCoroutine(CloseAttacks(clawPlayable, 3.3167f/2, new Vector3(0.5f, 2, 3), new Vector3(6, 4, 5)));
        breakAtks[1] = () => StartCoroutine(HornAttack());
        basicAtks[0] = () => StartCoroutine(BiteAttack());

        strikeArea.TagSetting(new string[] {"Obstacle", "Chaser"}, 0, 3);
        hitbox = strikeArea.GetComponent<BoxCollider>();
    }

    void HitboxSetting(Vector3 pos, Vector3 size){
        strikeArea.transform.localPosition = pos;
        hitbox.size = size;
    }

    //근접 공격
    IEnumerator CloseAttacks(PlayableDirector playable, float cooltime, Vector3 hitboxPos, Vector3 hitboxSize){
        isAttack = true;
        HitboxSetting(hitboxPos, hitboxSize);
        playable.Play();

        yield return new WaitForSeconds(cooltime);

        isAttack = false;
    }

    //할퀴기 공격
    IEnumerator ClawAttack(){
        Debug.Log("ClawAttack");
       
        isAttack = true;
        HitboxSetting(new Vector3(0.5f, 2, 3), new Vector3(6, 4, 5));
        

        strikeArea.AddDmgSetting(1);
        

        yield return new WaitForSeconds(0.01f);
        
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

    private void OnTriggerStay(Collider other) {
        PlayerTriggerEnterCheck(other.gameObject.tag, other.transform);
    }
}
