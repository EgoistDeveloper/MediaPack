using MediaPack.Data;
using System;
using static MediaPack.DI.DI;
using MediaPack.Helpers;
using System.IO;
using System.Windows;
using Unosquare.FFME;
using System.Windows.Input;
using System.Diagnostics;
using Unosquare.FFME.Common;
using System.Linq;
using FFmpeg.AutoGen;
using System.Text;
using Unosquare.FFME.ClosedCaptions;
using MediaPack.Models.Channel.Enums;
using MediaPack.Models.Channel.Entities;
using MediaPack.Dialogs.Channel;
using System.Windows.Threading;

namespace MediaPack.ViewModel.Channel
{
    public class ChannelViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        public ChannelViewModel()
        {
            Channel = ViewModelApplication.CurrentChannel;

            PlayerFrameVisibility = Visibility.Hidden;
            PropertiesVisibility = Visibility.Hidden;
            RootGridWidth = 40;

            ShowPlayerFrameCommand = new RelayParameterizedCommand(ShowPlayerFrame);
            ReloadStreamCommand = new RelayCommand(p => ReloadStream());
            PauseStreamCommand = new RelayCommand(p => PauseStream());
            PlayStreamCommand = new RelayCommand(p => PlayStream());
            RecordStreamCommand = new RelayCommand(p => RecordStream());
            OpenCapturesListCommand = new RelayCommand(p => OpenCapturesList());
            PageUnloadedCommand = new RelayCommand(p => PageUnloaded());
            SetCollapseStatusCommand = new RelayParameterizedCommand(SetCollapseStatus);

            MediaElement.MediaOpened += MediaElement_MediaOpenedEventArgs;
            MediaElement.MediaStateChanged += MediaElement_MediaStateChangedEventArgs;

            DispatcherTimer = new DispatcherTimer();
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            DispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            DispatcherTimer.IsEnabled = false;

            if (Channel != null && Channel.M3U8Address != null)
            {
                PlayStream(Channel.M3U8Address);
            }
        }

        #region Commands

        public ICommand ShowPlayerFrameCommand { get; set; }
        public ICommand ReloadStreamCommand { get; set; }
        public ICommand PauseStreamCommand { get; set; }
        public ICommand PlayStreamCommand { get; set; }
        public ICommand RecordStreamCommand { get; set; }
        public ICommand SetCollapseStatusCommand { get; set; }
        public ICommand OpenCapturesListCommand { get; set; }
        public ICommand PageUnloadedCommand { get; set; }

        #endregion


        #region Public Properties

        public Models.Channel.Entities.Channel Channel { get; set; }
        public bool RecordingStatus { get; set; }
        public Visibility PlayerFrameVisibility { get; set; }
        private TransportStreamRecorder StreamRecorder;
        public MediaElement MediaElement { get; set; }
        public Visibility PropertiesVisibility { get; set; }
        public double RootGridWidth { get; set; }
        public Capture CurrentCapture { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }
        public TimeSpan SpendTime { get; set; }

        #endregion


        #region Methods

        public void SetCollapseStatus(object parameter)
        {
            var status = (bool)parameter;

            PropertiesVisibility = status ? Visibility.Visible : Visibility.Hidden;
            RootGridWidth = status ? double.NaN : 40;
        }

        public void PlayStream(string m3u8Address)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    Debug.WriteLine(m3u8Address);

                    MediaElement = new MediaElement
                    {
                        Volume = .50
                    };
                    MediaElement.MediaOpening += OnMediaOpening;

