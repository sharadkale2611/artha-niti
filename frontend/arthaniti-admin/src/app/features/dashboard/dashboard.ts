import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section>
      <h1>Dashboard</h1>
      <p>Welcome to the ArthaNiti admin application.</p>
    </section>
  `,
})
export class Dashboard {}
