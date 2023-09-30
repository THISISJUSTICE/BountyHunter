using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//탄창 갯수, 수류탄 갯수, 보유한 총
public class ChaserSoldier : PlayerBasic
{
    skill[] skills;
    int curWeapon; //현재 들고 있는 무기를 확인(0: Q(권총), 1: W(소총, 기관총), 2: E(저격총, 샷건), 3: R(수류탄, 폭탄))
    SoldierGun[] havingWeapons; //현재 가지고 있는 무기

    private void Awake() {
        PlayerBasicInit();
        SkillInit();
    }

    void SkillInit(){
        skills = new skill[5];
        skills[0] = new skill(NormalAttack);
        skills[1] = new skill(NormalAttack);
        skills[2] = new skill(NormalAttack);
        skills[3] = new skill(NormalAttack);
        skills[4] = new skill(NormalAttack);
        havingWeapons = new SoldierGun[5];
    }

    private void Start()
    {
        LRInit();
    }

    void SoldierInit(){
        havingWeapons[0] = weaponPos.GetChild(0).GetChild(0).GetComponent<SoldierGun>();
        havingWeapons[1] = weaponPos.GetChild(1).GetChild(0).GetComponent<SoldierGun>();
        havingWeapons[2] = weaponPos.GetChild(2).GetChild(0).GetComponent<SoldierGun>();
        havingWeapons[3] = weaponPos.GetChild(3).GetChild(0).GetComponent<SoldierGun>();
        havingWeapons[4] = weaponPos.GetChild(4).GetChild(0).GetComponent<SoldierGun>();
    }

    private void FixedUpdate() {
        Move();
        MoveAnimation();
        Attack(skills); //매개함수 5개
    }

    //기본 공격(현재 들고 있는 무기에 따라 공격이 달라짐)
    void NormalAttack(){

    }

    //무기 변경(현재 무기와 같은 경우 장전)
    void ChangeWeapon(int weaponKind){
        if(weaponKind != 3 && curWeapon == weaponKind) {
            GunReload();
            return;
        }
        switch(weaponKind){
            case 0: //권총
                break;
            case 1: //소총, 기관총
                break;
            case 2: //저격총, 샷건
                break;
            case 3: //수류탄
                break;
        }
        curWeapon = weaponKind;
    }

    //탄창 장전
    void GunReload(){

    }
}
