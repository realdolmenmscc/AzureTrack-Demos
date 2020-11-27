import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AuthModule, OidcConfigService, LogLevel } from 'angular-auth-oidc-client';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';
import { LogoutComponent } from './logout/logout.component';

export function configureAuth(oidcConfigService: OidcConfigService) {
  return () =>
    oidcConfigService.withConfig({
            stsServer: 'https://login.microsoftonline.com/a8ad445d-cb71-4d2b-bedd-f4dd8fee406e',
            authWellknownEndpoint: 'https://login.microsoftonline.com/a8ad445d-cb71-4d2b-bedd-f4dd8fee406e/v2.0/.well-known/openid-configuration',
            redirectUrl: window.location.origin,
            postLogoutRedirectUri: window.location.origin,
            clientId: 'fdf59ce4-736c-4e4a-ad98-6a505a94c9d4',
            scope: 'openid api://7d21bda8-d3e0-4eba-a470-c5d9916a6671/data.read',
            responseType: 'code',
            silentRenew: true,
            maxIdTokenIatOffsetAllowedInSeconds: 600,
            issValidationOff: true, // this needs to be true if using a common endpoint in Azure
            autoUserinfo: false,
            silentRenewUrl: window.location.origin + '/silent-renew.html',
            logLevel: LogLevel.Debug
    });
}

@NgModule({
  declarations: [AppComponent, HomeComponent, UnauthorizedComponent],
  imports: [
    BrowserModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: 'logout', component: LogoutComponent },
      { path: 'unauthorized', component: UnauthorizedComponent },
    ]),
    AuthModule.forRoot(),
    HttpClientModule,
  ],
  providers: [
    OidcConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: configureAuth,
      deps: [OidcConfigService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
