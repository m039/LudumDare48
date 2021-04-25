using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using m039.Common;
using System;

public class CameraController : MonoBehaviour
{
    public Rope rope;

    public float referenceOrthographicSize1 = 17.34f;

    public float referenceOrthographicSize2 = 18.51f;

    const int ReferenceNumberOfLinks1 = 100;

    const int ReferenceNumberOfLinks2 = 150;

    public float transitionTime = 2f;

    float _time;

    int _previousNumberOfLinks = -1;

    int _currentNumberOfLinks = -1;

    void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraOrthographicSize();
    }

    void UpdateCameraPosition()
    {
        var cameraPosition = Camera.main.transform.position;
        var centerOfMass = rope.GetCenterOfMass();
        cameraPosition.x = centerOfMass.x;
        cameraPosition.y = centerOfMass.y;
        Camera.main.transform.position = cameraPosition;
    }

    void UpdateCameraOrthographicSize()
    {
        if (rope.LinksCount == ReferenceNumberOfLinks1 || _previousNumberOfLinks == -1)
        {
            Camera.main.orthographicSize = GetOrthographicSize(rope.LinksCount);
            _previousNumberOfLinks = rope.LinksCount;
            _currentNumberOfLinks = rope.LinksCount;
            _time = float.MaxValue;
        }

        if (rope.LinksCount != _currentNumberOfLinks)
        {
            _previousNumberOfLinks = _currentNumberOfLinks;
            _currentNumberOfLinks = rope.LinksCount;
            _time = 0;
        }

        if (_time < transitionTime)
        {
            Camera.main.orthographicSize = EasingFunction.EaseInOutQuad(
                GetOrthographicSize(_previousNumberOfLinks),
                GetOrthographicSize(_currentNumberOfLinks),
                _time / transitionTime
                );

            _time += Time.deltaTime;
        }
    }

    float GetOrthographicSize(int numberOfLinks)
    {
        var orthographicSizeReferenceDelta = referenceOrthographicSize2 - referenceOrthographicSize1;
        float numberOfLinksReferenceDelta = ReferenceNumberOfLinks2 - ReferenceNumberOfLinks1;

        var numberOfLinksDelta = numberOfLinks - ReferenceNumberOfLinks1;
        var orthographicSizeDelta = numberOfLinksDelta * orthographicSizeReferenceDelta / numberOfLinksReferenceDelta;

        return referenceOrthographicSize1 + orthographicSizeDelta;
    }
}
