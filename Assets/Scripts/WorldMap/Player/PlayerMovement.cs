using System;
using System.Collections;
using UnityEngine;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using UnityEngine.EventSystems;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private LightshipMapView _lightshipMapView;

    [SerializeField]
    private float _editorMovementSpeed;

    //[SerializeField]
    //private MapPlayer _model;

    private double _lastGpsUpdateTime;
    private Vector3 _targetMapPosition;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private Vector3 _currentMapPosition;
    private float _lastMapViewUpdateTime;
    private bool _isFirstClick = false;
    private float _timeSinceFirstClick = 0f;
    private const float DoubleClickTimeWindow = 0.25f;
    private const float WalkThreshold = 0.5f;
    private const float TeleportThreshold = 200f;
    public Action<string> OnGpsError;

    public static event Action<LatLng> OnPlayerMoved;

    private static bool IsLocationServiceInitializing =>
        Input.location.status == LocationServiceStatus.Initializing;

    private void Start()
    {
        _lightshipMapView.MapOriginChanged += OnMapViewOriginChanged;
        _lightshipMapView.MapCenterChanged += OnMapCenterChanged;
        _currentMapPosition = _targetMapPosition = transform.position;
        /* Disabled for now
        StartCoroutine(UpdateGpsLocation());
        */
    }

    private void OnDestroy()
    {
        _lightshipMapView.MapOriginChanged -= OnMapViewOriginChanged;
        _lightshipMapView.MapCenterChanged -= OnMapCenterChanged;
    }

    private void OnDisable()
    {
        _lightshipMapView.MapOriginChanged -= OnMapViewOriginChanged;
        _lightshipMapView.MapCenterChanged -= OnMapCenterChanged;
        //_model.UpdateMovement(0);
    }

    private void OnMapCenterChanged(LatLng center)
    {
        OnPlayerMoved?.Invoke(center);
    }

    private void OnMapViewOriginChanged(LatLng center)
    {
        Vector3 offset = _targetMapPosition - _currentMapPosition;
        _currentMapPosition = _lightshipMapView.LatLngToScene(center);
        _targetMapPosition = _currentMapPosition + offset;
    }

    private IEnumerator UpdateGpsLocation()
    {
        yield return null;

        if (!Application.isEditor)
        {
            yield return null;
        }

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return new WaitForSeconds(1.0f);
            }
        }
#endif
        if (!Input.location.isEnabledByUser)
        {
            OnGpsError?.Invoke("Location permission not enabled");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (IsLocationServiceInitializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            OnGpsError?.Invoke("GPS initialization timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            OnGpsError?.Invoke("Unable to determine device location");
            yield break;
        }

        while (isActiveAndEnabled)
        {
            LocationInfo gpsInfo = Input.location.lastData;
            if (gpsInfo.timestamp > _lastGpsUpdateTime)
            {
                _lastGpsUpdateTime = gpsInfo.timestamp;
                LatLng location = new(gpsInfo.latitude, gpsInfo.longitude);
                UpdatePlayerLocation(location);
            }

            yield return null;
        }
        Input.location.Stop();
    }

    private void UpdatePlayerLocation(in LatLng location)
    {
        _targetMapPosition = _lightshipMapView.LatLngToScene(location);
    }

    public void Update()
    {
        UpdateMapViewPosition();

        if (_isMoving)
        {
            MoveTowardsTargetPosition();
        }

        // HandleDoubleClickInput();

        Vector3 movementVector = _targetPosition - _currentMapPosition;
        float movementDistance = movementVector.magnitude;

        /* Disabled for now
        switch (movementDistance)
        {
            case > TeleportThreshold:
                _currentMapPosition = _targetMapPosition;
                break;

            case > WalkThreshold:
            {
                // If the player is not stationary,
                // rotate to face their movement vector
                Vector3 forward = movementVector.normalized;
                Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = rotation;
                break;
            }
        }
        */

        _currentMapPosition = Vector3.Lerp(_currentMapPosition, _targetMapPosition, Time.deltaTime);

        transform.position = _currentMapPosition;
        //_model.UpdateMovement(movementDistance);
    }

    private void HandleDoubleClickInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isFirstClick)
            {
                _isFirstClick = true;
                _timeSinceFirstClick = Time.time;
            }
            else
            {
                if (Time.time - _timeSinceFirstClick <= DoubleClickTimeWindow)
                {
                    HandleMouseInput();
                    _isFirstClick = false;
                }
                else
                {
                    _timeSinceFirstClick = Time.time;
                }
            }
        }
    }

    private void HandleMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Camera camera = GetActiveHighestPriorityCamera();

        if (!camera.CompareTag("OrbitCamera"))
        {
            return;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        float mapHeight = 0.0f;
        Plane plane = new(Vector3.up, new Vector3(0, mapHeight, 0));
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _targetPosition = hitPoint;
            _isMoving = true;
        }
    }

    /**
     * TODO: Move this to a utility class
     * CONSIDER: Use a more robust method of determining the highest priority camera
     */
    private Camera GetActiveHighestPriorityCamera()
    {
        Camera[] cameras = Camera.allCameras;
        Camera highestPriorityCamera = null;

        foreach (var camera in cameras)
        {
            if (
                camera.isActiveAndEnabled
                && (highestPriorityCamera == null || camera.depth > highestPriorityCamera.depth)
            )
            {
                highestPriorityCamera = camera;
            }
        }

        return highestPriorityCamera;
    }

    private void MoveTowardsTargetPosition()
    {
        Vector3 direction = (_targetPosition - _currentMapPosition).normalized;
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * _editorMovementSpeed
            );
        }

        Vector3 newPosition =
            _currentMapPosition + direction * (_editorMovementSpeed * Time.deltaTime);

        if (Vector3.Distance(newPosition, _targetPosition) < 2f)
        {
            _isMoving = false;
            newPosition = _targetPosition;
        }

        _currentMapPosition = newPosition;
        _targetMapPosition = _currentMapPosition;
    }

    private void UpdateMapViewPosition()
    {
        if (Time.time < _lastMapViewUpdateTime + 1.0f)
        {
            return;
        }

        _lastMapViewUpdateTime = Time.time;
        _lightshipMapView.SetMapCenter(transform.position);
    }
}
