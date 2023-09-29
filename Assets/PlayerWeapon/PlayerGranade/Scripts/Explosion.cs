using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject superGranage;

    private void OnEnable()
    {
        Destroy(superGranage, 2.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Plyaer Hit!");
            //superGranage.GetComponent<Granade>().damage  //수류탄에 입력된 데미지를 받음. 받은 데미지 값을 부딪힌 플레이어에게 적용
        }
    }
}
