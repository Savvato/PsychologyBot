import { Component } from '@angular/core';
import { User } from '../models/user';
import { SignalRService } from '../signalr.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DetailsComponent } from './details/details.component';

@Component({
    selector: 'app-workspace',
    templateUrl: './workspace.component.html',
    styleUrls: ['./workspace.component.css']
})
export class WorkspaceComponent {
    selectedUser: User;

  constructor(public signalR: SignalRService, private modalService: NgbModal) { }

    onSelect(user: User): void {
      this.selectedUser = user;
    }

    showNotes(user: User): void {
      const detailsModal = this.modalService.open(DetailsComponent, { size: 'lg' });
      detailsModal.componentInstance.user = user;
    }
}
