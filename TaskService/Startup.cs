using Owin;

namespace TaskService
{
    public partial class Startup
    {
        // The OWIN middleware will invoke this method when the app starts
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
