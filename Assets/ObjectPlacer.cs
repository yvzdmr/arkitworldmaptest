using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ObjectPlacer : MonoBehaviour
{

    public GameObject prefab;

    List<GameObject> gos = new List<GameObject>();

    void Start()
    {
        Load();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {

            Pose? pose = GetRayPose(Input.mousePosition);
            if (pose != null)
                Instantiate(pose.Value);
        }
        
    }
    private void OnDestroy() {
        Save();
    }

    public void Save() {
        var hologramMap = new HologramMap();
        foreach(GameObject g in gos) {
            hologramMap.list.Add(new Pose(g.transform.position, g.transform.rotation));
        }

        var json = JsonUtility.ToJson(hologramMap);
        PlayerPrefs.SetString("hologramMap", json);

    }

    public void Load() {
        var json = PlayerPrefs.GetString("hologramMap");
        if (!string.IsNullOrEmpty(json)) {
            var hologramMap = JsonUtility.FromJson<HologramMap>(json);
            foreach(Pose p in hologramMap.list) {
                Instantiate(p);
            }
        }
    }


    void Instantiate(Pose pose) {
        var obj = Instantiate(prefab, pose.position, pose.rotation);
        gos.Add(obj);
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
    public List<Pose> list = new List<Pose>();

    public HologramMap() {

    }
}