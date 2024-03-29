using UnityEngine;
using UnityEditor;
using System.Collections.Generic; // For handling Lists

public class BoxColliderAdjuster : EditorWindow
{

    private List<SkinnedMeshRenderer> selectedMeshes = new List<SkinnedMeshRenderer>();
     [SerializeField]
    private Vector3 colliderCenterOffset = Vector3.zero;
    

    [MenuItem("Tools/Box Collider Adjuster")]
    public static void ShowWindow()
    {
        GetWindow<BoxColliderAdjuster>("Box Collider Adjuster").Show();
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("Adjust the box colliders of all selected mesh objects.", MessageType.Info);
        
        if (GUILayout.Button("Adjust Colliders for Selected Meshes"))
        {
            UpdateSelectedMeshes();

            Debug.Log("meshes Selected: " + selectedMeshes.Count);
            if (selectedMeshes.Count > 1){
                AdjustCollidersForMultipleSelected();
            } else {
                AdjustCollidersForSingleSelected();
            }
        }
    }


    private void UpdateSelectedMeshes()
    {
        selectedMeshes.Clear();
        foreach (GameObject obj in Selection.gameObjects)
        {
            SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                selectedMeshes.Add(renderer);
            }
        }
    }


    private void AdjustCollidersForMultipleSelected()
    {
        if (selectedMeshes.Count == 0)
        {
            Debug.LogWarning("No skinned mesh renderers selected.");
            return;
        }

        Bounds aggregateBounds = new Bounds();
        bool hasSetInitialBounds = false;

        foreach (SkinnedMeshRenderer renderer in selectedMeshes)
        {
            if (!hasSetInitialBounds)
            {
                aggregateBounds = renderer.bounds;
                hasSetInitialBounds = true;
            }
            else
            {
                aggregateBounds.Encapsulate(renderer.bounds);
            }
        }

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in selectedMeshes)
        {
            Transform rootBone = skinnedMeshRenderer.rootBone;
            if (rootBone != null)
            {
                Transform colliderBone = FindColliderBone(rootBone);
                if (colliderBone != null)
                {
                    AdjustColliderForMultiple(colliderBone, aggregateBounds);
                }
                else
                {
                    Debug.LogWarning($"Collider bone not found in hierarchy for object: {skinnedMeshRenderer.gameObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Root bone not found for object: {skinnedMeshRenderer.gameObject.name}");
            }
        }
    }

    private Transform FindColliderBone(Transform bone)
    {
        // Check for the BoxCollider on the current bone
        BoxCollider collider = bone.GetComponent<BoxCollider>();
        if (collider != null)
        {
            return bone;
        }

        // If this bone does not contain "BdBn" in its name, it's considered not suitable
        if (!bone.name.Contains("BdBn"))
        {
            return null;
        }

        // If we're at the root (no more parents), then return null since we haven't found a suitable collider
        if (bone.parent == null)
        {
            return null;
        }

        // Otherwise, continue searching up the hierarchy
        return FindColliderBone(bone.parent);
    }


    private void AdjustColliderForMultiple(Transform colliderBone, Bounds bounds)
    {
        BoxCollider colliderComponent = colliderBone.GetComponent<BoxCollider>();
        if (colliderComponent == null)
        {
            Debug.LogWarning($"No BoxCollider found for {colliderBone.name}. Attempting to add one.");
            colliderComponent = colliderBone.gameObject.AddComponent<BoxCollider>();
        }

        // Apply correction for swapped axes and padding
        float padding = .5f; // Adjust padding as needed, this adds extra space on the Z axis
        Vector3 correctedSize = new Vector3(
            bounds.size.z / colliderBone.lossyScale.z, // Swap Z and X sizes
            bounds.size.y / colliderBone.lossyScale.y, // Y stays the same
            bounds.size.x / colliderBone.lossyScale.x * padding // Add padding to the corrected Z axis (originally X)
        );

        colliderComponent.size = correctedSize;

        // Center adjustment remains the same, make sure it matches your expectations
        Vector3 globalColliderPosition = colliderBone.position;
        Vector3 meshCenterGlobal = bounds.center;
        Vector3 offsetVector = globalColliderPosition - meshCenterGlobal;
        colliderComponent.center = colliderBone.InverseTransformDirection(-offsetVector);

        Debug.Log($"Adjusted BoxCollider for {colliderBone.name} with size {colliderComponent.size} and center {colliderComponent.center}.");
    }


    private void AdjustColliderForSingle(Transform colliderBone, SkinnedMeshRenderer skinnedMeshRenderer)
        {
            BoxCollider colliderComponent = colliderBone.GetComponent<BoxCollider>();
            if (colliderComponent == null)
            {
                Debug.LogWarning($"No BoxCollider found for {colliderBone.name}. Make Sure to select objects that have driving bones with a collider.");
            }

            // Calculate the global center of the mesh.
            Vector3 meshCenterGlobal = skinnedMeshRenderer.bounds.center;

            // The global location of the collider bone.
            Vector3 boneGlobalPosition = colliderBone.position;

            // Determine the vector from the mesh center to the bone.
            Vector3 offsetVector = boneGlobalPosition - meshCenterGlobal;

            // Calculate collider size to match the mesh size, adjusted for the bone's scale.
            Bounds meshBounds = skinnedMeshRenderer.sharedMesh.bounds;
            Vector3 meshSize = meshBounds.extents * 2; // Full size in local space.
            Vector3 colliderSize = new Vector3(
                meshSize.x / colliderBone.lossyScale.x,
                meshSize.y / colliderBone.lossyScale.y,
                meshSize.z / colliderBone.lossyScale.z
            );
            colliderComponent.size = colliderSize;

            // Adjust the collider's center by applying the inverse of the offset vector,
            // transformed into the bone's local space (as the collider's center is relative to its transform).
            colliderComponent.center = colliderBone.InverseTransformDirection(-offsetVector);

            Debug.Log($"Adjusted BoxCollider for {colliderBone.name} with size {colliderSize} and center {colliderComponent.center}.");
        }

    private void AdjustCollidersForSingleSelected()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                Transform rootBone = skinnedMeshRenderer.rootBone;
                if (rootBone != null)
                {
                    Transform colliderBone = FindColliderBone(rootBone);
                    if (colliderBone != null)
                    {
                        AdjustColliderForSingle(colliderBone, skinnedMeshRenderer);
                    }
                    else
                    {
                        Debug.LogWarning($"Collider bone not found in hierarchy for object: {selectedObject.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Root bone not found for object: {selectedObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Selected object does not have a skinned mesh: {selectedObject.name}");
            }
        }
    }


}

////////////////////////////////////// OLD ONE ///////////////////////////////////////////////


    

    


   
    
