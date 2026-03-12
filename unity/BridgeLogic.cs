using UnityEngine;
using System.Runtime.InteropServices;

public class AstraBridge : MonoBehaviour {
    // Native iOS Bridge
    [DllImport("__Internal")]
    private static extern void _OnUnityActionCompleted(string result);

    public void OnNativeCommandReceived(string command) {
        Debug.Log("Command from iOS Native: " + command);
        // Logic to move 3D Character or trigger AI animation
        TriggerAIVisualization(command);
    }

    private void TriggerAIVisualization(string command) {
        // High-end animation logic for the AI assistant
        if (command == "HAPPY") {
            GetComponent<Animator>().SetTrigger("isHappy");
        }
    }
}
