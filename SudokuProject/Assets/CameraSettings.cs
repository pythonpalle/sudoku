using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ColorObject colorObject;
    private void OnEnable()
    {
        colorObject.OnColorChange += OnColorUpdate;
    }
    
    private void OnDisable()
    {
        colorObject.OnColorChange += OnColorUpdate;
    }
    private void OnColorUpdate(Color color)
    {
        _camera.backgroundColor = color;
    }
}
