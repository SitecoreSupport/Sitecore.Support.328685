namespace Sitecore.Support.XA.Foundation.Theming.Bundler
{
    using Sitecore.Data.Items;
    using Sitecore.XA.Foundation.Theming.Configuration;
    using Sitecore.XA.Foundation.SitecoreExtensions.Comparers;
    using Sitecore.XA.Foundation.Theming;
    using Sitecore.XA.Foundation.Theming.Bundler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Sitecore.Support.XA.Foundation.SitecoreExtensions.Extensions;
    using Sitecore.XA.Foundation.Theming.EventHandlers;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
    using Sitecore.XA.Foundation.Abstractions;
    using System.Web;
    using System.Web.Caching;

    public class AssetLinksGenerator : Sitecore.XA.Foundation.Theming.Bundler.AssetLinksGenerator
    {

        private readonly TreeComparer _comparer;
        private readonly AssetConfiguration _configuration;
        protected IContentRepository ContentRepository
        {
            get;
        }

        protected IDatabaseRepository DatabaseRepository
        {
            get;
        }

        protected IContext Context
        {
            get;
        }
        private bool IsNotEmpty(Item optimizedScriptItem)
        {
            using (Stream stream = ((MediaItem)optimizedScriptItem).GetMediaStream())
            {
                return stream != null && stream.Length > 0;
            }
        }
        private void InitializeCache()
        {
            if (HttpContext.Current.Cache["XA.AssetsService.Content.Version.Master"] == null)
            {
                HttpContext.Current.Cache.Insert("XA.AssetsService.Content.Version.Master", 0, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            if (HttpContext.Current.Cache["XA.AssetsService.Content.Version.Web"] == null)
            {
                HttpContext.Current.Cache.Insert("XA.AssetsService.Content.Version.Web", 0, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
        }
        public AssetLinksGenerator()
        {
            _comparer = new TreeComparer();
            _configuration = AssetConfigurationReader.Read();
            ContentRepository = ServiceLocator.ServiceProvider.GetService<IContentRepository>();
            DatabaseRepository = ServiceLocator.ServiceProvider.GetService<IDatabaseRepository>();
            Context = ServiceLocator.ServiceProvider.GetService<IContext>();
            InitializeCache();
        }
        protected override string GetOptimizedItemLink(Item theme, OptimizationType type, AssetServiceMode mode, string query, string fileName)// Override original method
        {
            query = string.Format(query, Templates.OptimizedFile.ID, fileName);
            Item item = theme.Axes.SelectSingleItem(query);
            if (item != null && IsNotEmpty(item))
            {
                return item.SitecoreSupportBuildAssetPath(true);
            }
            return new AssetBundler().GetOptimizedItemPath(theme, type, mode);
        }

        protected override IEnumerable<string> QueryAssets(Item theme, string query)// Override original method
        {
            Item[] array = theme.Axes.SelectItems(query);
            if (array != null)
            {
                List<Item> list = array.ToList();
                list.Sort(_comparer);
                return from i in list
                       where i.TemplateID != Templates.OptimizedFile.ID
                       select i.SitecoreSupportBuildAssetPath();
            }
            return Enumerable.Empty<string>();
        }
        public new static AssetLinks GenerateLinks(IThemesProvider themesProvider) // main entry point
        {
            if (AssetContentRefresher.IsPublishing() || IsAddingRendering())
            {
                return new AssetLinks();
            }
            return new AssetLinksGenerator().GenerateAssetLinks(themesProvider);
        }
    }
}