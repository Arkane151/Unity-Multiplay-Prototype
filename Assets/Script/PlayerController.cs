using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public PlayerInput playerInput;
    CharacterController characterController;   
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = playerInput.actions["Movement"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x,0, input.y);
        move = move.x * Camera.main.transform.right + move.z * Camera.main.transform.forward;
        move.y = 0;
        characterController.Move(move * Time.deltaTime * 3);
    }
}

