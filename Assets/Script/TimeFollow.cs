// I used https://www.youtube.com/watch?v=ysg9oaZEgwc to figure out how to make the timer follow the player.

using UnityEngine;

public class TimeFollow : MonoBehaviour
{
    Transform unit;

    Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = unit.position + offset;
    }
}
