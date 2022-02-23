using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
      private void OnCollisionEnter(Collision collision)
      {
            Debug.Log(collision.gameObject.name);
      }
      private void OnTriggerEnter(Collider other)
      {
            Debug.Log("trigger: " + other.name);
      }
}
