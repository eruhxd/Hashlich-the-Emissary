﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Transform weaponMuzzle;

    [Header("General")]
    public LayerMask hittableLayers;
    public GameObject bulletHolePrefab;

    [Header("Shoot Paramaters")]
    public float fireRange = 200;
    public float recoilForce = 4f; // Fuerza de retroceso del arma

    [Header("Sounds & Visuals")]
    public GameObject flashEffect;

    private Transform cameraPlayerTransform;
    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = this.transform.localPosition;
        cameraPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        HandleShoot();

        transform.localPosition = Vector3.Lerp(initialPosition, Vector3.zero, Time.deltaTime * 5f);
    }

    private void HandleShoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // Instanciar el destello de disparo
            GameObject flashClone = Instantiate(flashEffect, weaponMuzzle.position, Quaternion.Euler(weaponMuzzle.forward), transform);
            Destroy(flashClone, 1f);

            AddRecoil();

            RaycastHit hit;
            if (Physics.Raycast(cameraPlayerTransform.position, cameraPlayerTransform.forward, out hit, fireRange, hittableLayers))
            {
                // Instanciar agujero de bala
                GameObject bulletHoleClone = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHoleClone, 4f);

                // Verificar si el objeto alcanzado es un enemigo
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Llamar a un método de daño en el enemigo
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(10); // Asumimos que 10 es el daño
                    }
                }
            }
        }
    }

    private void AddRecoil()
    {
        transform.Rotate(-recoilForce, 0f, 0f);
        transform.position = transform.position - transform.forward * (recoilForce / 50f);
    }
}