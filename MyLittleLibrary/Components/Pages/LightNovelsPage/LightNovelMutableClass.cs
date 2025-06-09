using MyLittleLibrary.Components.Shared;

namespace MyLittleLibrary.Components.Pages.LightNovelsPage
{
    public class LightNovelMutable : BookMutable
    {
        public string Volume { get; set; }
        public override object VolumeValue => Volume;

    }
}