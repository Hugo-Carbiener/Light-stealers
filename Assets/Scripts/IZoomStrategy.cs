using UnityEngine;

public interface IZoomStrategy
{
    void zoomIn(Camera cam, float delta, float nearZoomLimit);
    void zoomOut(Camera cam, float delta, float farZoomLimit);
}
