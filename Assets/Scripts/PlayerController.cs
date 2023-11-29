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

    public Transform rightGun;
    public Transform leftGun;

    void Update()
    {
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(5, 5, 0), Color.green);
        PlayerMovement();
        PlayerRotation();
        if (Input.GetKeyDown(KeyCode.Space))
        {

            defaultGun.FireGun(rightGun, this);
            defaultGun.FireGun(leftGun, this);
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

    public void Fire(Transform gunPos)
    {    //get gun position
        // fire from gun position
        // y-axis of gun position + movement distance over attack speed
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        Debug.Log("angle: " + angle);
        Debug.Log("mousePos: " + mousePos);
        float angleRad = angle * Mathf.Deg2Rad;
        float terminalPointX;
        float terminalPointY;
        if (gunPos.position.x < 0)
        {
            terminalPointX = (gunPos.position.x - 5f) * Mathf.Cos(angleRad);
        }
        else
        {
            terminalPointX = (gunPos.position.x + 5f) * Mathf.Cos(angleRad);

        }
        if (gunPos.position.y < 0)
        {
            terminalPointY = (gunPos.position.y - 5f) * Mathf.Sin(angleRad);
        }
        else
        {
            terminalPointY = (gunPos.position.y + 5f) * Mathf.Sin(angleRad);

        }



        float distCovered = (Time.time - startTime) * 1f;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;
        GameObject newBullet = Instantiate(bullet, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);

        newBullet.transform.DOMove(new Vector3(terminalPointX, terminalPointY, 0), attackRate).OnComplete(() =>
        {
            Destroy(newBullet);
        });
    }

    // public void TestingFire(Transform gunPos)
    // {
    //     // GameObject newBullet = Instantiate(bullet, new Vector3(gunPos.position.x, gunPos.position.y, 0), Quaternion.identity);
    //     GameObject newBullet = ObjectPool.SharedInstance.GetPooledObject();

    //     if (newBullet != null)
    //     {

    //         newBullet.transform.position = gunPos.transform.position;
    //         newBullet.transform.rotation = gunPos.transform.rotation;
    //         newBullet.SetActive(true);
    //     }
    //     Vector3 bulletDirection = transform.up;
    //     bulletDirection.Normalize();
    //     StartCoroutine(MoveBullet(newBullet, bulletDirection));
    //     // bullet.transform.position += bulletDirection * Time.deltaTime * 10f;
    //     // Debug.Log(bullet.transform.position);
    //     // Destroy(newBullet, 3f); // Destroy the bullet after 2 seconds, adjust as needed.

    // }
    // IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    // {
    //     float startTime = Time.time;
    //     while (Time.time - startTime < 2f) // Adjust the duration as needed.
    //     {
    //         bullet.transform.position += direction * 10f * Time.deltaTime;
    //         yield return null;
    //     }
    //     bullet.SetActive(false);
    // }
}
