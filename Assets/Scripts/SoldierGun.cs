using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SoldierGun : MonoBehaviour
{
    public float fireRate; //발사 속도
    public float reloadRate; //장전 속도
    public int maxBulletCount; //탄창 당 총알 갯수

    public GameObject fireEffect; //발사 이펙트
    public GameObject bullet; //총알 오브젝트

    Transform gunPos; //발사 방향을 정하기 위한 위치
    Transform bulletPos; //총알이 나오는 위치
    Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }


    //발사
    public void Shoot(){
        //발사 이펙트
        //총알 생성
    }

    //장전
    public void Reload(){

    }
}
