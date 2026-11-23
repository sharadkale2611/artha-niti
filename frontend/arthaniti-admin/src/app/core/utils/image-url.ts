import { environment } from '../../../environments/environment';

const ABSOLUTE_URL = /^(?:[a-z][a-z\d+.-]*:|\/\/)/i;

// Static files are served from the backend root, so strip the API path (e.g. "/api").
function getBackendOrigin(): string {
  try {
    return new URL(environment.apiUrl).origin;
  } catch {
    return environment.apiUrl.replace(/\/api\/?$/i, '').replace(/\/+$/, '');
  }
}

export function resolveImageUrl(path: string | null | undefined): string | null {
  const value = path?.trim();

  if (!value) {
    return null;
  }

  if (ABSOLUTE_URL.test(value)) {
    return value;
  }

  return `${getBackendOrigin()}/${value.replace(/^\/+/, '')}`;
}
