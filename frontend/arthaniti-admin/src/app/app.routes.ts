import { Routes } from '@angular/router';

export const routes: Routes = [
   {
    path: 'auth',
    loadChildren: () =>
      import('./layouts/auth-layout/auth-layout.routes')
        .then(m => m.AUTH_ROUTES)
  },
  {
    path: '',
    loadChildren: () =>
      import('./layouts/main-layout/main-layout.routes')
        .then(m => m.MAIN_ROUTES)
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];