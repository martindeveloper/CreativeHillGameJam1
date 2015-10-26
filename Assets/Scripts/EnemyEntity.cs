using UnityEngine;
using System.Collections;

public class EnemyEntity : MonoBehaviour {
    public bool IsMoving = true;
    public bool IsUsingGroundCheck = false;
    public float Speed = 2.0f;
    public Vector3 MoveOffset = Vector3.zero;
    public Vector3 MoveOffsetLock = Vector3.zero;
    public bool IsShooting = false;
    public Vector2 ShootDelayInterval = new Vector2(3, 10);
    public Sprite DeadStateSprite;

    private Transform EnemyTransform;
    private Rigidbody2D EnemyRigidbody;
    private Vector3 TargetStartPosition;
    private Vector3 TargetEndPosition;
    private Vector3 ActiveTarget;
    private bool IsFacingRight = true;
    private bool IsShootInQueue = false;
    private const float FloorCheckOffset = 1.4f;
    private bool IsDead = false;
    private GameManager Game;
    
    public void Start()
    {
        EnemyTransform = GetComponent<Transform>();
        EnemyRigidbody = GetComponent<Rigidbody2D>();

        Vector3 axisLock = Vector3.Scale(MoveOffsetLock, MoveOffset);

        TargetStartPosition = EnemyTransform.position + MoveOffset;
        TargetEndPosition = EnemyTransform.position - MoveOffset + axisLock;

        ActiveTarget = TargetStartPosition;

        Game = GameManager.Instance;
    }

    public void Update()
    {
        if (IsMoving)
        {
            float step = Speed * Time.deltaTime;

            if(IsUsingGroundCheck)
            {
                // Check if we are on border of platform
                const float rayLength = 1.0f;
                Vector2 raycastPosition = EnemyTransform.position;
                raycastPosition.x += (IsFacingRight) ? FloorCheckOffset : -(FloorCheckOffset);

                RaycastHit2D[] raycastResults = { new RaycastHit2D() };

                Physics2D.RaycastNonAlloc(raycastPosition, Vector2.down, raycastResults, rayLength);

                RaycastHit2D rayHit = raycastResults[0];

                if (!rayHit)
                {
                    SwapMoveTarget();
                }

                if (Application.isEditor)
                {
                    Vector2 debugDirection = Vector2.down * rayLength;
                    Debug.DrawRay(raycastPosition, debugDirection, Color.red);
                }
                
                // Endless move
                Vector3 target = EnemyTransform.position + ((IsFacingRight) ? Vector3.right : Vector3.left * 2.0f);
                Vector3 stepVector = Vector3.MoveTowards(EnemyTransform.position, target, step);

                // Move to using rb
                EnemyRigidbody.MovePosition(stepVector);
            }
            else
            {
                Vector3 stepVector = Vector3.MoveTowards(EnemyTransform.position, ActiveTarget, step);

                // Move to using rb
                EnemyRigidbody.MovePosition(stepVector);

                if (Vector3.SqrMagnitude(EnemyTransform.position - ActiveTarget) < 0.0001)
                {
                    SwapMoveTarget();
                }
            }
        }

        if(IsShooting && IsShootInQueue == false)
        {
            StartCoroutine(DelayedShoot());
        }
    }

    private void SwapMoveTarget()
    {
        // Swap target
        ActiveTarget = (ActiveTarget == TargetStartPosition) ? TargetEndPosition : TargetStartPosition;

        // Flip sprite
        FlipEnemySprite();
    }

    public void OnDeath()
    {
        if(DeadStateSprite != null)
        {
            IsMoving = false;
            IsShooting = false;
            IsUsingGroundCheck = false;

            Transform childSprite = EnemyTransform.Find("Sprite");

            if(childSprite != null)
            {
                // Destroy animator on child sprite
                Destroy(childSprite.GetComponent<Animator>());

                // Set dead sprite
                childSprite.GetComponent<SpriteRenderer>().sprite = DeadStateSprite;
            }

            // Stop all coroutines - eg. shooting
            StopAllCoroutines();

            // Simulate ragdoll effect
            EnemyRigidbody.isKinematic = false;
            GetComponent<BoxCollider2D>().sharedMaterial = Game.Settings.DeadEntityPhysicsMaterial;

            // Disable collision with player and etc.
            this.gameObject.layer = LayerMask.NameToLayer("DeadEntities");
        }
        else
        {
            Destroy(this.gameObject);
        }

        IsDead = true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsDead)
        {
            // Ignore all collisions
            return;
        }

        switch (collider.gameObject.tag)
        {
            case TagsStructure.PlayerBullet:
                BulletEntity bullet = collider.gameObject.GetComponent<BulletEntity>();

                bullet.OnHit();

                OnDeath();
                break;
        }
    }

    private IEnumerator DelayedShoot()
    {
        IsShootInQueue = true;

        yield return new WaitForSeconds(Random.Range(ShootDelayInterval.x, ShootDelayInterval.y));

        Debug.Log(string.Format("{0} is shooting", this.name));

        GameObject bullet = Instantiate(Game.Settings.EnemyBulletEntity, EnemyTransform.position, Quaternion.identity) as GameObject;

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
