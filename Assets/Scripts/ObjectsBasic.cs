using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//던전에 존재하는 모든 오브젝트의 기본
public abstract class ObjectsBasic : MonoBehaviour
{
    protected int curHealthPoint; //현재 체력
    protected int ownArmor; //해당 오브젝트의 방어력

    //돌진을 받을 시
    public void Rushed(int armor, float acceleration){
        HPDecrese(RushDamaged(ownArmor, armor, acceleration));
    }

    //돌진 시 받는 데미지 계산
    public int RushDamaged(int myArmor, int matchArmor, float acceleration){
        if(myArmor / 2 < matchArmor){
            return matchArmor/myArmor * (int)(acceleration *20);
        }
        return 0;
    }

    // //피해를 받을 때 실행(오브젝트의 위치 반환)
    // public void Attacked(int attackDamage, int magicDamage, Vector3 dmgPos){
    //     int dmg = obstacleStatus.CalculateDamage(attackDamage, magicDamage);
    //     if(dmg > 0){
    //         HPDecrese(dmg);
            
    //         if(dmg > obstacleStatus.maxHealthPoint / 50) DamagedEffect(dmgPos);
    //     }
    // }

    //받은 데미지를 바탕으로 체력 감소
    void HPDecrese(int dmg){
        curHealthPoint -= dmg;
        if(curHealthPoint <= 0) Death();
    }

    protected abstract void Death();

}
