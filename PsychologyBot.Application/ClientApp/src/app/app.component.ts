import { Component } from '@angular/core';
import { OAuthService, JwksValidationHandler } from 'angular-oauth2-oidc';
import { SignalRService, ConnectionStatus } from './signalr.service';
import { authConfig } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  private _userName: string;

  constructor(public signalR: SignalRService, private authService: OAuthService) {
    this.authService.configure(authConfig);
    this.authService.tokenValidationHandler = new JwksValidationHandler();

    this.authService.events
      .subscribe(e => {
        if (e.type === 'token_received') {
          this.authService.loadUserProfile();
          this.signalR.startConnection();
        }
      });

    this.authService.loadDiscoveryDocumentAndLogin()
      .then(() => { 
        this.signalR.startConnection(); 
      });
  }

  public get userName(): string | null {
    if (this._userName) {
      return this._userName;
    }

    let claims = this.authService.getIdentityClaims();
    this._userName = claims ? claims['name'] : null;

    return this._userName;
  }

  public get isConnectionRejected(): boolean {
    return this.signalR.connectionStatus == ConnectionStatus.Rejected;
  }
}
