using Windows.UI.Xaml.Documents;
using CTime2.Behaviors;

namespace CTime2.Views.About
{
    public class MoreInfoTextFromResources : TextBlockTextFromResources
    {
        public MoreInfoTextFromResources()
        {
            this.Resource = "MoreInfo";
        }

        protected override void WireEvents(Span span)
        {
            base.WireEvents(span);

            var websiteHyperlink = (Hyperlink) span.FindName("CTimeWebsiteHyperlink");
            websiteHyperlink.Click += async (s, e) =>
            {
                var viewModel = (AboutViewModel) this.AssociatedObject.DataContext;
                await viewModel.OpenCTimeWebsite.ExecuteAsync();
            };

            var mailHyperlink = (Hyperlink)span.FindName("CTimeMailAddressHyperlink");
            mailHyperlink.Click += async (s, e) =>
            {
                var viewModel = (AboutViewModel)this.AssociatedObject.DataContext;
                await viewModel.SendCTimeMail.ExecuteAsync();
            };
        }
    }
}