using Grapevine.Interfaces.Server;

namespace IotDataStation.Interface
{
    public interface IAuthentication
    {
        bool ValidateUser(IHttpContext context);
        string GetLoginPageUri(IHttpContext context);
    }
}
