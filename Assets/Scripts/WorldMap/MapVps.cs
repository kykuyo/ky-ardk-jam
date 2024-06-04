using Niantic.Lightship.AR.VpsCoverage;

public class MapVps : MapEntity
{
    public AreaTarget areaTarget;

    protected override void OnPlayerInteract()
    {
        VpsContainerUI vpsContainerUI = FindObjectOfType<VpsContainerUI>();
        vpsContainerUI.ShowVpsContainerUI(areaTarget);
    }
}
