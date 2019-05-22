import { Injectable, OnInit } from '@angular/core';
import * as SignalR from "@aspnet/signalr";
import { User } from './workspace/user';
import { Message } from './workspace/message';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    public users: User[];

    private hubConnection: SignalR.HubConnection;

    constructor() { }

    public startConnection() {
        this.hubConnection = new SignalR.HubConnectionBuilder()
            .withUrl('/chat')
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
        this.hubConnection.on('userAdded', (user: User) => {
            console.log(`Got user ID: ${user.id}`);
            this.users.push(user);
        });
    }

    private addChatListener() {
        this.hubConnection.on('chatUpdate', (userId: String, message: Message) => {
            console.log(`Got user id: ${userId}, message: ${message.messageString}`);

            let user = this.users.find(user => user.id == userId);

            if (user == null) {
                console.log(`User ${userId} is not found`);
            }

            user.messages.push(message);
        });
    }

    private addUsersListListener() {
        this.hubConnection.on('allUsers', (users: User[]) => {
            console.log(`Got ${users.length} users`);
            this.users = users;
        });
    }

    private invokeUsersList() {
        console.log('Invoking GetAllUsers');
        this.hubConnection.invoke('getAllUsers');
    }

    public sendMessage(user: User, message: string) {
        console.log(`Sending a message to ${user.id}`);
        this.hubConnection.invoke('sendMessageToUser', user.id, message);
    }
}
