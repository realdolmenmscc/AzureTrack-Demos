import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated$: Observable<boolean>;
  constructor(
    private authservice: AuthService,
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
    this.userData$ = this.authservice.userData;
    this.isAuthenticated$ = this.authservice.signedIn;
  }

  callApiGet() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44390/api/weatherforecast')
      .pipe(catchError((error) => of(error)));
  }

  callApiGetUsers() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44390/api/users')
      .pipe(catchError((error) => of(error)));
  }

  callApiPost() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .post('https://localhost:44390/api/weatherforecast', { date: new Date(), summary: 'test', temperatureC: 10 })
      .pipe(catchError((error) => of(error)));
  }

  login() {
    this.authservice.signIn();
  }

  forceRefreshSession() {
    this.authservice.forceRefreshSession().subscribe((data) => {
      console.log('Refresh completed');
    });
  }

  logout() {
    this.authservice.signOut();
  }
}
