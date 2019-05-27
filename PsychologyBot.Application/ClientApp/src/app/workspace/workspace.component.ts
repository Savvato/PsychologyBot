import { Component, OnInit } from '@angular/core';
import { User } from './user';
import { SignalRService } from '../signalr.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DetailsComponent } from './details/details.component';

@Component({
    selector: 'app-workspace',
    templateUrl: './workspace.component.html',
    styleUrls: ['./workspace.component.css']
})
export class WorkspaceComponent implements OnInit {
    selectedUser: User;

  constructor(public signalR: SignalRService, private modalService: NgbModal) { }

    ngOnInit() {
      this.signalR.startConnection();
    }

    onSelect(user: User): void {
      this.selectedUser = user;
    }

    showNotes(user: User): void {
      const detailsModal = this.modalService.open(DetailsComponent, { size: 'lg' });
      detailsModal.componentInstance.user = user;
    }
}
