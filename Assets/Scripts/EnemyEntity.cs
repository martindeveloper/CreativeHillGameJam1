using UnityEngine;
using System.Collections;

public class EnemyEntity : MonoBehaviour {
    public bool IsMoving = true;
    public bool IsShooting = false;
    public Vector2 ShootDelayInterval = new Vector2(3, 10);
    public GameObject BulletEntity;
    public float Speed = 2.0f;
    public Vector3 MoveOffset = Vector3.zero;
    public Vector3 MoveOffsetLock = Vector3.zero;

    private Transform EnemyTransform;
    private Vector3 TargetStartPosition;
    private Vector3 TargetEndPosition;
    private Vector3 ActiveTarget;
    private bool IsFacingRight = true;
    private bool IsShootInQueue = false;

    public void Start()
    {
        EnemyTransform = GetComponent<Transform>();

        Vector3 axisLock = Vector3.Scale(MoveOffsetLock, MoveOffset);

        TargetStartPosition = EnemyTransform.position + MoveOffset;
        TargetEndPosition = EnemyTransform.position - MoveOffset + axisLock;

        ActiveTarget = TargetStartPosition;
    }

    public void Update()
    {
        if(IsMoving)
        {
            float step = Speed * Time.deltaTime;
            EnemyTransform.position = Vector3.MoveTowards(EnemyTransform.position, ActiveTarget, step);

            if(EnemyTransform.position == ActiveTarget)
            {
                // Swap target
                ActiveTarget = (ActiveTarget == TargetStartPosition) ? TargetEndPosition : TargetStartPosition;

                // Flip sprite
                FlipEnemySprite();
            }
        }

        if(IsShooting && IsShootInQueue == false)
        {
            StartCoroutine(DelayedShoot());
        }
    }

    private IEnumerator DelayedShoot()
    {
        IsShootInQueue = true;

        yield return new WaitForSeconds(Random.Range(ShootDelayInterval.x, ShootDelayInterval.y));

        Debug.Log(string.Format("{0} is shooting", this.name));

        GameObject bullet = Instantiate(BulletEntity, EnemyTransform.position, Quaternion.identity) as GameObject;

        if(bullet != null)
        {
            BulletEntity bulletObject = bullet.GetComponent<BulletEntity>();
            bulletObject.Direction = (IsFacingRight) ? Vector3.right : Vector3.left;
            bulletObject.OnShoot();
        }

        IsShootInQueue = false;
    }

    private void FlipEnemySprite()
    {
        IsFacingRight = !IsFacingRight;

        Vector3 playerScale = EnemyTransform.localScale;
        playerScale.x *= -1.0f;

        EnemyTransform.localScale = playerScale;
    }
}
