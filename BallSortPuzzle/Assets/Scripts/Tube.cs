using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField] private int height;
    public int Height { get => height; set => height = value; }

    public void Resize()
    {
        transform.GetChild(0).localScale = new Vector3(1, height + 0.2f, 1);
        var oldPos = transform.GetChild(1).position;
        transform.GetChild(1).position = new Vector3(oldPos.x, -height - 0.2f, oldPos.z);
    }

}
