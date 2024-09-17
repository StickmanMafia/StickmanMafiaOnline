public static class ChatMessageArgumentsService
{
    public static object[] GetPlayerMessageArguments(string header, string message) => new object[]
    {
        "chat-message",
        header,
        message
    };

    public static object[] GetActionImpactMessageArguments(string nickname, string messageLocalizedKey) => new object[]
    {
        nickname,
        messageLocalizedKey
    };

    public static object[] GetQuickMessageArguments(string targetNickname, string senderNickname) => new object[]
    {
        targetNickname, 
        senderNickname
    };

    public static object[] GetDisguisedMessageArguments(string targetNickname, string localizedString) => new object[]
    {
        targetNickname,
        localizedString
    };
}