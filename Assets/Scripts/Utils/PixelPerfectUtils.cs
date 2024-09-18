using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class PixelPerfectUtils
{
    public static PixelPerfectCamera pixelPerfectCamera { get; private set; }
         
    private static void initCamera()
    {
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
    }

    /**
     * Converts a screen point set in the reduced canvas of the pixel perfect camera to a screen point position in a full resolution
     */
    public static Vector3 pixelPerfectToFullScreen(Vector3 input)
    {
        if (!pixelPerfectCamera) initCamera();

        return new Vector3(
            input.x * Screen.width / pixelPerfectCamera.refResolutionX,
            input.y * Screen.height / pixelPerfectCamera.refResolutionY,
            input.z);
    }
}
