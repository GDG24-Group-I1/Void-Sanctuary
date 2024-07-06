using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownAnimation : MonoBehaviour
{

    [SerializeField] private float YDiff;
    [SerializeField] private float Speed;

    private Vector3 startPos;
    private Vector3 endPos;

    private Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = transform.position.ShiftBy(y: YDiff);
        direction = YDiff > 0 ? Direction.Up : Direction.Down;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == Direction.Up)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, Speed * Time.deltaTime);
            if (transform.position == endPos)
            {
                direction = Direction.Down;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y - startPos.y) <= 0.1f)
            {
                direction = Direction.Up;
            }
        }
    }
}
