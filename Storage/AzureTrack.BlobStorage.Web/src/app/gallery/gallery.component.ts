import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { switchMap, tap, map } from 'rxjs/operators';
import { BlobModel } from '../../models';
import { saveAs } from 'file-saver';
import { Router } from '@angular/router';

@Component({
  selector: 'app-gallery',
  templateUrl: './gallery.component.html'
})
export class GalleryComponent implements OnInit {
  category: string = this.router.url.substring(1);
  blobs$: Observable<BlobModel[]>;
  download$ = new Subject<string>();
  downloadZip$ = new Subject();

  constructor(private http: HttpClient, @Inject('API_URL') private apiUrl: string,
    private router: Router) { }

  ngOnInit() {
    this.blobs$ = this.http.get<BlobModel[]>(`${this.apiUrl}/blobs/${this.category}`);
    const headers = new HttpHeaders({ 'Content-Type': 'application/octet-stream' });

    this.download$.pipe(
      switchMap(name => this.http.get(`${this.apiUrl}/blobs/${this.category}/${name}`,
        { headers, responseType: 'blob' }).pipe(
          tap(blob => saveAs(blob, name, {
            type: 'application/octet-stream'
          }))
        ))
    ).subscribe();

    this.downloadZip$.pipe(
      switchMap(_ => this.http.get(`${this.apiUrl}/blobs/zip-collection/${this.category}.zip`, { headers, responseType: 'blob' }).pipe(
        tap(blob => saveAs(blob, `${this.category}.zip`, {
          type: 'application/octet-stream'
        }))
      ))
    ).subscribe();
  }
}
