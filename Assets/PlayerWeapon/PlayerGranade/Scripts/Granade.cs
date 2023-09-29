using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public int damage;
    Animator anim;
    bool isPullPin;
    bool isDoing;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("PullPin", false);
        isPullPin = false;
        isDoing = false;
    }

    private void OnEnable()
    {
        anim.SetBool("PullPin", false);
        isPullPin = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Granade") && isPullPin && !isDoing)
        {
            anim.SetTrigger("Explosion");
            isPullPin = false;
            anim.SetBool("PullPin", false);
            isDoing = true;
            return;
        }

        if (Input.GetButton("Granade") && !anim.GetBool("PullPin") && !isDoing) {
            anim.SetBool("PullPin", true);
            Invoke("SetPullPin", 1);
            isDoing = false;
        }
    }

    void SetPullPin() {
        isPullPin = true;
    }
}
