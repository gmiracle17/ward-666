using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light lightToFlicker;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f; // Lower is faster

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerSpeed)
        {
            lightToFlicker.intensity = Random.Range(minIntensity, maxIntensity);
            timer = 0f;
        }
    }
}
