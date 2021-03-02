using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OnlineCoaching.Notifications.Startup))]

namespace OnlineCoaching.Notifications
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //enable singnalR in application
            app.MapSignalR();
        }
    }
}
