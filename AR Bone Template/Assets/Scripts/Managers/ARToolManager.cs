using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ARToolManager implemented as a singleton.
public class ARToolManager : MonoBehaviour
{
    // Field assigned in inspector
    [SerializeField]
    private Transform m_ARObjectsHolder;
    [SerializeField]
    private GameObject m_ARPointPrefab, m_ARPlanePrefab, m_DistancePrefab, m_AnglePrefab;
    [SerializeField]
    private DynamicMenu m_DynamicMenuAction;
    [SerializeField]
    private QRAnchorManager m_QRAnchorManager;
    [SerializeField]
    private TextMeshPro m_DebugTextField;

    // Private
    private Transform m_CameraTransform;
    private readonly List<ARObject> m_SelectedObjects = new();
    private int m_ARPointCount, m_ARPlaneCount;

    #region MonoBehaviour functions

    // Start is called before the first frame update
    void Start()
    {
        m_CameraTransform = Camera.main.transform;
        m_ARPointCount = 0;
        m_ARPlaneCount = 0;
        UpdateDynamicMenuAction();
        m_QRAnchorManager.TrackObject(m_ARObjectsHolder.gameObject);
    }

    #endregion

    #region Button functions

    public void SpawnPointButtonPressed() => SpawnPoint();
    private GameObject SpawnPoint()
    {
        m_ARPointCount++;
        return SpawnARObject(m_ARPointPrefab, "Point " + m_ARPointCount);
    }

    public void SpawnPlaneButtonPressed() => SpawnPlane();
    public GameObject SpawnPlane()
    {
        m_ARPlaneCount++;
        GameObject arPlane = SpawnARObject(m_ARPlanePrefab, "Plane " + m_ARPlaneCount);
        ARPlane arPlanScript = arPlane.GetComponentInChildren<ARPlane>();
        arPlanScript.GetRootTransform().LookAt(Camera.main.transform);
        return arPlane;
    }

    private GameObject SpawnARObject(GameObject objectToSpawn, string arObjectName)
    {
        Vector3 spawnLoc = m_CameraTransform.position + 0.5f * m_CameraTransform.forward;
        if (spawnLoc != Vector3.zero) {
            GameObject arObject = Instantiate(objectToSpawn, spawnLoc, Quaternion.identity, m_ARObjectsHolder);
            arObject.GetComponentInChildren<ARObject>()?.Init(arObjectName);
            return arObject;
        }
        return null;
    }

    #endregion

    #region Helper functions

    public void AddSelectedARObject(ARObject arObject)
    {
        m_DynamicMenuAction.transform.position = arObject.transform.position + Camera.main.transform.right * 0.25f;
        m_DynamicMenuAction.transform.LookAt(Camera.main.transform);
        m_SelectedObjects.Add(arObject);
        UpdateDynamicMenuAction();
    }

    public void RemoveSelectedARObject(ARObject arObject)
    {
        m_SelectedObjects.Remove(arObject);
        UpdateDynamicMenuAction();
    }

    public void DebugText(string text) {
        //m_DebugTextField.text = m_DebugTextField.text + "\n" + text;
        //Debug.Log(text);
    }

    #endregion

    #region Dynamic Menu Helper functions

    private void UpdateDynamicMenuAction()
    {
        m_DynamicMenuAction.gameObject.SetActive(false);
        m_DynamicMenuAction.ResetMenu();
        m_SelectedObjects.RemoveAll(arObj => arObj == null);
        int numSelectedARObjects = m_SelectedObjects.Count;
        switch (numSelectedARObjects) {
            case 0:
                break;
            case 1:
                UpdateDynamicMenuActionWithOneARObjects(m_SelectedObjects[0]);
                m_DynamicMenuAction.Init();
                break;
            case 2:
                UpdateDynamicMenuActionWithTwoARObjects(m_SelectedObjects[0], m_SelectedObjects[1]);
                m_DynamicMenuAction.Init();
                break;
            default:
                UpdateDynamicMenuActionWithMoreThanTwo();
                m_DynamicMenuAction.Init();
                break;
        }
    }

