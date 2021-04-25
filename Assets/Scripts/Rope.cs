using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;
using System;

public class Rope : MonoBehaviour
{
    static int _sNumberOfLinks = 0;

    const int NumberOfJoints = 100;

    const int NumberToElongate = 50;

    const float DefaultMass = 0.01f;

    const float Force = 20.1f;

    float AnchorWidth = 10f / NumberOfJoints;

    public Socket socketPrefab;

    public Rigidbody2D startRigidBody;

    public Rigidbody2D endRigidBody;

    public LineRenderer lineRednerer;

    public SpriteRenderer startRenderer;

    public SpriteRenderer endRenderer;

    GameObject _linksParent;

    Vector3[] _jointPositions;

    bool _reverse = false;

    readonly List<GameObject> _links = new List<GameObject>();

    void Start()
    {
        _linksParent = new GameObject("Link Parent".Decorate());
        _linksParent.transform.SetParent(transform);

        var previousRigidBody = startRigidBody;

        for (int i = 0; i < NumberOfJoints; i++)
        {
            var link = CreateLink("Link ", previousRigidBody);

            _links.Add(link);

            previousRigidBody = link.GetComponent<Rigidbody2D>();
        }

        endRigidBody.GetComponent<HingeJoint2D>().connectedBody = previousRigidBody;
        endRigidBody.transform.position = startRigidBody.transform.position;
        endRigidBody.velocity = Vector2.zero;
        endRigidBody.angularVelocity = 0;

        endRigidBody.mass = startRigidBody.mass = DefaultMass;
    }

    GameObject CreateLink(string name, Rigidbody2D connectedBody)
    {
        var link = new GameObject(name + " " + _sNumberOfLinks++);
        link.transform.SetParent(_linksParent.transform);
        link.layer = 3;

        var rigidBody = link.AddComponent<Rigidbody2D>();
        rigidBody.gravityScale = 0;
        rigidBody.angularDrag = 1f;
        rigidBody.drag = 3f;
        rigidBody.mass = DefaultMass;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        //rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;

        var collider = link.AddComponent<CircleCollider2D>();
        collider.radius = lineRednerer.startWidth / 2f;

        var joint = link.AddComponent<HingeJoint2D>();
        joint.connectedBody = connectedBody;
        joint.enableCollision = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = new Vector2(0f, AnchorWidth);
        return link;
    }

    void FixedUpdate()
    {
        // Add force on mouse button down.

        if (Input.GetMouseButton(0))
        {
            var endRb = _reverse ? startRigidBody : endRigidBody;

            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = (p - endRb.transform.position).normalized;

#if true
            endRb.AddForce(direction * Force, ForceMode2D.Force);
#else
            endRb.MovePosition(p);
#endif
        }

        // Update line renderer.

        if (_jointPositions == null || _jointPositions.Length != _links.Count)
        {
            _jointPositions = new Vector3[_links.Count];
        }

        for (int i = 0; i < _links.Count; i++)
        {
            _jointPositions[i] = _links[i].transform.position;
        }

        lineRednerer.positionCount = _jointPositions.Length;
        lineRednerer.SetPositions(_jointPositions);

        var endR = _reverse ? startRenderer : endRenderer;
        var starR = _reverse ? endRenderer : startRenderer;

        endR.transform.position = _jointPositions[_jointPositions.Length - 1];
        starR.transform.position = _jointPositions[0];
    }

    public void Reconnect(Socket socket)
    {
        var startRb = _reverse? endRigidBody : startRigidBody;
        var startHinge = startRb.GetComponent<Joint2D>();
        var endRb = _reverse? startRigidBody: endRigidBody;
        var endHinge = endRb.GetComponent<Joint2D>();

        endRb.isKinematic = true;
        endRb.transform.position = socket.transform.position;
        endRb.velocity = Vector2.zero;
        endRb.angularVelocity = 0;
        endHinge.enabled = false;

        var previousSocketPosition = startRb.position;

        startRb.isKinematic = false;
        startRb.mass = endRb.mass;
        startRb.velocity = Vector2.zero;
        startRb.angularVelocity = 0;
        startHinge.enabled = true;

        var previousRigidBody = endRb;

        System.Action<int> updateChild = (index) =>
        {
            var child = _links[index];
            var rigidbody = child.GetComponent<Rigidbody2D>();

            var hingeJoint = child.GetComponent<Joint2D>();
            hingeJoint.connectedBody = previousRigidBody;

            previousRigidBody = rigidbody;
        };

        if (_reverse)
        {
            for (int i = 0; i < _links.Count; i++)
            {
                updateChild(i);
            }
        } else
        {
            for (int i = _links.Count - 1; i >= 0; i--)
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

    public Vector2 GetCenterOfMass()
    {
        return (startRigidBody.transform.position + endRigidBody.transform.position) / 2;
    }

    public void Elongate()
    {
        if (!_reverse)
        {
            var previousRigidBody = _links[_links.Count - 1].GetComponent<Rigidbody2D>();

            for (int i = 0; i < NumberToElongate; i++)
            {
                var link = CreateLink("New Link+", previousRigidBody);

                link.transform.position = endRigidBody.transform.position;

                _links.Add(link);

                previousRigidBody = link.GetComponent<Rigidbody2D>();
            }

            endRigidBody.GetComponent<Joint2D>().connectedBody = previousRigidBody;
        } else
        {
            var previousRigidBody = _links[0].GetComponent<Rigidbody2D>();
            var newLinks = new List<GameObject>();

            for (int i = 0; i < NumberToElongate; i++)
            {
                var link = CreateLink("New Link-", previousRigidBody);

                link.transform.position = startRigidBody.transform.position;

                newLinks.Add(link);

                previousRigidBody = link.GetComponent<Rigidbody2D>();
            }

            newLinks.Reverse();

            startRigidBody.GetComponent<Joint2D>().connectedBody = previousRigidBody;// newLinks[0].GetComponent<Rigidbody2D>();

            newLinks.AddRange(_links);
            _links.Clear();
            _links.AddRange(newLinks);
        }
    }
}
