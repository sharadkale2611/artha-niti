import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  catchError,
  finalize,
  Observable,
  shareReplay,
  switchMap,
  throwError,
} from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';

const PUBLIC_AUTH_ENDPOINTS = [
  '/auth/login',
  '/auth/register',
  '/auth/refresh-token',
  '/auth/logout',
];

const apiBase = environment.apiUrl.replace(/\/+$/, '');
const apiPath = new URL(apiBase).pathname.replace(/\/+$/, '');

let refreshInFlight: Observable<string> | null = null;

function toApiUrl(url: string): string {
  const isRelative = url.startsWith('/') && !url.startsWith('//');

  if (!isRelative || url.startsWith('/assets/')) {
    return url;
  }

  // Avoid duplicating the path segment already present in apiUrl (e.g. /api)
  if (apiPath && (url === apiPath || url.startsWith(`${apiPath}/`))) {
    return `${new URL(apiBase).origin}${url}`;
  }

  return `${apiBase}${url}`;
}

function isPublicEndpoint(url: string): boolean {
  const path = url.slice(apiBase.length).split(/[?#]/)[0].toLowerCase();

  return PUBLIC_AUTH_ENDPOINTS.some(
    (endpoint) => path === endpoint || path === `${endpoint}/`,
  );
}

function withBearer(
  req: HttpRequest<unknown>,
  token: string,
): HttpRequest<unknown> {
  return req.clone({
    headers: req.headers.set('Authorization', `Bearer ${token}`),
  });
}

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const url = toApiUrl(req.url);
  let apiRequest = url === req.url ? req : req.clone({ url });

  const isApiRequest =
    apiRequest.url === apiBase || apiRequest.url.startsWith(`${apiBase}/`);

  if (!isApiRequest || isPublicEndpoint(apiRequest.url)) {
    return next(apiRequest);
  }

  const token = authService.getAccessToken();
  const hadExplicitAuth = apiRequest.headers.has('Authorization');

  if (token && !hadExplicitAuth) {
    apiRequest = withBearer(apiRequest, token);
  }

  const sentRequest = apiRequest;

  return next(sentRequest).pipe(
    catchError((error: unknown) => {
      // Only refresh for requests this interceptor authenticated.
      if (
        !(error instanceof HttpErrorResponse) ||
        error.status !== 401 ||
        !token ||
        hadExplicitAuth
      ) {
        return throwError(() => error);
      }

      const currentToken = authService.getAccessToken();
      if (currentToken && currentToken !== token) {
        return next(withBearer(sentRequest, currentToken));
      }

      if (!refreshInFlight) {
        refreshInFlight = authService.refreshSession(token).pipe(
          finalize(() => { refreshInFlight = null; }),
          shareReplay({ bufferSize: 1, refCount: false }),
        );
      }

      return refreshInFlight.pipe(
        catchError((refreshError: unknown) => {
          if (authService.getAccessToken() === token) {
            authService.logout();
            void router.navigate(['/auth/login']);
          }
          return throwError(() => refreshError);
        }),
        switchMap((newToken) => next(withBearer(sentRequest, newToken))),
      );
    }),
  );
};
