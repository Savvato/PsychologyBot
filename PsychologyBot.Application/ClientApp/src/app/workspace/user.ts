import { Message } from './message';
import { Note } from './note';

export class User {
    channelId: String;
    name: String;
    gender: String;
    messages: Message[];
    hasNewMessages: Boolean;
    notes: Note[];
}
