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
    }

    //일정 시간이 지나면 총알이 사라짐
    IEnumerator Disappear(){
        yield return new WaitForSeconds(existTime);
        parent.BulletDisappear(this, bulletKind);
    }
}
