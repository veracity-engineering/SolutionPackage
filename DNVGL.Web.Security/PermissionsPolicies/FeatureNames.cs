using System.Collections.Generic;

namespace DNVGL.Web.Security.PermissionsPolicies
{
    public static class FeatureNames
    {
        //Standardized Features
        public const string Accelerometer = "accelerometer";
        public const string AmbientLightSensor = "ambient-light-sensor";
        public const string Autoplay = "autoplay";
        public const string Battery = "battery";
        public const string Camera = "camera";
        public const string CrossOriginIsolated = "cross-origin-isolated";
        public const string DisplayCapture = "display-capture";
        public const string DocumentDomain = "document-domain";
        public const string EncryptedMedia = "encrypted-media";
        public const string ExecutionWhileNotRendered = "execution-while-not-rendered";
        public const string ExecutionWhileOutOfViewport = "execution-while-out-of-viewport";
        public const string Fullscreen = "fullscreen";
        public const string Geolocation = "geolocation";
        public const string Gyroscope = "gyroscope";
        public const string Magnetometer = "magnetometer";
        public const string Microphone = "microphone";
        public const string Midi = "midi";
        public const string NavigationOverride = "navigation-override";
        public const string Payment = "payment";
        public const string PictureInPicture = "picture-in-picture";
        public const string PublickeyCredentialsGet = "publickey-credentials-get";
        public const string ScreenWakeLock = "screen-wake-lock";
        public const string Usb = "usb";
        public const string WebShare = "web-share";
        public const string XrSpatialTracking = "xr-spatial-tracking";

        //Proposed Features
        public const string ClipboardRead = "clipboard-read";
        public const string ClipboardWrite = "clipboard-write";
        public const string Gamepad = "gamepad";
        public const string SpeakerSelection = "speaker-selection";

        //Experimental Features
        public const string ConversionMeasurement = "conversion-measurement"; 
        public const string FocusWithoutUserActivation = "focus-without-user-activation";
        public const string Hid = "hid";
        public const string IdleDetection = "idle-detection";
        public const string Serial = "serial";
        public const string SyncScript = "sync-script";
        public const string TrustTokenRedemption = "trust-token-redemption";
        public const string VerticalScroll = "vertical-scroll";

        private static List<string> features = new List<string>
        {
            Accelerometer
            ,AmbientLightSensor
            ,Autoplay
            ,Battery
            ,Camera
            ,CrossOriginIsolated
            ,DisplayCapture
            ,DocumentDomain
            ,EncryptedMedia
            ,ExecutionWhileNotRendered
            ,ExecutionWhileOutOfViewport
            ,Fullscreen
            ,Geolocation
            ,Gyroscope
            ,Magnetometer
            ,Microphone
            ,Midi
            ,NavigationOverride
            ,Payment
            ,PictureInPicture
            ,PublickeyCredentialsGet
            ,ScreenWakeLock
            ,Usb
            ,WebShare
            ,XrSpatialTracking

            ,ClipboardRead
            ,ClipboardWrite
            ,Gamepad
            ,SpeakerSelection

            ,ConversionMeasurement
            ,FocusWithoutUserActivation
            ,Hid
            ,IdleDetection
            ,Serial
            ,SyncScript
            ,TrustTokenRedemption
            ,VerticalScroll
        };
        public static List<string> All {
            get { return features; }
        }
        
    }
}

