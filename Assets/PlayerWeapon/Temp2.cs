using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp2 : MonoBehaviour
{
    public GameObject granade;
    public bool isGra;
    public bool isReady;
    public float speed;
    public Transform forPos;
    GameObject granadeClone;

    private void Update()
    {
        
        if (Input.GetButton("Granade")) {

            if (!isGra)
            {
                granadeClone = Instantiate(granade, gameObject.transform.position, gameObject.transform.rotation);
                isGra = true;
                granadeClone.GetComponent<Rigidbody>().mass = 0;
                Debug.Log("1");
            }

            else if (isGra && !isReady)
            {
                Invoke("Ready", 1);
                Debug.Log("2");
            }

            else if (isGra && isReady) {
                granadeClone.GetComponent<Rigidbody>().mass = 1;
                granadeClone.GetComponent<Rigidbody>().velocity = (gameObject.transform.position - forPos.position) * speed;
                isGra = false;
                isReady = false;
                Debug.Log("3");
            }
        }
    }

    void Ready() {
        isReady = true;
        Debug.Log("4");
    }
}
