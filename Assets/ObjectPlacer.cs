using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ObjectPlacer : MonoBehaviour
{

    public HologramVisualizer hologramVisualizerPrefab;


    List<HologramVisualizer> hologramVisualizers = new List<HologramVisualizer>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {

            Pose? pose = GetRayPose(Input.mousePosition);
            if (pose != null) {
                var hologram = new Hologram { bornPose = pose.Value, anchoredPose = pose.Value };
                VisualizeHologram(hologram);
            }
                
        }
        
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

    public void Save() {
        HologramMap map = new HologramMap();
        foreach(HologramVisualizer hv in hologramVisualizers) {
            map.list.Add(hv.Hologram);
        }
        var json = JsonUtility.ToJson(map);
        PlayerPrefs.SetString("hologramMap", json);
        PlayerPrefs.Save();
    }

    public void Load() {
        var json = PlayerPrefs.GetString("hologramMap");
        if (!string.IsNullOrEmpty(json)) {
            var map = JsonUtility.FromJson<HologramMap>(json);
            foreach(Hologram hologram in map.list) {
                VisualizeHologram(hologram);
            }
        }
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