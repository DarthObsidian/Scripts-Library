using UnityEngine;
using UnityEngine.Events;

public class UserInput : MonoBehaviour
{
    private Coroutine play;
    private WaitForSeconds oneHundredth => new WaitForSeconds(0.01f);

    private bool takeInput;

    public static UnityAction<float, float> XYInput;

    private void Start()
    {
        takeInput = true;
        play = StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        while(takeInput)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            if(XYInput != null)
                XYInput(moveX, moveZ);

            yield return oneHundredth;
        }
    }
}