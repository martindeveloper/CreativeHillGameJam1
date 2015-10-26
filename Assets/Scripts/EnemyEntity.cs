using UnityEngine;
using System.Collections;

public class EnemyEntity : MonoBehaviour {
    public bool IsMoving = true;
    public float Speed = 2.0f;
    public Vector3 MoveOffset = Vector3.zero;

    private Transform EnemyTransform;
    private Vector3 TargetStartPosition;
    private Vector3 TargetEndPosition;
    private Vector3 ActiveTarget;
    private bool IsFacingRight = true;

    public void Start()
    {
        EnemyTransform = GetComponent<Transform>();

        TargetStartPosition = EnemyTransform.position + MoveOffset;
        TargetEndPosition = EnemyTransform.position - MoveOffset;

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
    }

    private void FlipEnemySprite()
    {
        IsFacingRight = !IsFacingRight;

        Vector3 playerScale = EnemyTransform.localScale;
        playerScale.x *= -1.0f;

        EnemyTransform.localScale = playerScale;
    }
}
