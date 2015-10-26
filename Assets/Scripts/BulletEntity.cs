using UnityEngine;
using System.Collections;

public class BulletEntity : MonoBehaviour {
    public float LifeTime = 4.0f;
    public float Speed = 2.0f;
    public Vector3 Direction = new Vector3(1, 0, 0);

    private Coroutine DelayedDestroyCoroutine;
    private Transform BulletTransform;

    public void Start()
    {
        BulletTransform = GetComponent<Transform>();
        DelayedDestroyCoroutine = StartCoroutine(DelayedDestroy());
    }

    public void OnShoot()
    {
        
    }

    public void Update()
    {
        float step = (Speed / 10.0f) * Time.deltaTime;
        Vector3 target = BulletTransform.position + (step * Direction);

        BulletTransform.position = target;
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(LifeTime);

        Destroy(this.gameObject);
    }

    public void OnHit()
    {
        StopCoroutine(DelayedDestroyCoroutine);

        Destroy(this.gameObject);
    }
}
