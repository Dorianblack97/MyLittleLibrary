using MyLittleLibrary.Components.Shared;

namespace MyLittleLibrary.Components.Pages.MangasPage
{
    public class MangaMutable : BookMutable
    {
        public int Volume { get; set; } = 1;
    
        public override object VolumeValue => Volume;

    }
}