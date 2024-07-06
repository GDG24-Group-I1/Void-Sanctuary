using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownAnimation : MonoBehaviour
{

    [SerializeField] private float YDiff;
    [SerializeField] private float Duration;
    [SerializeField] private float RotateSpeed;

    private AnimationCurve upCurve;
    private AnimationCurve downCurve;
    private float startPos;
    private float endPos;
    private float currentTime;

    private Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.y;
        endPos = transform.position.y + YDiff;
        direction = YDiff > 0 ? Direction.Up : Direction.Down;
        upCurve = AnimationCurve.EaseInOut(0, startPos, Duration, endPos);
        downCurve = AnimationCurve.EaseInOut(0, endPos, Duration, startPos);
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (direction == Direction.Up)
        {
            transform.position = transform.position.CopyWith(y: upCurve.Evaluate(currentTime));
            if (currentTime >= Duration)
            {
                direction = Direction.Down;
                currentTime = 0;
            }
        }
        else
        {
            transform.position = transform.position.CopyWith(y: downCurve.Evaluate(currentTime));
            if (currentTime >= Duration)
            {
                direction = Direction.Up;
                currentTime = 0;
            }
        }
        if (RotateSpeed != 0)
        {
            transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
        }
    }
}
