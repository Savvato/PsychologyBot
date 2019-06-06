import { Injectable, OnInit } from '@angular/core';
import * as SignalR from '@aspnet/signalr';
import { User } from './workspace/user';
import { Message } from './workspace/message';
import { Note } from './workspace/note';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public users: User[];

  private hubConnection: SignalR.HubConnection;

  constructor(private authService: OAuthService) {}

  public startConnection() {
    this.hubConnection = new SignalR.HubConnectionBuilder()
      .withUrl('/chat',
        {
           accessTokenFactory: () => this.authService.getIdToken()
        })
      .configureLogging(SignalR.LogLevel.Debug)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.addUsersListener();
        this.addUsersListListener();
        this.addChatListener();
        this.invokeUsersList();
      })
      .catch(error => console.log('Error while starting connection: ' + error));
  }

  private addUsersListener() {
    this.hubConnection.on('userAdded',
      (user: User) => {
        console.log(`Got user ID: ${user.channelId}`);
        this.users.push(user);
      });
  }

  private addChatListener() {
    this.hubConnection.on('chatUpdate',
      (userId: String, message: Message) => {
        console.log(`Got user id: ${userId}, message: ${message.messageString}`);

        let user = this.users.find(user => user.channelId === userId);

        if (user == null) {
          console.log(`User ${userId} is not found`);
          return;
        }
        user.messages.push(message);
        if (message.isUserMessage) {
          user.hasNewMessages = true;
        }
        this.users.sort((a, b) => a.hasNewMessages === b.hasNewMessages ? 0 : a.hasNewMessages ? -1 : 1);
      });
  }

  private addUsersListListener() {
    this.hubConnection.on('allUsers',
      (users: User[]) => {
        console.log(`Got ${users.length} users`);
        this.users = users;
        this.users.sort((a, b) => a.hasNewMessages === b.hasNewMessages ? 0 : a.hasNewMessages ? -1 : 1);
      });
  }

  private invokeUsersList() {
    console.log('Invoking GetAllUsers');
    this.hubConnection.invoke('getAllUsers');
  }

  public sendMessage(user: User, message: string) {
    console.log(`Sending a message to ${user.channelId}`);
    this.hubConnection.invoke('sendMessageToUser', user.channelId, message);
  }

  public markUserMessagesAsRead(user: User) {
    console.log(`Marking user (${user.channelId}) messages as read`);
    this.hubConnection.invoke('markUserMessagesAsRead', user.channelId);
  }

  public addNoteToUser(user: User, noteText: string) {
    console.log(`Adding new note to ${user.channelId}`);
    this.hubConnection.invoke('addNoteToUser', user.channelId, noteText);
    var newNote = new Note();
    newNote.noteString = noteText;
    user.notes.push(newNote);
  }

  public removeNoteFromUser(user: User, note: Note) {
    console.log(`Removing note (${note.id}) from ${user.channelId}`);
    this.hubConnection.invoke('removeNoteFromUser', user.channelId, note.id);
    user.notes.splice(user.notes.indexOf(note), 1);
  }
}
