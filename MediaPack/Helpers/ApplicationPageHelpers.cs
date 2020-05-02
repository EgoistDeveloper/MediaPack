using MediaPack.Models.Channel.Enums;
using MediaPack.Models.Common;
using MediaPack.UI.Pages;
using MediaPack.ViewModel.App;
using MediaPack.ViewModel.Channel;

namespace MediaPack.Helpers
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                //case ApplicationPage.WelcomePage:
                //    return new WelcomePage(viewModel as WelcomeViewModel);
                case ApplicationPage.Tv:
                    var channelsVM = new ChannelsViewModel
                    {
                        ChannelType = ChannelType.Tv
                    };

                    channelsVM.LoadTvChannels();
                    channelsVM.LoadFavoriteTvChannels();

                    return new Channels(channelsVM);
                case ApplicationPage.Radio:
                    channelsVM = new ChannelsViewModel
                    {
                        ChannelType = ChannelType.Radio
                    };

                    channelsVM.LoadTvChannels();
                    channelsVM.LoadFavoriteTvChannels();
                    return new Channels(channelsVM);
                case ApplicationPage.TvChannel:
                    return new UI.Pages.Channel(viewModel as ChannelViewModel);
                case ApplicationPage.Settings:
                    return new AppSettings(viewModel as AppSettingsViewModel);
                default:
                    // Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Alert developer of issue
            //Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
