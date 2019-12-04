using System;

using Sample.Localization;

namespace Sample.Services
{
    public static class ApplicationExceptions
    {
        public static string ToString(Exception exception)
        {
            switch (exception)
            {
                case ServerException serverException:
                    return SampleResources.Error_Business;
                case NetworkException networkException:
                    return SampleResources.Error_Network;
                default:
                    return SampleResources.Error_Unknown;
            }
        }
    }

    public class ServerException : Exception
    {
    }

    public class NetworkException : Exception
    {
    }
}