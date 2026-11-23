import { Routes } from '@angular/router';

import { MainLayout } from './main-layout';
import { authGuard } from '../../core/guards/auth.guard';
import { adminGuard } from '../../core/guards/admin.guard';

export const MAIN_ROUTES: Routes = [
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard, adminGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('../../features/dashboard/dashboard').then((m) => m.Dashboard),
      },
     
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
    ],
  },
];