    private void UpdateDynamicMenuActionWithOneARObjects(ARObject arObject)
    {
        if (arObject is ARPlane arplane) {
            // Add Distance-Measurement Button!
            m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
                "Add new relative plane",
                () => {
                    m_ARPlaneCount++;
                    GameObject arPlane2GO = Instantiate(m_ARPlanePrefab, arplane.GetRootTransform());
                    arPlane2GO.transform.localPosition = Vector3.zero;
                    ARPlane arPlane2 = arPlane2GO.GetComponentInChildren<ARPlane>();
                    arPlane2.Init(arplane.GetNextChildName());
                    arPlane2.IsAnchored = true;
                    arPlane2.Init(arplane);
                    arObject.Deselect(); // Deselect current plane
                    arPlane2.Select(); // Select child plane
                    MeasureAnglesBetweenTwoPlanes(arplane, arPlane2);
                }
            ));

            // Add sliders on page 2 for relative planes.
            if (arplane.IsAnchored) {
                AddSlidersForRelativePlane(arplane);
            }
        } else {
            // Add Distance-Measurement Button!
            m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
                "Add point and measure distance",
                () => {
                    GameObject newARObject = SpawnPoint();
                    ARObject arObject2 = newARObject.GetComponentInChildren<ARObject>();
                    MeasureDistanceBetweenTwoPoints(arObject, arObject2);
                }
            ));
        }

        // Add Delete Button!
        m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
            "Delete selected object",
            () => {
                RemoveSelectedARObject(arObject);
                arObject.DestroyARObject();
            }
        ));
    }

    private void UpdateDynamicMenuActionWithTwoARObjects(ARObject arObject1, ARObject arObject2)
    {
        // Add Distance-Measurement Button!
        bool objectAreAssociated = arObject1.IsAssociatedWithGameObject(arObject2) || arObject2.IsAssociatedWithGameObject(arObject1);
        if (!objectAreAssociated) {
            m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
                "Measure distance",
                () => {
                    MeasureDistanceBetweenTwoPoints(arObject1, arObject2);
                    UpdateDynamicMenuAction();
                }
            ));
        }

        // Add Delete Button!
        m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
            "Delete selected objects",
            () => {
                m_SelectedObjects.Remove(arObject1);
                m_SelectedObjects.Remove(arObject2);
                UpdateDynamicMenuAction();
                arObject1.DestroyARObject();
                arObject2.DestroyARObject();
            }
        ));
    }

    private void UpdateDynamicMenuActionWithMoreThanTwo()
    {
        // Add Delete Button!
        m_DynamicMenuAction.AddElement(0, new ButtonDefinition(
            "Delete selected objects",
            () => {
                List<ARObject> arObjectListCopy = new List<ARObject>(m_SelectedObjects);
                foreach (ARObject aRObject in arObjectListCopy) {
                    m_SelectedObjects.Remove(aRObject);
                    aRObject.DestroyARObject();
                }
                UpdateDynamicMenuAction();
            }
        ));
    }

    private void AddSlidersForRelativePlane(ARPlane arplane)
    {
        m_DynamicMenuAction.AddElement(1, new RotationSliderDefinition(MixedReality.Toolkit.Axis.X, arplane.GetRootTransform()));
        m_DynamicMenuAction.AddElement(1, new RotationSliderDefinition(MixedReality.Toolkit.Axis.Y, arplane.GetRootTransform()));
        //m_DynamicMenuAction.AddElement(1, new RotationSliderDefinition(MixedReality.Toolkit.Axis.Z, arplane.GetRootTransform()));
        m_DynamicMenuAction.AddElement(1, new OffsetSliderDefinition(arplane.GetReferencePlane(), arplane.GetRootTransform()));
    }

    private void MeasureDistanceBetweenTwoPoints(ARObject arObject1, ARObject arObject2)
    {
        GameObject distanceObj = Instantiate(m_DistancePrefab, m_ARObjectsHolder);
        DistanceMeasure distanceMeasure = distanceObj.GetComponent<DistanceMeasure>();
        distanceMeasure.Init(arObject1, arObject2);
        arObject1.AddAssociatedGameObject(arObject2);
        arObject2.AddAssociatedGameObject(arObject1);
    }


    private void MeasureAnglesBetweenTwoPlanes(ARPlane arObject1, ARPlane arObject2)
    {
        GameObject angleObj = Instantiate(m_AnglePrefab, m_ARObjectsHolder);
        AngleMeasure angleMeasure = angleObj.GetComponent<AngleMeasure>();
        angleMeasure.Init(arObject1, arObject2);
        arObject1.AddAssociatedGameObject(arObject2);
        arObject2.AddAssociatedGameObject(arObject1);
    }

    #endregion
}
