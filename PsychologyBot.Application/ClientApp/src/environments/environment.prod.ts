import { AuthConfig } from 'angular-oauth2-oidc';

export const environment = {
  production: true
};

export const authConfig: AuthConfig = {
  issuer: '',
  clientId: '',
  redirectUri: '',
  postLogoutRedirectUri: '',
  scope: 'openid profile',
  responseType: 'id_token token'
}
