import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from '../user';
import { Note } from '../note';
import { SignalRService } from '../../signalr.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css', "../workspace.component.css"]
})
export class DetailsComponent {
  @Input() user: User;
  
  constructor(public signalR: SignalRService, public activeModal: NgbActiveModal) { }

  addNote() {
    var noteText = (<HTMLTextAreaElement>document.getElementById("newNote")).value;
    this.signalR.addNoteToUser(this.user, noteText);
  }

  removeNote(note: Note) {
    this.signalR.removeNoteFromUser(this.user, note);
  }

}
