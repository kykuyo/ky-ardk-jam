using System;
using Singleton.Singleton;
using UnityEngine;

public class VpsService : Singleton<VpsService>
{
    public event Action<string, VpsStatus> OnVpsStatusReceived;

    public async void GetVpsStatus(string vpsId)
    {
        VpsStatus vpsStatus = await Firebase.GetDataAsync<VpsStatus>($"/vps-status/{vpsId}");

        if (vpsStatus == null)
        {
            Debug.Log("VPS status is null");
            return;
        }

        OnVpsStatusReceived?.Invoke(vpsId, vpsStatus);
    }
}
