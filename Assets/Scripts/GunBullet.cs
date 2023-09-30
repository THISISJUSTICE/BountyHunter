using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : MonoBehaviour
{
    public int bulletKind; //총알의 종류
    public int damage; //피해량
    public float speed; //총알 이동 속도
    public float existTime; //총알이 존재하는 시간

    SoldierGun parent; //총알을 발사한 총
    public Rigidbody rigid;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    public void BulletInit(SoldierGun parent){
        this.parent = parent;
        StartCoroutine(WaitTime());
    }


    //생성하고 일정 시간 대기
    IEnumerator WaitTime(){
        yield return new WaitForSeconds(existTime);
        Disappear();
    }

    //총알 삭제
    public void Disappear(){
        parent.BulletDisappear(this, bulletKind);
    }
}
