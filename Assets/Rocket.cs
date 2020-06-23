using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
  Rigidbody rigidody;
    // Start is called before the first frame update
    void Start()
    {
      rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      ProcessInput();
    }

    private void ProcessInput()
    {
      if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
      {
        rigidbody.force.AddRelativeForce;
      }
      if (Input.GetKey(KeyCode.A))
      {
        print("rotating left");
      }
      else if (Input.GetKey(KeyCode.D))
      {
        print("rotating right");
      }
    }
}
