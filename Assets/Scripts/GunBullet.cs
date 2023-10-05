using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : MonoBehaviour
{
    public int bulletKind; //총알의 종류
    public int damage; //피해량
    public float speed; //총알 이동 속도
    public float existTime; //총알이 존재하는 시간
    public Rigidbody rigid;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    public void BulletInit(){
        StartCoroutine(WaitTime());
    }


    //생성하고 일정 시간 대기
    IEnumerator WaitTime(){
        yield return new WaitForSeconds(existTime);
        Disappear();
    }

    //총알 삭제
    public void Disappear(){
        ObjectManager.Instace.playerObjects.bulletObjects[bulletKind].Push(this);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.tag == "Obstacle"){
            other.transform.GetComponentInParent<ObstacleBasic>().Attacked(damage, 0);
            
        }
        Disappear();
        
    }
}
