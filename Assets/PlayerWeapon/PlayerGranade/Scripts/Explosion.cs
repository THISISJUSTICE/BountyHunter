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
            //superGranage.GetComponent<Granade>().damage  //����ź�� �Էµ� �������� ����. ���� ������ ���� �ε��� �÷��̾�� ����
        }
    }
}
