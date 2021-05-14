using UnityEngine;
using System.Collections.Generic;

public class Tube : MonoBehaviour
{
    [SerializeField] private float height;
    public float Height { get => height; set => height = value; }

    public int index;
    public Stack<GameObject> balls = new Stack<GameObject>();
    public bool isSelected = false;

    public void Resize()
    {
        transform.GetChild(0).localScale = new Vector3(1, height + 0.2f, 1);
        var oldPos = transform.GetChild(1).position;
        transform.GetChild(1).position = new Vector3(oldPos.x, -height - 0.2f, oldPos.z);
    }

    public GameObject removeTopBall() {

        if(balls.Count == 0) {
            return null;
        }

        GameObject ball = balls.Pop();

        return ball;
    }

    public void addBallToTube(GameObject ball) {
        balls.Push(ball);
    }

    public void changeSelectedValue() {
        isSelected = !isSelected;
    }

    public Vector3 getPositionOfTopBall() {

        GameObject topBall = this.removeTopBall();

        if(topBall == null) {
           return new Vector3(this.transform.position.x, (float)-2.7, this.transform.position.z);
        }

        Vector3 tubePosition = topBall.transform.position;
        this.addBallToTube(topBall);

        return tubePosition;
    }

}
