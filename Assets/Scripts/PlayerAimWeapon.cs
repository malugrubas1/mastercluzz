using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
    }

    void Update()
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
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10f; 
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(Bullet, aimGunEndPointTransform.position, transform.rotation);
            Vector3 mousePosition = GetMouseWorldPosition();
            // ANIMACJA 
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                gunEndPointPosition = aimGunEndPointTransform.position,
                 shootPosition = mousePosition, 
            }); 
        }
    }
}
