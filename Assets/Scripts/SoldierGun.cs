using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SoldierGun : MonoBehaviour
{
    public int gunKind; //총의 종류(0: 권총류, 1: 소총, 기관총류, 2: 저격총, 샷건류)
    public float fireRate; //발사 대기 시간
    public float reloadRate; //장전 속도
    public int maxBulletCount; //탄창 당 총알 갯수

    ParticleSystem fireEffect; //발사 이펙트

    Transform gunPos; //발사 방향을 정하기 위한 위치
    public Transform bulletPos; //총알이 나오는 위치
    Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
        fireEffect = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    //발사
    public void Shoot(int bulletKind){
        anim.Play("Shoot");
        fireEffect.Play();
        CreateBullet(bulletKind);
    }

    //총알 생성
    void CreateBullet(int bulletKind){
        GunBullet bullet;
        if(ObjectManager.Instace.playerObjects.bulletObjects[bulletKind].Count == 0){
            bullet = Instantiate(ObjectManager.Instace.playerObjects.bulletPrefabs[bulletKind].gameObject, bulletPos.position, Quaternion.identity).GetComponent<GunBullet>();
        }
        else{
            bullet = ObjectManager.Instace.playerObjects.bulletObjects[bulletKind].Pop();
        }
        Rigidbody bulletRigid = bullet.rigid;
        bulletRigid.AddForce(Vector3.forward * bullet.speed, ForceMode.Impulse);
    }

    //장전
    public void Reload(){

    }

    public void BulletDisappear(GunBullet bullet, int bulletKind){
        ObjectManager.Instace.playerObjects.bulletObjects[bulletKind].Push(bullet);
        bullet.gameObject.SetActive(false);
    }
}
