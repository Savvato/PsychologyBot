import { Component, OnInit, Input } from "@angular/core";
import { User } from "../user";
import { SignalRService } from "../../signalr.service";

@Component({
  selector: "app-chat",
  templateUrl: "./chat.component.html",
  styleUrls: ["./chat.component.css", "../workspace.component.css"]
})
export class ChatComponent implements OnInit {
  @Input() user: User;
  input: string;

  constructor(public signalR: SignalRService) { }

  ngOnInit() {
  
  }

  ngOnChanges() {
    if (this.user != null) {
      this.user.messages.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
      this.user.hasNewMessages = false;
      this.signalR.readNewMessages(this.user);
      this.scrollToBottom();
    }
  }

  async scrollToBottom() {
    setTimeout(() => {
      const chatCard = document.getElementById("chat");
      chatCard.scrollTop = chatCard.scrollHeight;
    }, 500);

  }

  onSubmit() {
    this.signalR.sendMessage(this.user, this.input);
    this.user.hasNewMessages = false;
    this.signalR.readNewMessages(this.user);
    this.input = "";
  }

}
