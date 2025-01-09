    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocation : MonoBehaviour
{
    public float latitude;
    public float longitude;

    // The name of the controller button you want to use
    // You can find these in Edit > Project Settings > Input Manager
    public string controllerButtonName = "Submit"; // Example: "Submit" (often mapped to 'A' on Xbox, 'X' on PlayStation)

    void Update()
    {
        // Check for the controller button press
        if (Input.GetButtonDown(controllerButtonName))
        {
            RequestLocation();
        }
    }

    // Public method to be called when the button is pressed
    public void RequestLocation()
    {
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled by user.");
            yield break;
        }

        // Start service before querying location data.
        Input.location.Start();

        // Wait until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            Input.location.Stop();
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            Input.location.Stop();
            yield break;
        }
        else
        {
            // Access granted and location data is available
            Debug.Log("Location: " + Input.location.lastData.latitude + "; " + Input.location.lastData.longitude);

            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            // Stop service after getting the location (if you only need it once per button press)
            Input.location.Stop();
        }
    }
}
