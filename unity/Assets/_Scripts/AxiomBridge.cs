using UnityEngine;
using System;

public class AxiomBridge : MonoBehaviour 
{
    // References to your existing logic
    public AxiomFactory factory;
    public AxiomSphericalCamera cameraController;

    /// <summary>
    /// This is the EXACT method name Flutter will call.
    /// It must take exactly ONE string parameter.
    /// </summary>
    public void OnFlutterMessage(string jsonPayload) 
    {
        try 
        {
            Debug.Log($"[Axiom Bridge] Received: {jsonPayload}");

            // 1. If the message contains "objects", it's a geometry update
            if (jsonPayload.Contains("objects")) 
            {
                factory.BuildMathScene(jsonPayload);
                
                // Always auto-focus after building
                if (cameraController != null) {
                    cameraController.FocusAll();
                }
            } 
            // 2. If it's a simple command string
            else if (jsonPayload == "reset_camera") 
            {
                cameraController.FocusAll();
            }
        } 
        catch (Exception e) 
        {
            Debug.LogError($"[Axiom Bridge] Error processing message: {e.Message}");
        }
    }
}
