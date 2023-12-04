using System.Text.Json;

namespace FeatureStateService.Models;

public enum Message
{
    FeatureStateSnapshot,
    FeatureStateChange
}

public static class MessageExtensions
{
    public static string GetName(this Message message)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(message.ToString());
    }
}