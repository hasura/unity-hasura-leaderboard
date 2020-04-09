using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Device {
	static float _orthoScreenWidth = 0f;
    static float _orthoScreenHeight = 0f;

    // Approximate value only
    static Vector2 estimatedSizeInInches;
    public static Vector2 GetEstimatedSizeInInches() {
        return estimatedSizeInInches;
    }

    static public float orthoScreenWidth { get { return _orthoScreenWidth; } }
    static public float orthoScreenHeight { get { return _orthoScreenHeight; } }
    static public Vector2 orthoScreenSize { get { return new Vector2(_orthoScreenWidth, _orthoScreenHeight); } }

    static public Rect orthoScreenRect
    {
	    get
	    {
		    return Rect.MinMaxRect(-_orthoScreenWidth * 0.5f, -_orthoScreenHeight * 0.5f, _orthoScreenWidth * 0.5f,
			    _orthoScreenHeight * 0.5f);
	    }
    }

    private static bool didInitDeviceData = false;
    public static void InitDeviceData() {
        if (didInitDeviceData)
            return;


#if UNITY_STANDALONE_OSX
        Screen.SetResolution(640, 1136, false);
#endif

        didInitDeviceData = true;
		
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;

		// Approximated value
		float dpi = Screen.dpi;
		if (dpi > 10.0f) {
			estimatedSizeInInches = new Vector2 (((float)Screen.width) / dpi, ((float)Screen.height) / dpi);
		}
		else {
			estimatedSizeInInches = (Screen.width < Screen.height) ? new Vector2 (2.30f, 4.09f) : new Vector2 (4.09f, 2.30f);
		}

		// Ortho Screen Sizes
		_orthoScreenHeight = Camera.main.orthographicSize * 2f;
	    _orthoScreenWidth = _orthoScreenHeight * (screenWidth / screenHeight);


    }
}
