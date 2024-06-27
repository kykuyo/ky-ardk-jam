using System;
using System.Collections;
using System.Text.RegularExpressions;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;
using Niantic.Lightship.AR.VpsCoverage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VpsTracker : MonoBehaviour
{
    [SerializeField]
    private ARLocationManager _arLocationManager;

    [SerializeField]
    private RectTransform _panel;

    [SerializeField]
    private TMP_Text _status;

    [SerializeField]
    private TMP_Text _name;

    [SerializeField]
    private RawImage _image;

    [SerializeField]
    [Range(0f, 100f)]
    private float _angleThreshold = 30f;

    [SerializeField]
    private float checkInterval = 2f;

    [SerializeField]
    private float hidePanelDelay = 2f;

    private CoverageClientManager _coverageClientManager;

    private ARLocation _arLocation;

    private bool hasNotifiedStartTracking = false;

    private AreaTarget _areaTarget;

    public static event Action<string> OnVpsTrackingStarted;

    private void Start()
    {
        _status.text = "Starting VPS tracking...";
        _coverageClientManager = GetComponent<CoverageClientManager>();
        _arLocationManager.locationTrackingStateChanged += OnLocationTrackingStateChanged;

        _areaTarget = GameManager.Instance.AreaTarget;

        LoadData(_areaTarget);

        StartTracking(_areaTarget);

        StartCoroutine(CheckDirectionRoutine());
    }

    private void StartTracking(AreaTarget areaTarget)
    {
        var anchorPayloadString = areaTarget.Target.DefaultAnchor;

        if (string.IsNullOrEmpty(anchorPayloadString))
        {
            // If this area has no anchor payload, don't do anything
            // Select a different area target in a real application
            Debug.LogError($"No anchor found for {areaTarget.Target.Name}");
            return;
        }

        var anchorPayload = new ARPersistentAnchorPayload(anchorPayloadString);

        var locationGameObject = new GameObject();
        var arLocation = locationGameObject.AddComponent<ARLocation>();
        arLocation.Payload = anchorPayload;
        _arLocationManager.SetARLocations(arLocation);
        _arLocationManager.StartTracking();
    }

    private void LoadData(AreaTarget areaTarget)
    {
        _name.text = areaTarget.Target.Name;
        _coverageClientManager.TryGetImageFromUrl(
            areaTarget.Target.ImageURL,
            downLoadedImage =>
            {
                if (downLoadedImage == null)
                {
                    return;
                }
                _image.texture = downLoadedImage;
            }
        );
    }

    private void OnLocationTrackingStateChanged(ARLocationTrackedEventArgs args)
    {
        if (args.Tracking)
        {
            _arLocation = args.ARLocation;
            _status.text = "Tracking the location...";

            if (!hasNotifiedStartTracking)
            {
                LocalizationTarget target = _areaTarget.Target;

                string vpsId = !string.IsNullOrEmpty(target.Identifier)
                    ? target.Identifier
                    : Regex.Replace(target.Name, "[^a-zA-Z0-9]", "");
                OnVpsTrackingStarted?.Invoke(vpsId);

                hasNotifiedStartTracking = true;
            }
        }
        else
        {
            _arLocation = null;
            _status.text = "Location tracking lost";
        }
    }

    private IEnumerator CheckDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CheckDirectionToARLocation();
        }
    }

    void CheckDirectionToARLocation()
    {
        if (_arLocation != null)
        {
            if (!IsLookingAtARLocation())
            {
                _panel.gameObject.SetActive(true);
                _status.text = "You are not Looking at the location (use the image as a guide)";
            }
            else
            {
                _status.text = "Tracking the location...";
                StartCoroutine(HidePanelAfterDelay());
            }
        }
    }

    private bool IsLookingAtARLocation()
    {
        if (_arLocation == null)
        {
            return false;
        }

        Vector3 position = _arLocation.transform.position;
        Vector3 directionToARLocation = position - Camera.main.transform.position;
        float angle = Vector3.Angle(Camera.main.transform.forward, directionToARLocation);

        return angle <= _angleThreshold;
    }

    IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(hidePanelDelay);
        _panel.gameObject.SetActive(false);
    }
}
