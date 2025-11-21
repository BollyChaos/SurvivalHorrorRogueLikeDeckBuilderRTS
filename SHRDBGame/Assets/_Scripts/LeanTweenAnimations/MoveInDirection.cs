using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInDirection : MonoBehaviour
{
    
 public Vector3 direction = Vector3.forward;
public float speed = 5f;

void Update()
{
    transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
}

}
