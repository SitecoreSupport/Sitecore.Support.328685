namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Extensions
{
    using Sitecore.Resources.Media;
    using Sitecore.Data.Items;
    using Sitecore.Configuration;

    public static class ItemExtensions
    {
        public static string SitecoreSupportBuildAssetPath(this Item item, bool addTimestamp = false) // renamed method for patching purpose
        {
            string mediaUrl = MediaManager.GetMediaUrl(item, new MediaUrlOptions
            {
                Thumbnail = true
            });
            mediaUrl = mediaUrl.Replace("&thn=1", string.Empty);
            mediaUrl = mediaUrl.Replace("?thn=1&", "?");
            mediaUrl = mediaUrl.Replace("?thn=1", string.Empty);
            mediaUrl = (mediaUrl.Contains("://") ? mediaUrl : StringUtil.EnsurePrefix('/', mediaUrl));
            if (addTimestamp)
            {
                mediaUrl = mediaUrl + "?t=" + item[Sitecore.XA.Foundation.SitecoreExtensions.Templates.Statistics.Fields.__Created];
            }
            #region Fix 328685  start
            if (Settings.Media.AlwaysAppendRevision)
            {
                return HashingUtils.ProtectAssetUrl(mediaUrl); // Protect asset to fix 328685
            }
            #endregion
            return mediaUrl;
        }
    }
}