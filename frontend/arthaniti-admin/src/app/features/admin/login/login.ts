import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  template: `
    <main class="login-page">
      <form class="login-card" [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <header class="login-heading">
          <h1>Admin Login</h1>
          <p class="subtitle">ArthaNiti Chartered Accountant Portal</p>
        </header>

        <div class="form-field">
          <label for="email">Email Address</label>
          <input
            id="email"
            type="email"
            formControlName="email"
            autocomplete="username"
            placeholder="Enter your email address"
          />
        </div>

        <div class="form-field">
          <label for="password">Password</label>
          <input
            id="password"
            type="password"
            formControlName="password"
            autocomplete="current-password"
            placeholder="Enter your password"
          />
        </div>

        @if (error()) {
          <p class="error" role="alert">{{ error() }}</p>
        }

        <button type="submit" [disabled]="form.invalid || submitting()">
          {{ submitting() ? 'Signing in...' : 'Sign In' }}
        </button>
      </form>
    </main>
  `,
  styles: `
    .login-page {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      box-sizing: border-box;
      background: #f0f4f8;
      padding: 1.5rem;
    }

    .login-card {
      width: 100%;
      max-width: 26rem;
      box-sizing: border-box;
      display: grid;
      gap: 1.5rem;
      padding: 2.5rem;
      background: #fff;
      border-radius: 0.9rem;
      border: 1px solid #edf0f3;
      box-shadow: 0 1rem 2.5rem rgba(31, 45, 61, 0.12);
    }

    .login-heading {
      text-align: center;
      margin-bottom: 0.5rem;
    }

    h1 {
      margin: 0;
      color: #172b45;
      font-size: 2rem;
      font-weight: 600;
    }

    .subtitle {
      margin: 0.4rem 0 0;
      color: #738197;
      font-size: 0.95rem;
    }

    .form-field {
      display: grid;
      gap: 0.55rem;
    }

    label {
      color: #263b56;
      font-size: 0.95rem;
      font-weight: 600;
    }

    input {
      width: 100%;
      min-height: 3.3rem;
      border: 1px solid #d5dce5;
      border-radius: 0.45rem;
      padding: 0 0.85rem;
      color: #1d2939;
      font: inherit;
      outline: none;
      transition: border-color 150ms ease, box-shadow 150ms ease;
    }

    input:focus {
      border-color: #2b63e8;
      box-shadow: 0 0 0 3px rgba(43, 99, 232, 0.12);
    }

    input::placeholder {
      color: #98a2b3;
    }

    button {
      min-height: 3.3rem;
      border: 0;
      border-radius: 0.45rem;
      background: #2b63e8;
      color: #fff;
      font: inherit;
      font-size: 1.05rem;
      font-weight: 600;
      cursor: pointer;
    }

    button:disabled {
      cursor: not-allowed;
      opacity: 0.75;
    }

    button:not(:disabled):hover {
      background: #2054d3;
    }

    .error {
      color: #e53935;
      margin: 0;
      line-height: 1.5;
    }

    @media (max-width: 480px) {
      .login-page {
        padding: 1rem;
      }

      .login-card {
        padding: 2rem 1.5rem;
      }

      h1 {
        font-size: 1.75rem;
      }
    }
  `,
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly error = signal('');

  readonly form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set('');

    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => void this.router.navigate(['/dashboard']),
      error: (error: unknown) => {
        this.submitting.set(false);
        this.error.set(error instanceof Error ? error.message : 'Login failed.');
      },
    });
  }
}
