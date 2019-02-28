using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class ObjectPlacer : MonoBehaviour
{

    public HologramVisualizer hologramVisualizerPrefab;


    List<HologramVisualizer> hologramVisualizers = new List<HologramVisualizer>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isLocked && !IsPointerOverUIObject()) {

            Pose? pose = GetRayPose(Input.mousePosition);
            if (pose != null) {
                var hologram = new Hologram { bornPose = pose.Value, anchoredPose = pose.Value };
                VisualizeHologram(hologram);
            }
                
        }
        
    }

    private void Start() {
        VisualizeLockedText();
    }
    public Text isLockedText;
    bool isLocked = true;
    public void ToggleLock() {
        isLocked = !isLocked;
        VisualizeLockedText();
    }
    void VisualizeLockedText() {
        isLockedText.text = "Lock " + isLocked;
    }

    void VisualizeHologram(Hologram hologram) {
        var visualizer = Instantiate(hologramVisualizerPrefab);
        visualizer.Hologram = hologram;
        hologramVisualizers.Add(visualizer);
    }

    public void Clear() {
        while(hologramVisualizers.Count > 0) {
            var hv = hologramVisualizers[0];
            hologramVisualizers.RemoveAt(0);
            Destroy(hv.gameObject);
        }
    }

    private void OnDestroy() {
        Save();
    }
    private void OnApplicationQuit() {
        Save();
    }

    bool isLoaded = false;
    public void Save() {
        bool hasData = PlayerPrefs.GetString("hologramMap", "-1") != "-1";
        if (hasData && !isLoaded)
            return;

        HologramMap map = new HologramMap();
        foreach(HologramVisualizer hv in hologramVisualizers) {
            map.list.Add(hv.Hologram);
        }
        var json = JsonUtility.ToJson(map);
        PlayerPrefs.SetString("hologramMap", json);
        Debug.Log("Saved Data " + json);
        PlayerPrefs.Save();
    }

    public void Load() {
        var json = PlayerPrefs.GetString("hologramMap");
        Debug.Log("Loaded Data " + json);
        if (!string.IsNullOrEmpty(json)) {
            var map = JsonUtility.FromJson<HologramMap>(json);
            foreach(Hologram hologram in map.list) {
                VisualizeHologram(hologram);
            }
        }
        isLoaded = true;
    }



    Pose? GetRayPose(Vector2 screenPos) {
        var screenPosition = Camera.main.ScreenToViewportPoint(screenPos);
        ARPoint point = new ARPoint {
            x = screenPosition.x,
            y = screenPosition.y
        };

        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
                Debug.Log("Got hit!");
                var pos = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                var rot = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                return new Pose(pos, rot);
            }
        }

        return null;
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

[System.Serializable]
public class HologramMap {
    public List<Hologram> list = new List<Hologram>();

    public HologramMap() {

    }
}

[System.Serializable]
public class Hologram {
    public Pose bornPose;
    public Pose anchoredPose;

    public Hologram() {

    }
}