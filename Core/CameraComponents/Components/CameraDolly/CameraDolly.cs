//BY DX4D
using UnityEngine;
using OpenMMO;

[RequireComponent(typeof(PlayerComponent))] //ONLY FOR PLAYERS

/// <summary>Carries a set of camera prefabs that are loaded from the Resources/Cameras folder.</summary>
public class CameraDolly : MonoBehaviour
{
#pragma warning disable CS0414
    [Header("UPDATE FREQUENCY")]
    [Tooltip("How many update frames must pass before this component updates again?")]
    [SerializeField] [Range(1, 60)] int tickFrequency = 60; //TICK RATE

    [Header("CAMERA SELECT")]
    [Tooltip("Select a camera to load.")]
    [SerializeField] CameraType cameraType = CameraType.Malice;
    [Tooltip("Parent the camera to this object?")]
    public bool attachCameraToMe = false;

    [Header("-read only-")]
    [Tooltip("The camera prefab that will be spawned. (Set by the camera type)")]
    [SerializeField] GameObject cameraPrefab;

    GameObject mainCamera;

    public static GameObject spawnedCamera;
    [SerializeField] bool cameraIsSpawned = false;
#pragma warning restore CS0414

    //PLAYER COMPONENT REFERENCE
    PlayerComponent _player;
    PlayerComponent player
    {
        get
        {
            if (!_player) _player = GetComponent<PlayerComponent>();
            return _player;
        }
    }

#if _CLIENT
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UpdateAttachedCamera())
        {
            DestroySpawnedCamera();
            cameraIsSpawned = false;
        }
    }
#endif
    //AWAKE
    /*private void Start()
    {
        if (!AttachExistingCamera())
        {
            if (player != null && !player.IsLocalPlayer) //DISABLE IF NOT LOCAL PLAYER
            {
                DestroySpawnedCamera();
                return;
            }
               
            //if (!CameraDolly.spawnedCamera)
            //{
                Debug.Log("SPAWNING NEW CAMERA");
                LoadCameraPrefab(); //LOAD CAMERA PREFAB
                SpawnCamera(); //SPAWN CAMERA
            //}
        }
    }*/
    //ON DISABLE
    private void OnDisable()
    {
        Debug.Log("DISABLING CAMERA DOLLY");

        DestroySpawnedCamera();
        //cameraIsSpawned = false;

        //ENABLE MAIN CAMERA
        if (mainCamera != null) { mainCamera.SetActive(true); }
    }
    //ON ENABLE
    private void OnEnable()
    { Enable(); }
    private void Enable()
    {
        if (player != null && !player.IsLocalPlayer) //DISABLE IF NOT LOCAL PLAYER
        {
            //DestroySpawnedCamera(); //<-- Destroys on Disable now
            //enabled = false;
            return;
        }

        Debug.Log("ENABLING CAMERA DOLLY");

        //DISABLE MAIN CAMERA
        if (Camera.main != null && Camera.main.name.Contains("Main"))
        {
            mainCamera = Camera.main.gameObject;
            mainCamera.SetActive(false);
        }

        SpawnCamera(); //SPAWN CAMERA
    }
    //ON DESTROYED
    private void OnDestroy()
    {
        DestroySpawnedCamera();
        //cameraIsSpawned = false;
    }

    //UPDATE
    int frameCount = 0; //FRAME COUNTER
    void FixedUpdate()
    {
        if (!Application.isPlaying || player == null) return; //MUST BE PLAYING A SCENE AND LOGGED IN

        frameCount++; //INCREMENT TICK

        if (frameCount >= tickFrequency) //TICK THIS FRAME?
        {
            frameCount = 0; //RESET THE COUNTER

            //if (player != null && !player.IsLocalPlayer) { DestroySpawnedCamera(); return; } //DISABLE IF NOT LOCAL PLAYER

            if (cameraIsSpawned) return; //CAMERA ALREADY SPAWNED

            Enable();

            //if (AttachExistingCamera())
            //{
            //    Debug.Log("ATTACHING CAMERA FROM SCENE");
            //}
            //else
            //{
            //if (!cameraIsSpawned)
            //{



            //if (!CameraDolly.spawnedCamera)
            //{
            //    Debug.Log("SPAWNING NEW CAMERA");
            //    LoadCameraPrefab(); //LOAD CAMERA PREFAB
            //    SpawnCamera(); //SPAWN CAMERA
            //}
            //else
            //{
            //    if (!AttachExistingCamera())
            //    {
            //        DestroySpawnedCamera();
            //    }
            //}



                //}
            //}

            //if (!cameraIsSpawned && spawnedCamera) DestroySpawnedCamera(); //DESTROY UNSPAWNED CAMERA

            //if (!AttachExistingCamera()) //ATTACH EXISTING CAMERA
            //{
            //    if (UpdateAttachedCamera()) //UPDATE CAMERA
            //    {
            //        SpawnCamera(); //SPAWN CAMERA
            //    }
            //}

        }
    }

    //UPDATE ATTACHED CAMERA
    /// <summary></summary>
    /// <returns></returns>
    bool UpdateAttachedCamera()
    {
        if (!cameraPrefab || !cameraPrefab.name.Contains(cameraType.ToString() + "Camera")) //NOTE: We could use .ToUpper() so this is not case sensitive, but for performance reasons we do not...keep this in mind if you add new cameras.
        {
            //DestroySpawnedCamera(); //DESTROY OLD CAMERA
            LoadCameraPrefab(); //LOAD CAMERA PREFAB

            //cameraSpawned = false;
            return true;
        }

        return false;
    }

    //DESTROY SPAWNED CAMERA
    void DestroySpawnedCamera()
    {
        if (CameraDolly.spawnedCamera && player.IsLocalPlayer)
        {
            GameObject.Destroy(CameraDolly.spawnedCamera); //DESTROY SPAWNED CAMERA
        }
        cameraIsSpawned = false;
    }

    //ON CAMERA CHANGED
    void LoadCameraPrefab()
    {
        cameraPrefab = Resources.Load<GameObject>("Cameras/" + cameraType.ToString() + "Camera"); //LOAD NEW CAMERA PREFAB
    }

    //SPAWN CAMERA
    void SpawnCamera()
    {
        LoadCameraPrefab(); //LOAD CAMERA PREFAB

        if (attachCameraToMe) { CameraDolly.spawnedCamera = Instantiate<GameObject>(cameraPrefab, transform); } //INSTANTIATE (PARENTED)
        else { CameraDolly.spawnedCamera = Instantiate<GameObject>(cameraPrefab); } //INSTANTIATE (UNPARENTED)

        cameraIsSpawned = CameraDolly.spawnedCamera != null;
    }

    //ATTACH EXISTING CAMERA
    /// <summary>Attaches an existing camera to the camera dolly. Disables the main camera if the dolly does not handle it.</summary>
    /// <returns></returns>
    bool AttachExistingCamera()
    {
        if (Camera.main != null) //MAIN CAMERA EXISTS?
        {
            if (System.Enum.TryParse<CameraType>(Camera.main.name.Trim("Camera".ToCharArray()), out cameraType)) //SUPPORTED CAMERA?
            {
                CameraDolly.spawnedCamera = Camera.main.gameObject;
                cameraIsSpawned = true;
                return true;
            }
            //else //DISABLE MAIN CAMERA
            //{
                //if (Camera.main && cameraPrefab != Camera.main && Camera.main.name.Contains("Main")) { Camera.main.gameObject.SetActive(false); }
                //GameObject.Destroy(Camera.main.gameObject);//.SetActive(false);
            //}
        }
        return false; //NO CAMERA IN THE SCENE
    }
#endif
}