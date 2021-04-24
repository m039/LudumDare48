using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;

public class Rope : MonoBehaviour
{
    const int NumberOfJoints = 100;

    public Socket socketPrefab;

    public Rigidbody2D startRigidBody;

    public Rigidbody2D endRigidBody;

    public LineRenderer lineRednerer;

    public SpriteRenderer startRenderer;

    public SpriteRenderer endRenderer;

    GameObject _linksParent;

    readonly Vector3[] _jointPositions = new Vector3[NumberOfJoints];

    bool _reverse = false;

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
            rigidBody.mass = 1;
            rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = link.AddComponent<CircleCollider2D>();
            collider.radius = lineRednerer.startWidth / 2f;

            var hingeJoint = link.AddComponent<HingeJoint2D>();
            hingeJoint.connectedBody = previousRigidBody;
            hingeJoint.enableCollision = true;
            hingeJoint.autoConfigureConnectedAnchor = false;
            hingeJoint.connectedAnchor = new Vector2(0f, 10f / NumberOfJoints);

            previousRigidBody = rigidBody;
        }

        endRigidBody.GetComponent<HingeJoint2D>().connectedBody = previousRigidBody;
        endRigidBody.transform.position = startRigidBody.transform.position;
        endRigidBody.velocity = Vector2.zero;
        endRigidBody.angularVelocity = 0;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var endRb = _reverse ? startRigidBody : endRigidBody;

            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = (p - endRb.transform.position).normalized;

            endRb.AddForce(direction * 100f, ForceMode2D.Force);

            //endRigidBody.MovePosition(p);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < NumberOfJoints; i++)
        {
            var child = _linksParent.transform.GetChild(i);

            _jointPositions[i] = child.transform.position;
        }

        lineRednerer.positionCount = NumberOfJoints;
        lineRednerer.SetPositions(_jointPositions);

        var endR = _reverse ? startRenderer : endRenderer;
        var starR = _reverse ? endRenderer : startRenderer;

        endR.transform.position = _jointPositions[NumberOfJoints - 1];
        starR.transform.position = _jointPositions[0];
    }

    public void Reconnect(Socket socket)
    {
        var startRb = _reverse? endRigidBody : startRigidBody;
        var startHinge = startRb.GetComponent<HingeJoint2D>();
        var endRb = _reverse? startRigidBody: endRigidBody;
        var endHinge = endRb.GetComponent<HingeJoint2D>();

        endRb.isKinematic = true;
        endRb.transform.position = socket.transform.position;
        endRb.velocity = Vector2.zero;
        endRb.angularVelocity = 0;
        endHinge.enabled = false;

        var previousSocketPosition = startRb.position;

        startRb.isKinematic = false;
        startRb.mass = endRb.mass;
        startHinge.enabled = true;

        var previousRigidBody = endRb;

        System.Action<int> updateChild = (index) =>
        {
            var child = _linksParent.transform.GetChild(index);
            var rigidbody = child.GetComponent<Rigidbody2D>();

            var hingeJoint = child.GetComponent<HingeJoint2D>();
            hingeJoint.connectedBody = previousRigidBody;

            previousRigidBody = rigidbody;
        };

        if (_reverse)
        {
            for (int i = 0; i < NumberOfJoints; i++)
            {
                updateChild(i);
            }
        } else
        {
            for (int i = NumberOfJoints - 1; i >= 0; i--)
            {
                updateChild(i);
            }
        }

        startHinge.connectedBody = previousRigidBody;

        _reverse = !_reverse;

        var newSocket = Instantiate(socketPrefab);
        newSocket.transform.position = previousSocketPosition;
        newSocket.Appear = true;
    }
}
