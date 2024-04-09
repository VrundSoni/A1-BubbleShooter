using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Camera main;
    public Bubble BubblePrefab;
    public Transform FirePoint;

    public GameObject NextBubble;

    float horizontalAxis;
    Bubble NewBubble;

    Color NxtColor;
    Vector3 NewRot = Vector3.zero;

    void Update()
    {
        HandleKeyboardInput();

        if (Input.GetMouseButtonDown(0))
        {
            NewBubble = Instantiate(BubblePrefab, FirePoint.position, Quaternion.identity);
           
            //NewBubble.GetComponent<SpriteRenderer>().color = NxtColor;
            NewBubble.SetColor(NxtColor);
           
            NewBubble.direction = transform.up;
            NewBubble.bFromShooter = true;

            NextBubble.GetComponent<SpriteRenderer>().color = GetNextBubbleColor();
        }
    }

    void HandleKeyboardInput()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        NewRot.z += horizontalAxis;
        NewRot.z = Mathf.Clamp(NewRot.z, -60.0f, 60.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-NewRot), 1);
    }

    Color GetNextBubbleColor()
    {
        NxtColor = BGrid.colors[Random.Range(0, 5)];
        return NxtColor;
    }
}
