namespace Services.Vk;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed record VkRawPost(
    string VkPostId,
    string OwnerId,
    string Text,
    string PostUrl,
    string AuthorUrl,
    DateTimeOffset PublishedAt
);

public interface IVkPostProvider
{
    Task<IReadOnlyList<VkRawPost>> SearchAsync(
        string city,
        IEnumerable<string> keywords,
        DateTimeOffset since,
        CancellationToken ct = default);
}