                    await MediaElement.Open(new Uri(m3u8Address));
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
            });
        }

        public void PageUnloaded()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await MediaElement.Stop();
                await MediaElement.Close();

                DispatcherTimer.IsEnabled = false;
                DispatcherTimer.Stop();

                if (SpendTime.TotalSeconds > 10)
                {
                    using var db = new AppDbContext();

                    var spendTime = new SpendTime
                    {
                        ChannelId = Channel.Id,
                        Spendtime = SpendTime
                    };

                    db.SpendTimes.Add(spendTime);
                    db.SaveChanges();
                }
            });
        }

        public void ShowPlayerFrame(object parameter)
        {
            var state = bool.Parse((string)parameter);

            if (state)
            {
                PlayerFrameVisibility = Visibility.Visible;
            }
            else
            {
                PlayerFrameVisibility = Visibility.Hidden;
            }
        }

        public void ReloadStream()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await MediaElement.Stop();
                await MediaElement.Close();
                await MediaElement.Open(new Uri(Channel.M3U8Address));
            });
        }

        public void PauseStream()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await MediaElement.Stop();
                await MediaElement.Close();
            });
        }

        public void PlayStream()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await MediaElement.Open(new Uri(Channel.M3U8Address));
            });
        }

        public void OpenCapturesList()
        {
            var dialog = new CapturesListDialog();
            dialog.ShowDialogWindow(new CapturesListViewModel(dialog, Channel));
        }

        public void RecordStream()
        {
            using var db = new AppDbContext();

            var currentDir = @$"{Settings.CurrentDirectory}";

            // if already recording
            if (RecordingStatus)
            {
                StreamRecorder?.Close();
                StreamRecorder = null;

                if (CurrentCapture != null && CurrentCapture.Id > 0)
                {
                    var fileName = @$"{currentDir}\{CurrentCapture.FileName}";

                    if (File.Exists(fileName))
                    {
                        CurrentCapture.FileSize = new FileInfo(fileName).Length;

                        db.Captures.Update(CurrentCapture);
                        db.SaveChanges();
                    }
                }

                CurrentCapture = null;
            }
            // StreamRecorder is null and media is open
            else if (StreamRecorder == null && MediaElement.IsOpen)
            {
                // target recording dir and file name
                var directory = @$"Records\{Channel.ChannelType}\{Channel.Id}";
                var date = $"{DateTime.Now:yyyy-dd-M-HH-mm-ss}";
                var fileName = $@"{directory}\Record-{date}." 
                    + (Channel.ChannelType == ChannelType.Tv ? "ts" : "mp2");

                // check and create target dir
                if (!Directory.Exists(@$"{currentDir}\{directory}"))
                {
                    Directory.CreateDirectory(@$"{currentDir}\{directory}");
                }

                // start recording
                StreamRecorder = new TransportStreamRecorder(@$"{currentDir}\{fileName}", MediaElement);

                var capture = new Capture
                {
                    ChannelId = Channel.Id,
                    FileName = fileName,
                    CaptureDate = DateTime.Now
                };

                // save recording info
                db.Captures.Add(capture);
                db.SaveChanges();

                CurrentCapture = capture;

                MediaElement.RenderingVideo += MediaElement_RenderingVideo;
            }

            RecordingStatus = !RecordingStatus;
        }

        #endregion


        #region Events

        private void OnMediaOpening(object sender, MediaOpeningEventArgs e)
        {
            // Capture a reference to the MediaOptions object for real-time change
            // This usage of MediaOptions is unsupported.
            //ViewModel.CurrentMediaOptions = e.Options;

            // the event sender is the MediaElement itself
            var media = sender as MediaElement;

            // You can start off by adjusting subtitles delay
            // This defaults to 0 but you can delay (or advance with a negative delay)
            // the subtitle timestamps.
            e.Options.SubtitlesDelay = TimeSpan.Zero; // See issue #216

            // You can render audio and video as it becomes available but the downside of disabling time
            // synchronization is that video and audio will run on their own independent clocks.
            // Do not disable Time Sync for streams that need synchronized audio and video.
            e.Options.IsTimeSyncDisabled =
                e.Info.Format == "libndi_newtek" ||
                e.Info.MediaSource.StartsWith("rtsp://uno", StringComparison.OrdinalIgnoreCase);

            // You can disable the requirement of buffering packets by setting the playback
            // buffer percent to 0. Values of less than 0.5 for live or network streams are not recommended.
            e.Options.MinimumPlaybackBufferPercent = e.Info.Format == "libndi_newtek" ? 0 : 0.5;

            // The audio renderer will try to keep the audio hardware synchronized
            // to the playback position by default.
            // A few WMV files I have tested don't have continuous enough audio packets to support
            // perfect synchronization between audio and video so we simply disable it.
            // Also if time synchronization is disabled, the recommendation is to also disable audio synchronization.
            media.RendererOptions.AudioDisableSync =
                e.Options.IsTimeSyncDisabled ||
                e.Info.MediaSource.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase);

            // Legacy audio out is the use of the WinMM api as opposed to using DirectSound
            // Enable legacy audio out if you are having issues with the DirectSound driver.
            media.RendererOptions.UseLegacyAudioOut = e.Info.MediaSource.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase);

            // You can limit how often the video renderer updates the picture.
            // We keep it as 0 to refresh the video according to the native stream specification.
            media.RendererOptions.VideoRefreshRateLimit = 0;

            // Get the local file path from the URL (if possible)
            var mediaFilePath = string.Empty;
            try
            {
                var url = new Uri(e.Info.MediaSource);
                mediaFilePath = url.IsFile || url.IsUnc ? Path.GetFullPath(url.LocalPath) : string.Empty;
            }
            catch { /* Ignore Exceptions */ }

            // Example of automatically side-loading SRT subs
            if (string.IsNullOrWhiteSpace(mediaFilePath) == false)
            {
                var srtFilePath = Path.ChangeExtension(mediaFilePath, "srt");
                if (File.Exists(srtFilePath))
                    e.Options.SubtitlesSource = srtFilePath;
            }

            // You can also force video FPS if necessary
            // see: https://github.com/unosquare/ffmediaelement/issues/212
            // e.Options.VideoForcedFps = 25;

            // An example of selecting a specific subtitle stream
            var subtitleStreams = e.Info.Streams.Where(kvp => kvp.Value.CodecType == AVMediaType.AVMEDIA_TYPE_SUBTITLE).Select(kvp => kvp.Value);
            var englishSubtitleStream = subtitleStreams
                .FirstOrDefault(s => s.Language != null && s.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            if (englishSubtitleStream != null)
                e.Options.SubtitleStream = englishSubtitleStream;

            // An example of selecting a specific audio stream
            var audioStreams = e.Info.Streams.Where(kvp => kvp.Value.CodecType == AVMediaType.AVMEDIA_TYPE_AUDIO).Select(kvp => kvp.Value);
            var englishAudioStream = audioStreams
                .FirstOrDefault(s => s.Language != null && s.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            if (englishAudioStream != null)
                e.Options.AudioStream = englishAudioStream;

            // Setting Advanced Video Stream Options is also possible
            if (e.Options.VideoStream is StreamInfo videoStream)
            {
                // Example of forcing a codec for a stream
                // e.Options.DecoderCodec[videoStream.StreamIndex] = "mjpeg";

                // If we have a valid seek index let's use it!
                if (string.IsNullOrWhiteSpace(mediaFilePath) == false)
                {
                    try
                    {
                        // Try to Create or Load a Seek Index
                        var durationSeconds = e.Info.Duration.TotalSeconds > 0 ? e.Info.Duration.TotalSeconds : 0;
                        //var seekIndex = LoadOrCreateVideoSeekIndex(mediaFilePath, videoStream.StreamIndex, durationSeconds);

                        VideoSeekIndex seekIndex = null;

                        // Make sure the seek index belongs to the media file path
                        if (seekIndex != null &&
                            !string.IsNullOrWhiteSpace(seekIndex.MediaSource) &&
                            seekIndex.MediaSource.Equals(mediaFilePath, StringComparison.OrdinalIgnoreCase) &&
                            seekIndex.StreamIndex == videoStream.StreamIndex)
                        {
                            // Set the index on the options object.
                            e.Options.VideoSeekIndex = seekIndex;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception, and ignore it. Continue execution.
                        Debug.WriteLine($"Error loading seek index data. {ex.Message}");
                    }
                }

                // Hardware device priorities
                var deviceCandidates = new[]
                {
                    AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_QSV,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_DRM,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL,
                    AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC
                };

                // Hardware device selection
                if (videoStream.FPS <= 30)
                {
                    foreach (var deviceType in deviceCandidates)
                    {
                        var accelerator = videoStream.HardwareDevices.FirstOrDefault(d => d.DeviceType == deviceType);
                        if (accelerator == null) continue;
                        if (Debugger.IsAttached)
                            e.Options.VideoHardwareDevice = accelerator;

                        break;
                    }
                }

                // Start building a video filter
                var videoFilter = new StringBuilder();

                // The yadif filter de-interlaces the video; we check the field order if we need
                // to de-interlace the video automatically
                if (videoStream.IsInterlaced)
                    videoFilter.Append("yadif,");

                // Scale down to maximum 1080p screen resolution.
                if (videoStream.PixelHeight > 1080)
                {
                    // e.Options.VideoHardwareDevice = null;
                    videoFilter.Append("scale=-1:1080,");
                }

                // Example of fisheye correction filter:
                // videoFilter.Append("lenscorrection=cx=0.5:cy=0.5:k1=-0.85:k2=0.25,")
                e.Options.VideoFilter = videoFilter.ToString().TrimEnd(',');

                // Since the MediaElement control belongs to the GUI thread
                // and the closed captions channel property is a dependency
                // property, we need to set it on the GUI thread.
                media.Dispatcher?.InvokeAsync(() =>
                {
                    media.ClosedCaptionsChannel = videoStream.HasClosedCaptions ?
                        CaptionsChannel.CC1 : CaptionsChannel.CCP;
                });
            }

            // Examples of setting audio filters.
            // e.Options.AudioFilter = "aecho=0.8:0.9:1000:0.3";
            // e.Options.AudioFilter = "chorus=0.5:0.9:50|60|40:0.4|0.32|0.3:0.25|0.4|0.3:2|2.3|1.3";
            // e.Options.AudioFilter = "aphaser";
        }

        private void MediaElement_RenderingVideo(object sender, RenderingVideoEventArgs e)
        {
            const double snapshotPosition = 3;

            bool hasTakenThumb = false;

            var state = e.EngineState;

            if (state.Source == null)
                return;

            if (!state.HasMediaEnded && state.Position.TotalSeconds < snapshotPosition &&
                (!state.PlaybackEndTime.HasValue || state.PlaybackEndTime.Value.TotalSeconds > snapshotPosition))
                return;

            var thumbsDir = @$"{Settings.CurrentDirectory}\Records\Thumbnails";
            var thumbFileName = @$"{thumbsDir}\Thumbnail-{Path.GetFileNameWithoutExtension(CurrentCapture.FileName)}.jpg";

            if (MediaElement.HasVideo)
            {
                using var bmp = e.Bitmap.CreateDrawingBitmap();
                var thumbnail = ThumbnailGenerator.SnapThumbnail(bmp, thumbsDir);

                if (File.Exists($@"{thumbsDir}\{thumbnail}"))
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        CurrentCapture.Thumbnail = @$"Records\Thumbnails\{thumbnail}".PathToBitmapImage();
                    });

                    hasTakenThumb = true;
                }
            }

            if (hasTakenThumb)
            {
                MediaElement.RenderingVideo -= MediaElement_RenderingVideo;
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (MediaElement.IsOpen && MediaElement.IsPlaying)
            {
                SpendTime += TimeSpan.FromSeconds(1);
            }
        }

        private void MediaElement_MediaOpenedEventArgs(object sender, MediaOpenedEventArgs e)
        {
            if (!DispatcherTimer.IsEnabled)
            {
                DispatcherTimer.IsEnabled = true;
                DispatcherTimer.Start();
            }
        }

        private void MediaElement_MediaStateChangedEventArgs(object sender, MediaStateChangedEventArgs e)
        {
            if (e.MediaState == MediaPlaybackState.Play)
            {
                if (!DispatcherTimer.IsEnabled)
                {
                    DispatcherTimer.IsEnabled = true;
                    DispatcherTimer.Start();
                }
            }
            else if (e.MediaState == MediaPlaybackState.Stop)
            {
                if (DispatcherTimer.IsEnabled)
                {
                    DispatcherTimer.IsEnabled = false;
                    DispatcherTimer.Stop();
                }
            }
        }

        #endregion
    }
}