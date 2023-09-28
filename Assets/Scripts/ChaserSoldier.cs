using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserSoldier : PlayerBasic
{
    skill[] skills;
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
    }

    private void Start()
    {
        LRInit();
    }

    private void FixedUpdate() {
        Move();
        MoveAnimation();
        Attack(skills); //매개함수 5개
    }

    void NormalAttack(){

    }
}
