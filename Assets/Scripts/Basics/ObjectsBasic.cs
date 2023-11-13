using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//던전 내에 존재하는 모든 오브젝트의 기본
public abstract class ObjectsBasic : MonoBehaviour
{
    protected int curHealthPoint; //현재 체력

    //피해를 받을 때 실행(오브젝트의 위치 반환)
    public void Attacked(int attackDamage, int magicDamage, Vector3 dmgPos){
        int dmg = CalculateDamage(attackDamage, magicDamage);
        int hp = ReturnStatus("maxHealthPoint");
        if(dmg > 0){
            HPDecrese(dmg);
            
            if(dmg > hp / 50) DamagedEffect(dmgPos);
        }
    }

    //받은 데미지를 바탕으로 체력 감소
    void HPDecrese(int dmg){
        curHealthPoint -= dmg;
        if(curHealthPoint <= 0) Death();
    }

    //돌진 시 받는 데미지 계산
    int RushDamaged(int myArmor, int matchArmor, float acceleration){
        if(myArmor / 2 < matchArmor){
            return matchArmor/myArmor * (int)(acceleration *20);
        }
        return 0;
    }

    //돌진을 받을 시
    public virtual void Rushed(int matchArmor, float acceleration){
        int myArmor = ReturnStatus("armor");
        HPDecrese(RushDamaged(myArmor, matchArmor, acceleration));
    }

    //공격을 받을 때 해당 위치에 피해 이펙트 생성 -> virtual로 변경 예정
    protected abstract void DamagedEffect(Vector3 dmgPos);

    //필요한 스테이터스를 반환
    protected abstract int ReturnStatus(string kind);

    //받는 피해량 계산
    protected abstract int CalculateDamage(int attackDamage, int magicDamage);

    //체력이 0이 되어 죽었을 때 실행
    protected abstract void Death();

}
