using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage; //피해량
    public float speed; //총알 이동 속도
    public float existTime; //총알이 존재하는 시간

    private void OnEnable() {
       
    }

    IEnumerator Disappear(){
        yield return new WaitForSeconds(existTime);
        gameObject.SetActive(false);
    }

}
