import { Component, OnInit } from '@angular/core';
import { User } from './user';

@Component({
  selector: 'app-workspace',
  templateUrl: './workspace.component.html',
  styleUrls: ['./workspace.component.css']
})
export class WorkspaceComponent implements OnInit {
  users: User[];
  selectedUser: User;

  constructor() { }

  ngOnInit() {
  }

}
