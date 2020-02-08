using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Recipes.Models;
using OrchardCore.Recipes.Services;
using YesSql;

namespace OrchardCore.Contents.Recipes
{
    /// <summary>
    /// This recipe step creates a set of content items.
    /// </summary>
    public class ContentStep : IRecipeStepHandler
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;

        public ContentStep(IContentManager contentManager, ISession session)
        {
            _contentManager = contentManager;
            _session = session;
        }

        public async Task ExecuteAsync(RecipeExecutionContext context)
        {
            if (!String.Equals(context.Name, "Content", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = context.Step.ToObject<ContentStepModel>();

            foreach (JObject token in model.Data)
            {
                var contentItem = token.ToObject<ContentItem>();
                var modifiedUtc = contentItem.ModifiedUtc;
                var publishedUtc = contentItem.PublishedUtc;
                var isPublished = contentItem.Published;
                var isLatest = contentItem.Latest;

                var existing = await _contentManager.GetVersionAsync(contentItem.ContentItemVersionId);

                if (existing == null)
                {
                    // Initializes the Id as it could be interpreted as an updated object when added back to YesSql
                    contentItem.Id = 0;
                    await _contentManager.CreateAsync(contentItem);

                    // Overwrite ModifiedUtc & PublishedUtc values that handlers have changes
                    // Should not be necessary if IContentManager had an Import method
                    contentItem.ModifiedUtc = modifiedUtc;
                    contentItem.PublishedUtc = publishedUtc;

                    // Reset published and latest to imported values as CreateAsync sets these values arbitrarily.
                    contentItem.Published = isPublished;
                    contentItem.Latest = isLatest;

                    // Resolve previous published or draft items or they will continue to be listed as published or draft.
                    var relatedContentItems = await _session
                        .Query<ContentItem, ContentItemIndex>(x =>
                            x.ContentItemId == contentItem.ContentItemId && (x.Published || x.Latest))
                        .ListAsync();

                    // Alter previous items depending on published and latest values of imported item.
                    foreach (var relatedItem in relatedContentItems)
                    {
                        // CreateAsync calls session.Save so the importing item is now resolved as part of the query.
                        if (String.Equals(relatedItem.ContentItemVersionId, contentItem.ContentItemVersionId)){
                            continue;
                        }

                        if (isPublished)
                        {
                            relatedItem.Published = false;
                        }

                        if (isLatest)
                        {
                            relatedItem.Latest = false;
                        }

                        _session.Save(relatedItem);
                    }
                }
            }
        }
    }

    public class ContentStepModel
    {
        public JArray Data { get; set; }
    }
}
