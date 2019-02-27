using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class HologramVisualizer : MonoBehaviour
{
    public GameObject bornPrefab;
    public GameObject anchoredPrefab;

    GameObject bornCube;
    GameObject anchoredCube;

    Hologram hologram;
    public Hologram Hologram {
        get {
            return new Hologram() { bornPose = hologram.bornPose, anchoredPose = new Pose { position = anchoredCube.transform.position, rotation = anchoredCube.transform.rotation } };
        }
        set {
            if (hologram != null)
                return;

            hologram = value;

            bornCube = Instantiate(bornPrefab, hologram.bornPose.position, hologram.bornPose.rotation);
            anchoredCube = Instantiate(anchoredPrefab, hologram.anchoredPose.position, hologram.anchoredPose.rotation);
            anchoredCube.AddComponent<UnityARUserAnchorComponent>();

        }
    }

    private void OnDestroy() {
        if (bornCube != null)
            Destroy(bornCube);
        if (anchoredCube != null)
            Destroy(anchoredCube);
    }
}
