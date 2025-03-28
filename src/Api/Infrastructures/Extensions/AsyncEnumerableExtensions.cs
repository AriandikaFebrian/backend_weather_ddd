// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Api.Infrastructures.Extensions;

/// <summary>
/// AsyncEnumerableExtensions
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// ToListAsync
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            return await AsyncEnumerable.ToListAsync(source).ConfigureAwait(false);
        }
    }
}
