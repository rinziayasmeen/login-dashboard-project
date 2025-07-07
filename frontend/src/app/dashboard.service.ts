import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private apiUrl = 'http://localhost:5156/api/dashboard/data'; 

  constructor(private http: HttpClient) {}

  getChartData(): Observable<any[]> {
    const token = localStorage.getItem('jwt_token'); // Get token from login
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(this.apiUrl, { headers });
  }
}
