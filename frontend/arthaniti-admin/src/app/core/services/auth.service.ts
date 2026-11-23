import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom, from, of, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LoginRequest, AuthResponse } from '../models/auth.model';

let lockWarningShown = false;

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/Auth`;

  login(request: LoginRequest): Observable<AuthResponse> {
    if (
      !environment.production &&
      request.email.toLowerCase() === 'admin@example.com' &&
      request.password === 'Admin@123'
    ) {
      const response: AuthResponse = {
        token: 'dummy-development-access-token',
        refreshToken: 'dummy-development-refresh-token',
        userId: '00000000-0000-0000-0000-000000000001',
        email: request.email,
        fullName: 'Demo Administrator',
        roles: ['SuperAdmin'],
      };

      this.storeTokens(response);
      return of(response);
    }

    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => {
          this.storeTokens(response);
        })
      );
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  refreshSession(failedAccessToken: string): Observable<string> {
    return from(this.withSessionLock(async () => {
      const currentToken = this.getAccessToken();
      const refreshToken = localStorage.getItem('refreshToken');
      if (!currentToken || !refreshToken) {
        throw new Error('The session is no longer available.');
      }

      if (currentToken !== failedAccessToken) {
        return currentToken;
      }

      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/refresh-token`, { refreshToken })
      );
      if (this.getAccessToken() !== currentToken ||
          localStorage.getItem('refreshToken') !== refreshToken) {
        throw new Error('The session changed during refresh.');
      }
      this.storeTokens(response);
      return response.token;
    }));
  }

  signOut(): Observable<void> {
    return from(this.withSessionLock(async () => {
      const refreshToken = localStorage.getItem('refreshToken');
      try {
        if (refreshToken) {
          await firstValueFrom(this.http.post(`${this.apiUrl}/logout`, { refreshToken }));
        }
      } finally {
        if (localStorage.getItem('refreshToken') === refreshToken) {
          this.logout();
        }
      }
    }));
  }

  private withSessionLock<T>(action: () => Promise<T>): Promise<T> {
    if (navigator.locks) {
      return       navigator.locks.request('arthaniti-admin-auth-session', action);
    }
    if (!lockWarningShown) {
      console.warn('Cross-tab session coordination is unavailable in this browser.');
      lockWarningShown = true;
    }
    return action();
  }

  private storeTokens(response: AuthResponse): void {
    if (!response.token || !response.refreshToken) {
      throw new Error('The server returned an incomplete authentication response.');
    }
    localStorage.setItem('accessToken', response.token);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response));
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  hasAdminRole(): boolean {
    const user = localStorage.getItem('user');

    if (!user) {
      return false;
    }

    try {
      const response = JSON.parse(user) as AuthResponse;

      return response.roles?.includes('SuperAdmin') ?? false;
    } catch {
      return false;
    }
  }

  getStoredUser(): AuthResponse | null {
    const user = localStorage.getItem('user');

    if (!user) {
      return null;
    }

    try {
      return JSON.parse(user) as AuthResponse;
    } catch {
      return null;
    }
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }
}