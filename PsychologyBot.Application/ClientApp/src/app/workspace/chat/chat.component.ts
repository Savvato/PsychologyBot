import { Component, OnInit, Input } from '@angular/core';
import { User } from '../user';
import { SignalRService } from "../../signalr.service";

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  @Input() user: User;
  public input: string;

  constructor(public signalR: SignalRService) { }

  ngOnInit() { }

  onSubmit() {
    this.signalR.sendMessage(this.user, this.input);
    this.input = '';
  }

}
