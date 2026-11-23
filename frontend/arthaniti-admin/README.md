# ArthaNiti - Admin

Angular admin application for the ArthaNiti Chartered Accountant platform.

## Technology

- Angular 22
- Standalone components
- Angular Router
- Angular Reactive Forms
- RxJS
- Bootstrap 5
- Bootstrap Icons
- Angular Material

## Project Structure

```text
src/app/
  core/
    guards/             Route authentication and role guards
    interceptors/       HTTP authentication and refresh handling
    models/             Authentication models
    services/           Authentication and API services
  features/
    admin/login/        Standalone login component
    dashboard/          Dashboard landing component
  layouts/
    auth-layout/        Login route layout
    main-layout/        Authenticated application layout
  shared/               Shared UI and master-data styles
```

## Routing

- `/auth/login` displays the login page.
- `/auth` redirects to `/auth/login`.
- `/dashboard` displays the dashboard.
- `/` redirects to `/dashboard`.
- Protected main routes require authentication and the `SuperAdmin` role.

The route guards redirect unauthenticated or non-admin users to `/auth/login`.

## Development Login

Development login bypasses the API only when Angular uses the development
environment:

```text
Email: admin@example.com
Password: Admin@123
```

This credential creates a local development session with the `SuperAdmin` role.
The bypass is disabled in production. Real credentials are submitted to the
configured API in production mode.

The API URL is configured in:

- `environments/environment.ts`
- `environments/environment.prod.ts`

## Layout

The main layout contains:

- Fixed desktop sidebar
- Responsive mobile sidebar with backdrop
- ArthaNiti branding and custom logo
- Dashboard navigation
- Logout button in the top navigation
- Router outlet for page content

Logout clears tokens and stored user data, then redirects to `/auth/login`.

## Branding

The interface uses an ArthaNiti A-monogram that combines a capital A with a
financial growth arrow. The logo is available at:

```text
src/assets/images/arthaniti-logo.svg
```

The browser favicon and sidebar use this logo.

## Color Palette

```text
Primary blue: #1685E5
Teal:         #16A982
Warm accent:  #F0A33A
Canvas:       #F5F7FB
Sidebar:      #FFFFFF
```

The theme variables are defined in `src/styles.scss` and begin with
`--brand-*`, `--surface-*`, `--text-*`, and `--border-*`.

## Local Setup

Install dependencies:

```powershell
npm install
```

Start the development server:

```powershell
npm start
```

Open the application and navigate to:

```text
http://localhost:4200/auth/login
```

## Validation

Create a development build:

```powershell
npm run build
```

Create a production build:

```powershell
npm run build -- --configuration production
```

## Security Notes

- Use only the development credential locally.
- Do not add real credentials or secrets to source files.
- Replace dummy development authentication behavior before exposing the
  application outside a local development environment.
- Configure HTTPS and a valid production API URL before deployment.
