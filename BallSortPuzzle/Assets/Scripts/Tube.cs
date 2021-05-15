using UnityEngine;
using System.Collections.Generic;

public class Tube : MonoBehaviour
{
    [SerializeField] private float height;
    public float Height { get => height; set => height = value; }

    public int index;
    public readonly Stack<GameObject> Balls = new Stack<GameObject>();
    public bool isSelected = false;

    public void Resize()
    {
        transform.GetChild(0).localScale = new Vector3(1, height + 0.2f, 1);
        var oldPos = transform.GetChild(1).position;
        transform.GetChild(1).position = new Vector3(oldPos.x, -height - 0.2f, oldPos.z);
    }

    public GameObject removeTopBall()
    {
        return Balls.Count == 0 ? null : Balls.Pop();
    }

    public void addBallToTube(GameObject ball) {
        Balls.Push(ball);
    }

    public void changeSelectedValue() {
        isSelected = !isSelected;
    }

    public Vector3 getPositionOfTopBall() {
        var topBall = removeTopBall();

        if(topBall is null) {
           return new Vector3(this.transform.position.x, (float)-2.7, this.transform.position.z);
        }

        var tubePosition = topBall.transform.position;
        addBallToTube(topBall);

        return tubePosition;
    }

}
