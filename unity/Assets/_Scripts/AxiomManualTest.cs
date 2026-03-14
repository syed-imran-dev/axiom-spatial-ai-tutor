using UnityEngine;

public class AxiomManualTest : MonoBehaviour
{
    [Header("References")]
    public AxiomFactory factory;
    public AxiomSphericalCamera cameraController;

    [Header("Test Payload")]
    [TextArea(10, 20)]
    public string testJson;

    // This creates a button in the Inspector's "three-dot" (cog) menu
    [ContextMenu("Execute Test Payload")]
    public void ExecuteTest()
    {
        if (string.IsNullOrEmpty(testJson))
        {
            Debug.LogError("Axiom Test: No JSON payload found!");
            return;
        }

        Debug.Log("Axiom Test: Executing manual payload...");
        
        // 1. Clear the old scene
        factory.ClearAll();

        // 2. Build the new scene
        // We use the same method Flutter will call: BuildMathScene
        factory.BuildMathScene(testJson);

        // 3. Trigger the procedural camera focus
        cameraController.FocusAll();
    }

    [ContextMenu("Clear Scene Only")]
    public void ClearOnly()
    {
        factory.ClearAll();
    }
}
