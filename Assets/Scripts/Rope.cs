using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;

public class Rope : MonoBehaviour
{
    const int NumberOfJoints = 100;

    public Rigidbody2D startRigidBody;

    public Rigidbody2D endRigidBody;

    public LineRenderer lineRednerer;

    public SpriteRenderer endRenderer;

    GameObject _linksParent;

    readonly Vector3[] _jointPositions = new Vector3[NumberOfJoints];

    readonly Vector2[] _polygonPoints = new Vector2[4];

    void Start()
    {
        _linksParent = new GameObject("Link Parent".Decorate());
        _linksParent.transform.SetParent(transform);

        var previousRigidBody = startRigidBody;

        for (int i = 0; i < NumberOfJoints; i++)
        {
            var link = new GameObject("Link " + i);
            link.transform.SetParent(_linksParent.transform);
            link.layer = 3;

            var rigidBody = link.AddComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;
            rigidBody.mass = 10.5f;

            var collider = link.AddComponent<CircleCollider2D>();
            collider.radius = lineRednerer.startWidth / 2f;


            //if (i != 0)
            //{
            //    link.AddComponent<PolygonCollider2D>();
            //}

            var hingeJoint = link.AddComponent<HingeJoint2D>();
            hingeJoint.connectedBody = previousRigidBody;
            hingeJoint.enableCollision = true;
            hingeJoint.autoConfigureConnectedAnchor = false;
            hingeJoint.connectedAnchor = new Vector2(0f, 2f / NumberOfJoints);

            previousRigidBody = rigidBody;
        }

        endRigidBody.GetComponent<HingeJoint2D>().connectedBody = previousRigidBody;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endRigidBody.MovePosition(p);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < NumberOfJoints; i++)
        {
            var child = _linksParent.transform.GetChild(i);

            _jointPositions[i] = child.transform.position;

            //if (i != 0)
            //{
            //    var currentPoint = (Vector2)_jointPositions[i];
            //    var previousPoint = (Vector2)_jointPositions[i - 1];
            //    var direction = (currentPoint - previousPoint).normalized;
            //    var size = lineRednerer.startWidth / 2;

            //    var p1 = previousPoint + Vector2.Perpendicular(direction) * size;
            //    var p2 = previousPoint - Vector2.Perpendicular(direction) * size;
            //    var p3 = currentPoint + Vector2.Perpendicular(direction) * size;
            //    var p4 = currentPoint - Vector2.Perpendicular(direction) * size;

            //    var collider = child.GetComponent<PolygonCollider2D>();
            //    _polygonPoints[0] = child.transform.InverseTransformPoint(p1);
            //    _polygonPoints[1] = child.transform.InverseTransformPoint(p3);
            //    _polygonPoints[2] = child.transform.InverseTransformPoint(p4);
            //    _polygonPoints[3] = child.transform.InverseTransformPoint(p2);
            //    collider.points = _polygonPoints;
            //}

        }

        lineRednerer.positionCount = NumberOfJoints;
        lineRednerer.SetPositions(_jointPositions);

        endRenderer.transform.position = _jointPositions[NumberOfJoints - 1];
    }
}
