using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variables

    public int movementSpeed = 10;
    public int jumpHeight = 3;
    public int sensitivity = 3;

    Vector3 dir;
    Vector3 movement;

    Rigidbody rigid;

    #endregion

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // MOVEMENT
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        movement = dir.normalized * movementSpeed;

        rigid.MovePosition(rigid.position + transform.TransformDirection(movement) * Time.deltaTime);

        // JUMPING
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rigid.AddForce(Vector3.up * jumpHeight * 2, ForceMode.Impulse);
        }
    }

    void Update()
    {
        // MOUSE INPUT
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0));

        Camera.main.transform.Rotate(new Vector3(Input.GetAxisRaw("Mouse Y") * -sensitivity, 0, 0));
    }

    bool IsGrounded()
    {
        if (Physics.Raycast(rigid.position, Vector3.down, transform.localScale.y + 0.2f))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
