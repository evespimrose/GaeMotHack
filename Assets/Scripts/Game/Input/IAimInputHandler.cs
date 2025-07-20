using UnityEngine;

public interface IAimInputHandler
{
    void OnStartAiming(Vector2 position);
    void OnEndAiming(Vector2 position);
    void OnStartPowerHandling();
    void OnEndPowerHandling(float power);
}
