using UnityEngine;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json; // Make sure you have Json.Net in your project
using System.Collections.Generic;


public class AxiomFactory : MonoBehaviour 
{
    private List<GameObject> activeShapes = new List<GameObject>();


    public List<GameObject> GetActiveObjects() 
{
    return activeShapes; // Or whatever your list variable is named
}
    // This is the EXACT name the Test Script and Flutter will look for
    public void BuildMathScene(string jsonPayload) 
    {
        ClearAll(); // Always clear the old math before building new math

        try {
            // Parse the JSON into our data model
            var response = JsonConvert.DeserializeObject<AxiomResponse>(jsonPayload);

            foreach (var obj in response.objects) {
                // Call the internal spawn logic
                SpawnShape(
                    obj.type, 
                    obj.pos.ToVector3(), 
                    obj.rot.ToVector3(), 
                    obj.size.ToVector3(), 
                    obj.color.ToColor()
                );
            }
        } catch (System.Exception e) {
            Debug.LogError("Axiom JSON Parse Error: " + e.Message);
        }
    }
    // The core "Build" function that Flutter will trigger
    public void SpawnShape(string shapeType, Vector3 pos, Vector3 rot, Vector3 scale, Color color) 
    {
        GameObject go;

        // Map all 6 Unity 3D Primitives
        switch (shapeType.ToUpper()) 
        {
            case "SPHERE":   go = GameObject.CreatePrimitive(PrimitiveType.Sphere); break;
            case "CYLINDER": go = GameObject.CreatePrimitive(PrimitiveType.Cylinder); break;
            case "CAPSULE":  go = GameObject.CreatePrimitive(PrimitiveType.Capsule); break;
            case "PLANE":    go = GameObject.CreatePrimitive(PrimitiveType.Plane); break;
            case "QUAD":     go = GameObject.CreatePrimitive(PrimitiveType.Quad); break;
            default:         go = GameObject.CreatePrimitive(PrimitiveType.Cube); break;
        }

        // Apply Transformation
        go.transform.SetParent(this.transform);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.Euler(rot);
        go.transform.localScale = scale;

        // Apply Color with a clean "Unlit" or "Standard" look
        Renderer renderer = go.GetComponent<Renderer>();
        renderer.material.color = color;

        activeShapes.Add(go);
    }

    public List<GameObject> GetShapes() => activeShapes;

    public void ClearAll() 
    {
        foreach (var shape in activeShapes) { if(shape != null) Destroy(shape); }
        activeShapes.Clear();
    }


}



// Ensure these models are defined at the bottom of the file or in a new file
[System.Serializable]
public class AxiomResponse { public List<AxiomObject> objects; }

[System.Serializable]
public class AxiomObject {
    public string type;
    public Vector3Data size;
    public Vector3Data pos;
    public Vector3Data rot;
    public ColorData color;
}

[System.Serializable]
public class Vector3Data {
    public float x, y, z;
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[System.Serializable]
public class ColorData {
    public float r, g, b;
    public Color ToColor() => new Color(r, g, b, 1f);
}

