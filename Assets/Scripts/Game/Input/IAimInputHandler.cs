using UnityEngine;

/// <summary>
/// 발사 입력을 처리할 대상이 구현해야 하는 인터페이스
/// </summary>
public interface IAimInputHandler
{
    void OnStartAiming(Vector2 position);
    void OnEndAiming(Vector2 position);
    void OnUpdateAiming(Vector2 currentPosition);
}
