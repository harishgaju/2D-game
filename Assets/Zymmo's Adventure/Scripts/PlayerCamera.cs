using System;
using UnityEngine;

//public class PlayerCamera : MonoBehaviour
//{
//    [SerializeField] private float DeltaX = 2f;
//    [SerializeField] private float DeltaY = 2f;
//    [SerializeField] private float DeltaZ = 10f;

//    private void Awake()
//    {
//        PlayerController Controller = GetComponent<PlayerController>();

//        Controller./*OnPlayerUpdate*/ += UpdatePosition;
//    }

//    private void UpdatePosition()
//    {
//        Camera MainCamera = Camera.main;

//        MainCamera.transform.position = new Vector3(
//            transform.position.x + DeltaX,
//            transform.position.y + DeltaY,
//            transform.position.z - DeltaZ
//            );
//    }
//}
