using AutoMapper;
using CsvHelper;
using Microsoft.Azure.NotificationHubs;
using PowerArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppServiceBus
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InitializeAutoMapper();
                var parsed = Args.Parse<ApplicationArguments>(args);

                var hubManager = new HubManager(parsed.ConnectionString, parsed.HubPath);
                var registrations = hubManager.GetRegistrationsAsync().Result;
                WriteFile(registrations);
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<ApplicationArguments>());
            }
        }

        private static void InitializeAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<RegistrationDescription, Registration>()
                .ForMember(m => m.RegistrationId, m => m.MapFrom(p => p.RegistrationId))
                .ForMember(m => m.Tags, m => m.MapFrom(p => string.Join(",", p.Tags)))
                .AfterMap((desc, reg) =>
                {
                    if (desc is GcmRegistrationDescription)
                    {
                        reg.DeviceType = "gcm";
                        reg.AppleOrGcmToken = ((GcmRegistrationDescription)desc).GcmRegistrationId;
                    }
                    if (desc is AppleRegistrationDescription)
                    {
                        reg.DeviceType = "apple";
                        reg.AppleOrGcmToken = ((AppleRegistrationDescription)desc).DeviceToken;
                    }
                });
            });
        }

        private static void WriteFile(List<Registration> registrations)
        {
            using (var csv = new CsvWriter(new StreamWriter(@"export.csv")))
            {
                csv.Configuration.Delimiter = ";";
                csv.WriteRecords(registrations);
            }
        }
    }
}
