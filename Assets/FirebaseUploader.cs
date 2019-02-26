using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseUploader : MonoBehaviour {

    private FirebaseStorage _storage;
    private StorageReference _storageReference;
    private StorageReference _testStorageReference;
    private string _downloadURL;

    static FirebaseUploader Instance;

    private void Awake() {
        Instance = this;    
    }

    private void Start() {
        _storage = FirebaseStorage.DefaultInstance;

        
        _storageReference = _storage.RootReference;
        _testStorageReference = _storageReference.Child("armap");
    }

    public static void Upload(byte[] data) {
       

        Debug.Log("Serialized data: " + data.Length);

        Instance._testStorageReference.PutBytesAsync(data, null, new StorageProgress<UploadState>(state => {

            Debug.Log("Progress: " + state.BytesTransferred + " of " + state.TotalByteCount + " bytes transferred.");

        })).ContinueWith((Task<StorageMetadata> task) => {

            if (task.IsFaulted || task.IsCanceled) {
                Debug.Log("Error: " + task.Exception.ToString());
            } else {
                if (!task.IsFaulted)
                    Debug.Log("Finished uploading...");
            }
        });
    }

    public static async Task<byte[]> Download() {
        return await Instance._testStorageReference.GetBytesAsync(long.MaxValue);
    }

}
