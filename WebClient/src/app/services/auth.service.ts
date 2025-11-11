import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environments';

interface LoginResponse {
  token: string;
  tokenType: string;
  expiresAtUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly key = 'cpm.jwt';
  private readonly base = `${environment.apiBaseUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http
      .post<LoginResponse>(`${this.base}/login`, {
        Username: username,
        Password: password,
      })
      .pipe(tap((r) => localStorage.setItem(this.key, r.token)));
  }

  logout() {
    localStorage.removeItem(this.key);
  }
  get token() {
    return localStorage.getItem(this.key);
  }
  get isAuthenticated() {
    return !!this.token;
  }
}
