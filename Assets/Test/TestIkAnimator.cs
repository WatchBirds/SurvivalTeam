using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIkAnimator : MonoBehaviour
{
      public Animator animator;
      private float wantedYRotation;
      private float wantedCameraXRotation;
      public Transform head;
      private Vector3 targetPos;
      public Transform pos;

      // Start is called before the first frame update
      void Start()
    {
            animator = GetComponent<Animator>();
    }

      private void Update()
      {
          
            wantedCameraXRotation -= Input.GetAxis("Mouse Y");
            targetPos =transform.rotation* (head.position +new Vector3(0, wantedCameraXRotation, 2));
            pos.position = targetPos;
      }
      private void OnAnimatorIK(int layerIndex)
      {
            
            animator.SetLookAtPosition(targetPos);
            animator.SetLookAtWeight(1);
      }
}
