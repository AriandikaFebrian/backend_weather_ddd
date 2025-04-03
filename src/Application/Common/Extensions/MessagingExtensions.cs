// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// MessagingExtensions
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// GetTopicValue
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    /// <param name="topicName"></param>
    /// <returns></returns>
    public static string GetTopicValue(this AppSetting value, string name, string topicName)
    {
        if (value == null)
        {
            return null;
        }

        var topicData = value.Messaging.AzureEventHubs.Find(x => x.Name.Equals(name))?.Topics;
        var topic = topicData.Find(x => x.Name.Equals(topicName))?.Value;

        return topic;
    }
}
