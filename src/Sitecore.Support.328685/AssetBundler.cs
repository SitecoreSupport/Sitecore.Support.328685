namespace Sitecore.Support.XA.Foundation.Theming.Bundler
{
    using Sitecore.Data.Items;
    using Sitecore.XA.Foundation.Theming.Configuration;
    using Sitecore.Support.XA.Foundation.SitecoreExtensions.Extensions;

    public class AssetBundler : Sitecore.XA.Foundation.Theming.Bundler.AssetBundler
    {
        public override string GetOptimizedItemPath(Item theme, OptimizationType type, AssetServiceMode mode)
        {
            Item optimizedItem = GetOptimizedItem(theme, type, mode);
            if (optimizedItem != null)
            {
                return optimizedItem.SitecoreSupportBuildAssetPath(true);
            }
            return string.Empty;
        }
    }
}