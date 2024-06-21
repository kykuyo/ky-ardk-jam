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

    private CurrentVpsData _currentVpsData;

    private bool hasNotifiedStartTracking = false;

    public static event Action<string> OnVpsTrackingStarted;

    private void Start()
    {
        _coverageClientManager = GetComponent<CoverageClientManager>();
        _arLocationManager.locationTrackingStateChanged += OnLocationTrackingStateChanged;
        StartCoroutine(CheckDirectionRoutine());
    }

    private void LoadCurrentVps()
    {
        string currentVpsData = PlayerPrefs.GetString("CurrentVpsData");
        if (string.IsNullOrEmpty(currentVpsData))
        {
            return;
        }
        CurrentVpsData data = JsonUtility.FromJson<CurrentVpsData>(currentVpsData);
        _currentVpsData = data;

        _name.text = data.Name;

        Debug.Log($"Loading VPS: {data.Name}");
        _coverageClientManager.TryGetImageFromUrl(
            data.ImageURL,
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
                LoadCurrentVps();

                /**
                * The VPS identifier is used to uniquely identify the VPS.
                * If the VPS has an identifier, it will be used as the identifier.
                * Otherwise, the VPS name will be used as the identifier.
                * Warning: The name is not guaranteed to be unique. This is just for testing purposes.
                */
                string vpsId = !string.IsNullOrEmpty(_currentVpsData.Identifier)
                    ? _currentVpsData.Identifier
                    : Regex.Replace(_currentVpsData.Name, "[^a-zA-Z0-9]", "");

                Debug.Log($"VPS Tracking started for {vpsId}");

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
