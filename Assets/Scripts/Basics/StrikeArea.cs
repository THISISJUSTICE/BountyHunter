using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//공격 스킬을 사용할 때 범위 내에 데미지 적용
public class StrikeArea : MonoBehaviour
{
    [SerializeField]
    private int damage; //피해량(고정적으로 적용되는 최소 피해량)(인스펙터 상에서만 수정)
    

    protected string[] tags; //확인할 태그

    int addDmg; //추가 피해량
    int extraDmg = 1; //특정 태그가 맞으면 추가적인 피해량 증가
    int extraDmgVal = 1; //extraDmg의 피해 증가 수치
    int extraIndex; //특정 태그의 인덱스가 맞는지 확인
    

    //확인할 태그를 결정
    public void TagSetting(string[] tags, int extraIndex = -1, int extraDmgVal = 1){
        this.tags = tags;
        this.extraIndex = extraIndex;
        this.extraDmgVal = extraDmgVal;
    }

    //추가 피해량 설정
    public void AddDmgSetting(int dmg){
        addDmg = dmg;
    }

    private void OnCollisionEnter(Collision other) {
        if(CheckTag(other.transform.tag)){
            AttackObject(other, transform.position);
            
        }        
    }

    protected bool CheckTag(string tag){
        for(int i=0; i<tags.Length; i++){
            if(tag == tags[i]) {
                if(i == extraIndex) extraDmg = extraDmgVal;
                return true;
            }
        }
        return false;
    }

    protected void AttackObject(Collision other, Vector3 dmgPos){
        ObjectsBasic target;
        target =  other.transform.GetComponentInParent<ObjectsBasic>();
        if(target == null) target.transform.GetComponent<ObjectsBasic>();
        Debug.Log($"other.tag: {other.transform.tag}, obstacle.tag: {target.transform.tag}, dmg: {(damage + addDmg) * extraDmg}");
        target.Attacked((damage + addDmg) * extraDmg, 0, dmgPos);
        extraDmg = 1;
    }
}
