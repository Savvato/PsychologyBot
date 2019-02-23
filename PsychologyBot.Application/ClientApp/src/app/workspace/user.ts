import { Message } from './message';

export class User {
    id: Number;
    name: String;
    gender: String;
    hasFamily: Boolean;
    hasConversationTroubles: Boolean;
    messages: Message[];
}