import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout')
        .then(m => m.PublicLayout),

    children: [
      // Home
      {
        path: '',
        loadComponent: () =>
          import('./features/home/home')
            .then(m => m.Home)
      },

     
    ]
  },

  // Invalid URL
  {
    path: '**',
    redirectTo: ''
  }
];