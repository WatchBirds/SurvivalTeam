using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RigibodyTest : MonoBehaviour
{
      public GameObject obj;
      public float force = 1;
      private void FixedUpdate()
      {
            if (Input.GetMouseButtonDown(0))
            {
                  Rigidbody rigidbody = Instantiate(obj, transform.position, transform.rotation).GetComponent<Rigidbody>();
                  rigidbody.AddForce(rigidbody.transform.forward*force, ForceMode.Impulse); 
            }
      }
}
