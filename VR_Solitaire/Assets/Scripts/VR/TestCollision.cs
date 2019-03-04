using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //print("fixed: " + Time.fixedUnscaledDeltaTime + ", max: " + Time.maximumDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        print("aaa");
        print(collision.collider.name);
    }

    void OnTriggerEnter(Collider col)
    {
        print("t enter");
        print(col.name);
    }

    void OnTriggerStay(Collider other)
    {
        print("t stay");
        print(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        print("t exit");
        print(other.name);
    }
}
