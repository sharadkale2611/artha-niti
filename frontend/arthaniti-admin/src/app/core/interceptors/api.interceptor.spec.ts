import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';
import { apiInterceptor } from './api.interceptor';

describe('authentication refresh interceptor', () => {
  let http: HttpClient;
  let requests: HttpTestingController;
  const api = environment.apiUrl.replace(/\/+$/, '');
  const wait = () => new Promise(resolve => setTimeout(resolve, 0));

  beforeEach(() => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'old-access');
    localStorage.setItem('refreshToken', 'old-refresh');
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        provideHttpClient(withInterceptors([apiInterceptor])),
        provideHttpClientTesting()
      ]
    });
    http = TestBed.inject(HttpClient);
    requests = TestBed.inject(HttpTestingController);
    vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
  });

  afterEach(() => {
    requests.verify();
    localStorage.clear();
    vi.restoreAllMocks();
    TestBed.resetTestingModule();
  });

  it('shares a refresh and retries concurrent and delayed failures with the rotated tokens', async () => {
    const first = firstValueFrom(http.get(`${api}/first`));
    const second = firstValueFrom(http.get(`${api}/second`));
    const delayed = firstValueFrom(http.get(`${api}/delayed`));
    const firstRequest = requests.expectOne(`${api}/first`);
    const secondRequest = requests.expectOne(`${api}/second`);
    const delayedRequest = requests.expectOne(`${api}/delayed`);
    firstRequest.flush({}, { status: 401, statusText: 'Unauthorized' });
    secondRequest.flush({}, { status: 401, statusText: 'Unauthorized' });
    await wait();
    const refresh = requests.expectOne(`${api}/Auth/refresh-token`);
    expect(refresh.request.body).toEqual({ refreshToken: 'old-refresh' });
    refresh.flush({
      token: 'new-access', refreshToken: 'new-refresh',
      userId: '1', email: 'person@example.test', fullName: 'Person', roles: []
    });
    await wait();
    for (const path of ['first', 'second']) {
      const retry = requests.expectOne(`${api}/${path}`);
      expect(retry.request.headers.get('Authorization')).toBe('Bearer new-access');
      retry.flush({ ok: true });
    }
    delayedRequest.flush({}, { status: 401, statusText: 'Unauthorized' });
    const retry = requests.expectOne(`${api}/delayed`);
    expect(retry.request.headers.get('Authorization')).toBe('Bearer new-access');
    retry.flush({ ok: true });
    await Promise.all([first, second, delayed]);
    expect(localStorage.getItem('accessToken')).toBe('new-access');
    expect(localStorage.getItem('refreshToken')).toBe('new-refresh');
    requests.expectNone(`${api}/Auth/refresh-token`);

    const later = firstValueFrom(http.get(`${api}/later`));
    requests.expectOne(`${api}/later`).flush({}, { status: 401, statusText: 'Unauthorized' });
    await wait();
    const nextRefresh = requests.expectOne(`${api}/Auth/refresh-token`);
    expect(nextRefresh.request.body).toEqual({ refreshToken: 'new-refresh' });
    nextRefresh.flush({
      token: 'next-access', refreshToken: 'next-refresh',
      userId: '1', email: 'person@example.test', fullName: 'Person', roles: []
    });
    await wait();
    const nextRetry = requests.expectOne(`${api}/later`);
    expect(nextRetry.request.headers.get('Authorization')).toBe('Bearer next-access');
    nextRetry.flush({ ok: true });
    await later;
    expect(localStorage.getItem('refreshToken')).toBe('next-refresh');
  });

  it('clears the session after a rejected refresh without retrying the request', async () => {
    const result = firstValueFrom(http.get(`${api}/protected`)).catch(error => error);
    requests.expectOne(`${api}/protected`).flush({}, { status: 401, statusText: 'Unauthorized' });
    await wait();
    requests.expectOne(`${api}/Auth/refresh-token`)
      .flush({}, { status: 401, statusText: 'Unauthorized' });
    await result;
    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
    requests.expectNone(`${api}/protected`);
    requests.expectNone(`${api}/Auth/refresh-token`);
  });

  it('revokes the stored token on logout and clears the local session', async () => {
    const result = firstValueFrom(TestBed.inject(AuthService).signOut());
    await wait();
    const request = requests.expectOne(`${api}/Auth/logout`);
    expect(request.request.body).toEqual({ refreshToken: 'old-refresh' });
    request.flush({ message: 'Logout successful.' });
    await result;
    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
  });
});
