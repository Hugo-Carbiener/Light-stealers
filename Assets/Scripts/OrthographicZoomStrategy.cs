using UnityEngine;

public class OrthographicZoomStrategy : IZoomStrategy
{
    public OrthographicZoomStrategy(Camera cam, float startingZoom)
    {
        cam.orthographicSize = startingZoom;
    }

    public void zoomIn(Camera cam, float delta, float nearZoomLimit)
    {
        if (cam.orthographicSize == nearZoomLimit) return;
        cam.orthographicSize = Mathf.Max(cam.orthographicSize - delta, nearZoomLimit);
        BuildingConstructionUIManager.Instance.UpdateWorldPosition();
    }

    public void zoomOut(Camera cam, float delta, float farZoomLimit)
    {
        if (cam.orthographicSize == farZoomLimit) return;
        cam.orthographicSize = Mathf.Min(cam.orthographicSize + delta, farZoomLimit);
        BuildingConstructionUIManager.Instance.UpdateWorldPosition();
    }
}
