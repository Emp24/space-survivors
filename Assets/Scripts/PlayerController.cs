using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;



public class PlayerController : MonoBehaviour
{
    [SerializeField] FireStrategy defaultGun;
    [SerializeField] FireStrategy rocketGun;
    public GameObject player;
    public GameObject bullet;
    private float attackRate = 0.2f;
    public Vector2 speed = new(5, 5);
    public Projectile projectile;
    public Transform rightGun;
    public Transform leftGun;

    void Update()
    {
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 0), Color.green);
        PlayerMovement();
        PlayerRotation();
        if (Input.GetKeyDown(KeyCode.Space))
        {

            defaultGun.FireGun(rightGun, this.gameObject);
            defaultGun.FireGun(leftGun, this.gameObject);
        }
    }
    private float startTime;
    private float journeyLength;
    void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(new Vector3(-0.01f, -0.68f, 0), new Vector3(50, 50, 0));
    }

    public void PlayerMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(speed.x * inputX, speed.y * inputY, 0);

        movement *= Time.deltaTime;

        transform.Translate(movement);
    }

    public void PlayerRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }


}
