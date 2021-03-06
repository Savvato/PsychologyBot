﻿namespace PsychologyBot.Network.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration;

    using PsychologyBot.Core.Interfaces;
    using PsychologyBot.Core.Models;

    [Authorize(policy: "OnlyPsychologists")]
    public class ChatHub : Hub
    {
        private readonly IUserRepository userRepository;
        private readonly BotFrameworkAdapter adapter;
        private readonly ConfigurationCredentialProvider credentialProvider;

        public ChatHub(IUserRepository userRepository, IAdapterIntegration adapter, ConfigurationCredentialProvider credentialProvider)
        {
            this.userRepository = userRepository;
            this.adapter = (BotFrameworkAdapter)adapter;
            this.credentialProvider = credentialProvider;
        }

        public async Task GetAllUsers()
        {
            List<User> users = await this.userRepository.GetAllUsers();
            await this.Clients.Caller.SendAsync(method: "allUsers", users);
        }

        public async Task SendMessageToUser(string userId, string text)
        {
            User user = await this.userRepository.GetUserById(userId);
            Message message = new Message
            {
                MessageString = text,
                IsUserMessage = false,
                Date = DateTime.Now
            };
            user.Messages.Add(message);
            await this.userRepository.SaveChanges();

            await this.adapter.ContinueConversationAsync(
                botAppId: this.credentialProvider.AppId,
                reference: user.ConversationReference,
                callback: async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(
                    message.MessageString,
                    cancellationToken: cancellationToken),
                cancellationToken: default);

            await this.Clients.All.SendAsync(method: "chatUpdate", arg1: userId, arg2: message);
        }

        public async Task MarkUserMessagesAsRead(string userId)
        {
            User user = await this.userRepository.GetUserById(userId);
            user.HasNewMessages = false;
            await this.userRepository.SaveChanges();
        }

        public async Task AddNoteToUser(string userId, string noteText)
        {
            User user = await this.userRepository.GetUserById(userId);
            Note note = new Note
            {
                NoteString = noteText,
                Date = DateTime.Now
            };
            user.Notes.Add(note);
            await this.userRepository.SaveChanges();
        }

        public async Task RemoveNoteFromUser(string userId, int noteId)
        {
            User user = await this.userRepository.GetUserById(userId);
            Note noteToRemove = user.Notes.Find(n => n.Id == noteId);
            user.Notes.Remove(noteToRemove);
            await this.userRepository.SaveChanges();
        }
    }
}
