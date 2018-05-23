using PowerArgs;

namespace ConsoleAppServiceBus
{
    public class ApplicationArguments
    {
        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("cs")]
        public string ConnectionString { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("hp")]
        public string HubPath { get; set; }
    }
}
