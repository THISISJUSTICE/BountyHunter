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
    public IEnumerator Shoot(int bulletKind){
        anim.Play("Shoot");
        fireEffect.Play();
        yield return new WaitForSeconds(0.05f);
        CreateBullet(bulletKind);
    }

    //총알 생성
    void CreateBullet(int bulletKind){
        GunBullet bullet;
        if(ObjectManager.Inst.playerObjects.bulletObjects[bulletKind].Count > 0){
            bullet = ObjectManager.Inst.playerObjects.bulletObjects[bulletKind].Pop();
            bullet.transform.position = bulletPos.position;
        }
        else{
            bullet = Instantiate(ObjectManager.Inst.playerObjects.bulletPrefabs[bulletKind].gameObject, bulletPos.position, Quaternion.identity).GetComponent<GunBullet>();
            bullet.TagSetting(new string[] {"Obstacle", "Monster"});
        }
        bullet.gameObject.SetActive(true);
        Rigidbody bulletRigid = bullet.rigid;
        bullet.BulletInit();
        bulletRigid.velocity = Vector3.zero;
        bulletRigid.velocity = Vector3.forward * bullet.speed;
    }

    //장전
    public void Reload(){

    }

}
