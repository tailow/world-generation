using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour 
{

    #region Variables

    public float movementSpeed = 2f;

    public float sensitivity = 4f;

    #endregion

    void Update()
    {
        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Height"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed / 10f);
        transform.Rotate(new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Roll") / 5f) * sensitivity);
    }
}
