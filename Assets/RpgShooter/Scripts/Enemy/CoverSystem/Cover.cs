using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform playerTransform;
    [Header("Cover points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffset = 1;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private float zOffset = 1;

    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform;
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints = {
            new Vector3 (0, yOffset, zOffset), //前
            new Vector3 (0, yOffset, -zOffset), //后
            new Vector3 (xOffset, yOffset, 0), //左
            new Vector3 (-xOffset, yOffset, 0), //右
        };
        foreach (Vector3 v in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(v);
            CoverPoint newCover =
                Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform).GetComponent<CoverPoint>();
            coverPoints.Add(newCover);
        }
    }

    public List<CoverPoint> GetValidCoverPoint(Transform enemyTransform)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();
        foreach (var coverPoint in coverPoints)
        {
            if (IsCoverPointValid(coverPoint, enemyTransform))
                validCoverPoints.Add(coverPoint);
        }
        return validCoverPoints;
    }

    private bool IsCoverPointValid(CoverPoint coverPoint, Transform enemyTransform)
    {
        if (coverPoint.occupied)
            return false;
        if (IsCoverBehindPlayer(coverPoint, enemyTransform))
            return false;
        if (IsCoverCloseToLastCover(coverPoint, enemyTransform))
            return false;
        if (!IsFurtherestFromPlayer(coverPoint))
            return false;
        return true;
    }

    private bool IsFurtherestFromPlayer(CoverPoint coverPoint)
    {
        float maxDistance = 0;
        CoverPoint furtherestCoverPoint = null;
        foreach (var point in coverPoints)
        {
            float curDistance = Vector3.Distance(playerTransform.position, point.transform.position);
            if (curDistance > maxDistance)
            {
                maxDistance = curDistance;
                furtherestCoverPoint = point;
            }
        }
        return furtherestCoverPoint == coverPoint;
    }
    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTransform)
    {
        Vector3 coverToPlayer = playerTransform.position - coverPoint.transform.position;
        coverToPlayer.y = 0;
        Vector3 coverToEnemy = enemyTransform.position - coverPoint.transform.position;
        coverToEnemy.y = 0;
        float angle = Vector3.Angle(coverToEnemy, coverToPlayer);
        // Debug.Log(gameObject.name + angle);
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);
        // Debug.Log(distanceToPlayer < distanceToEnemy);
        //要判断下玩家敌人和避障点角度
        if (distanceToPlayer < distanceToEnemy && angle < 90)
            return true;
        return false;
    }

    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().lastCover;
        if (lastCover == null)
        {
            return false;
        }
        return Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
