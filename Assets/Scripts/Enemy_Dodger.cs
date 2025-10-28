using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dodger : Enemy
{
    [SerializeField] private float _dodgeSpeed = 7.0f;
    [SerializeField] private float _dodgeDistance = 2.0f;
    [SerializeField] private float _detectionRadius = 3.0f;
    [SerializeField] private LayerMask _laserLayer;
    [SerializeField] private float _dodgeCooldown = 1.0f;
    private float _lastDodgeTime = -999f;
    private bool _isDodging = false;

    protected override void Update()
    {
        base.Update();

        if (!_isDodging)
        {
            DetectAndDodge();
        }
    }

    private void DetectAndDodge()
    {
        if (Time.time < _lastDodgeTime + _dodgeCooldown) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _laserLayer);

        if (hits.Length > 0)
        {
            StartCoroutine(DodgeRoutine());
            _lastDodgeTime = Time.time;
        }
    }

    private IEnumerator DodgeRoutine()
    {
        _isDodging = true;

        float direction = Random.Range(0, 2) == 0 ? -1f : 1f;
        Vector3 dodgeTarget = transform.position + new Vector3(direction * _dodgeDistance, 0, 0);

        dodgeTarget.x = Mathf.Clamp(dodgeTarget.x, -8f, 8f);

        float elapsed = 0f;
        float dodgeDuration = 0.3f;

        while (elapsed < dodgeDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, dodgeTarget, _dodgeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDodging = false;
    }
}
