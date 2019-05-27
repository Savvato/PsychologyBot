import { Message } from './message';

export class User {
    channelId: String;
    name: String;
    gender: String;
    messages: Message[];
    hasNewMessages: Boolean;
    notes: String[];
}
