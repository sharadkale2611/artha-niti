import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { DashboardLayoutComponent, type MenuNode, type TopbarUser } from '@adminlte/angular';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, map } from 'rxjs';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DashboardLayoutComponent, RouterOutlet],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly storedUser = this.authService.getStoredUser();

  readonly menu: MenuNode[] = [
    { type: 'item', text: 'Dashboard', route: '/dashboard', icon: 'bi-speedometer' },
    // {
    //   type: 'group',
    //   text: 'Customers',
    //   icon: 'bi-people',
    //   children: [
    //     {
    //       type: 'item',
    //       text: 'Customer Records',
    //       route: '/customers',
    //       icon: 'bi-person-lines-fill',
    //     },
    //   ],
    // },
  ];
  readonly user: TopbarUser = {
    name: this.storedUser?.fullName ?? 'Administrator',
    image: 'assets/images/admin-avatar.png',
    role: this.storedUser?.roles?.join(', ') || 'SuperAdmin',
  };
  readonly currentPath = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
    ),
    { initialValue: this.router.url },
  );

  onLogout(): void {
    this.authService.signOut().subscribe({
      next: () => void this.router.navigate(['/auth/login']),
      error: () => {
        this.authService.logout();
        void this.router.navigate(['/auth/login']);
      },
    });
  }
}
