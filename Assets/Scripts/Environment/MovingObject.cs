using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MovingObject : MonoBehaviour
{
    public enum MovementType {Circle, BackForthLine, Random};

    public MovementType movementType;

    public List<Transform> points = new List<Transform>();

    [Range(1, 15)] public int MovementSpeed = 10;
    [Range(0,150)] public int WaitAtPoint = 0;

    int WaitFrames = 0;
    int WaitCooldown = 0;

    List<Vector2> pointPositions = new List<Vector2>();

    Vector2 StartPos = Vector2.zero;
    Vector2 target = Vector2.zero;

    bool goingInReverse = false;

    int targetID;

    void Start()
    {
        pointPositions.Clear();
        StartPos = transform.position;
        pointPositions.Add(StartPos);

        foreach(Transform point in points)
        {
            pointPositions.Add(new Vector2((int)Math.Round(point.position.x), (int)Math.Round(point.position.y)));
        }
        targetID = 1;
        target = pointPositions[targetID];
    }

    void FixedUpdate()
    {
        if(GetComponent<Enemy>() && !GetComponent<Enemy>().LogicEnabled) return;

        if(WaitFrames > 0)
        {
            print(WaitFrames);
            WaitFrames--;
            return;
        }

        if(WaitCooldown > 0)
        {
            WaitCooldown--;
        }

        
        if(new Vector2((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y)) != new Vector2((int)Math.Round(target.x), (int)Math.Round(target.y)))
        {
            Vector2 dir = (target - (Vector2)transform.position).normalized;
            transform.Translate(dir * MovementSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if(movementType == MovementType.Random)
            {
                targetID = UnityEngine.Random.Range(0, pointPositions.Count);
            }

            else if(targetID - 1 >= 0 && goingInReverse)
            {
                targetID--;
            }
            else if(targetID + 1 <= pointPositions.Count - 1 && !goingInReverse)
            {
                targetID++;
            }
            else
            {
                if(movementType == MovementType.Circle)
                {
                    targetID = -1;
                }
                else
                {
                    goingInReverse = !goingInReverse;
                }
                return;
            }
            target = pointPositions[targetID];
            if(WaitCooldown <= 0)
            {
                WaitFrames = WaitAtPoint;
                WaitCooldown = 10;
            }
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.violet;

        if(points == null) return;

        pointPositions.Clear();

        if(StartPos != Vector2.zero) pointPositions.Add(StartPos);
        else pointPositions.Add(transform.position);

        foreach(Transform point in points)
        {
            pointPositions.Add(point.position);
        }

        foreach (var pointPos in pointPositions)
        {
            if(movementType == MovementType.Random)
            {
                Gizmos.DrawSphere(pointPos, 0.3f);
                continue;
            }
            Gizmos.DrawSphere(pointPos, 0.1f);
        }

        if(points.Count < 1 || movementType == MovementType.Random) return;

        for (int i = 0; i < pointPositions.Count - 1; i++)
        {
            Gizmos.DrawLine(pointPositions[i], pointPositions[i + 1]);
        }

        if(movementType == MovementType.Circle)
        {
            Gizmos.DrawLine(pointPositions[pointPositions.Count - 1], StartPos);
        }
    }
}
