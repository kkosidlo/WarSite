using Hangfire;
using Owin;
using System;
using System.Threading.Tasks;
using WarStarts.Jobs;

namespace WarStarts
{
    public class Startup : Base
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => new DeathListCounter().Proceed(), Cron.MinuteInterval(7));
            RecurringJob.AddOrUpdate(() => new MembersListCounter().Proceed(), Cron.HourInterval(2));
        }
    }
}