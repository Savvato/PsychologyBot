import { Component } from '@angular/core';
import { AuthConfig, OAuthService, JwksValidationHandler } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: 'https://devidentity.akvelon.net:5003',
  clientId: '516ab147-4501-4976-b216-d6b41ba5ba8e',
  redirectUri: 'http://localhost:3978',
  postLogoutRedirectUri: 'http://localhost:3978',
  scope: 'openid profile',
  responseType: 'id_token token'
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  private _userName: string;

  constructor(private authService: OAuthService) {
    this.authService.configure(authConfig);
    this.authService.tokenValidationHandler = new JwksValidationHandler();

    this.authService.events
      .subscribe(e => {
        if (e.type === 'token_received') {
          this.authService.loadUserProfile();
        }
      });

    this.authService.loadDiscoveryDocumentAndLogin();
  }

  public get userName(): string | null {
    if (this._userName) {
      return this._userName;
    }

    let claims = this.authService.getIdentityClaims();
    this._userName = claims ? claims['name'] : null;

    return this._userName;
  }
}
