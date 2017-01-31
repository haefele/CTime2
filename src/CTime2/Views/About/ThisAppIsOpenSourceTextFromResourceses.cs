using Windows.UI.Xaml.Documents;
using CTime2.Behaviors;

namespace CTime2.Views.About
{
    public class ThisAppIsOpenSourceTextFromResourceses : TextBlockTextFromResources
    {
        public ThisAppIsOpenSourceTextFromResourceses()
        {
            this.Resource = "ThisAppIsOpenSource";
        }

        protected override void WireEvents(Span span)
        {
            base.WireEvents(span);
            
            var githubHyperlink = (Hyperlink)span.FindName("GitHubHyperlink");
            githubHyperlink.Click += async (s, e) =>
            {
                var viewModel = (AboutViewModel) this.AssociatedObject.DataContext;
                await viewModel.OpenGitHubProject.ExecuteAsync();
            };
        }
    }
}