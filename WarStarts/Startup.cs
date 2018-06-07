using Hangfire;
using Owin;
using System;
using System.Threading.Tasks;
using WarStarts.Jobs;

namespace WarStarts
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("Data Source=SQL6003.site4now.net;Initial Catalog=DB_A3C068_showland;User Id=DB_A3C068_showland_admin;Password=kacperQ123;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => new DeathListCounter().Proceed(), Cron.MinuteInterval(5));
            RecurringJob.AddOrUpdate(() => new MembersListCounter().Proceed(), Cron.HourInterval(1));
        }
    }
}