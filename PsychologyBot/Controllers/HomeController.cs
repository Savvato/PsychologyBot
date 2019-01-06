namespace PsychologyBot.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration;

    using PsychologyBot.ViewModels;

    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ConfigurationCredentialProvider credentialProvider;
        private readonly BotFrameworkAdapter adapter;

        public HomeController(
            IUserRepository userRepository,
            ConfigurationCredentialProvider credentialProvider,
            IAdapterIntegration adapter)
        {
            this.userRepository = userRepository;
            this.credentialProvider = credentialProvider;
            this.adapter = (BotFrameworkAdapter) adapter;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View(new UserViewModel()
            {
                AllUsers = this.userRepository.GetAllUsers(),
            });
        }

        [HttpGet]
        public IActionResult User(string id)
        {
            return this.View(new UserViewModel()
            {
                AllUsers = this.userRepository.GetAllUsers(),
                SelectedUser = this.userRepository.GetUserById(id),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Send(string id, MessageState messageState)
        {
            User user = this.userRepository.GetUserById(id);

            Message message = new Message(messageState.MessageString, false);

            user.Messages.Add(message);

            await this.adapter.ContinueConversationAsync(
                credentialProvider.AppId,
                user.ConversationReference,
                async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(
                    message.MessageString,
                    cancellationToken: cancellationToken),
                default(CancellationToken));

            return this.RedirectToAction("User", new { id = id });
        }
    }
}