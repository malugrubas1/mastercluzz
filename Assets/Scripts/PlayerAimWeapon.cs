using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAimWeapon : MonoBehaviour
{
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }

    private Transform aimTransform;
    public Transform aimGunEndPointTransform;
    public GameObject Bullet;

    public GameObject casingPrefab;
    public Transform casingPosition;
    public GameObject explosionParticlePrefab;
    public Transform bulletBoomPosition;

    public int bulletCount;
    public bool isReloading = false;
    public Text ammoCount;
    public Text Reloading;
    public GameObject ReloadingText;

    public int maxAmmo = 17;  // BEDZIE ZDEFINIOWANE I ZMIENIAC SIE DLA KAZDEJ BRONI (PODANE SA DLA PISTOLETU)
    public float reloadTime = 2f;

   

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        bulletCount = maxAmmo;
    }

    void Update()
    {
        if(UIManager.isPaused == false)
        {
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 aimDirection = (mousePosition - aimTransform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 a = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            a.y = -1f;
        }
        else
        {
            a.y = +1f;
        }
        aimTransform.localScale = a;
            
           
        HandleShooting();
        HandleReload();
        ammoCount.text = bulletCount.ToString();
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10f; 
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && UIManager.isPaused == false && bulletCount > 0 && isReloading == false)
        {
            Instantiate(Bullet, aimGunEndPointTransform.position, transform.rotation);
            Instantiate(casingPrefab, casingPosition.position, transform.rotation);
            Instantiate(explosionParticlePrefab, bulletBoomPosition.position, Quaternion.identity);
            Vector3 mousePosition = GetMouseWorldPosition();
            bulletCount--;

            // ANIMACJA 
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                gunEndPointPosition = aimGunEndPointTransform.position,
                 shootPosition = mousePosition, 
            }); 
        }
    }
    private void HandleReload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            ReloadingText.SetActive(true);
            Invoke("Reload", reloadTime);
        }
    }

    public void Reload()
    {
        bulletCount = maxAmmo;
        isReloading = false;
        ReloadingText.SetActive(false);
    }

}
