namespace InnoWatchApi
{
    public enum DataServiceApiResultCode
    {
        Success = 0,
        Failed = 1,
        Canceled = 2,
        ErrorProtocol = 1001,

        ErrorBadRequest = 2001,
        ErrorNotImplement = 2002,
        ErrorInternal = 2003,
        ErrorDataParsing = 2004,


        ErrorAuthorization = 3001,
        ErrorCannotFindUser = 3002,
        ErrorIncorrectPassword = 3003,

        ErrorCannotFindItem = 4001,

        None = 100000
    }
}