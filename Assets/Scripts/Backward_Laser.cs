using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backward_Laser : Laser
{
    void Start()
    {
        _isEnemyLaser = true;
    }

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 8f)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
