using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace web.Controllers
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<Jobclass>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithCronSchedule("0 50 23 ? * *")
            .Build();
            

            //    .WithSimpleSchedule(x => x
            //.WithIntervalInMinutes(10)
            //.RepeatForever())

            scheduler.ScheduleJob(job, trigger);
        }
    }
}