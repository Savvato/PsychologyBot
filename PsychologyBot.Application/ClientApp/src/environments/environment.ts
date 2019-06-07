import { AuthConfig } from 'angular-oauth2-oidc';

// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
};

export const authConfig: AuthConfig = {
  issuer: 'https://devidentity.akvelon.net:5003',
  clientId: '516ab147-4501-4976-b216-d6b41ba5ba8e',
  redirectUri: 'http://localhost:3978',
  postLogoutRedirectUri: 'http://localhost:3978',
  scope: 'openid profile',
  responseType: 'id_token token'
}

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
