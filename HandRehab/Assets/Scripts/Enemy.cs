﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public GameObject player;
    public GameObject projectile;

    Canvas canvas;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Invoke("Shoot", GenerateNextShotInterval());
        canvas = this.hpBar.GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        canvas.transform.LookAt(player.transform);
    }

    void Shoot() {
        this.transform.LookAt(player.transform);
        GameObject bullet = GameObject.Instantiate(projectile);
        bullet.GetComponent<Renderer>().material.color = this.type.color;
        bullet.GetComponent<Attack>().element = this.type.element;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.position = this.transform.position + this.transform.forward*2;
        rb.AddForce((player.transform.position - this.transform.position) * 150);
        GameObject.Destroy(bullet, 5);
        Invoke("Shoot", GenerateNextShotInterval());
    }

    float GenerateNextShotInterval() {
        return Random.Range(3, 10);
    }
}
