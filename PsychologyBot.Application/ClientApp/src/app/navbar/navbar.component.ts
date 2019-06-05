import { Component, OnInit, Input } from "@angular/core";

import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  @Input() userName: string;

  constructor(private authService: OAuthService) {}

  ngOnInit() {
  }

  public logout() {
    this.authService.logOut();
  }
}
