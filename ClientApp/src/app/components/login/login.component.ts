import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, ValidatorFn } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/auth.service';

function getSafeAssistantReturnUrl(requestedUrl: string | null): string {
  if (!requestedUrl?.startsWith('/')) {
    return '/assistant';
  }

  try {
    const parsedUrl = new URL(requestedUrl, window.location.origin);
    return parsedUrl.origin === window.location.origin && parsedUrl.pathname === '/assistant'
      ? `${parsedUrl.pathname}${parsedUrl.search}${parsedUrl.hash}`
      : '/assistant';
  } catch {
    return '/assistant';
  }
}

const nonBlank: ValidatorFn = control =>
  typeof control.value === 'string' && control.value.trim().length > 0 ? null : { required: true };

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  readonly loginForm = new FormGroup({
    username: new FormControl('', { nonNullable: true, validators: [nonBlank] }),
    password: new FormControl('', { nonNullable: true, validators: [nonBlank] })
  });

  submitted = false;
  isPending = false;
  errorMessage = '';

  readonly usernameControl = this.loginForm.controls.username;
  readonly passwordControl = this.loginForm.controls.password;

  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly changeDetector = inject(ChangeDetectorRef);

  submit(): void {
    if (this.isPending) {
      return;
    }

    this.submitted = true;
    this.errorMessage = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isPending = true;
    const { username, password } = this.loginForm.getRawValue();
    this.authService.login({ username: username.trim(), password }).pipe(
      finalize(() => {
        this.isPending = false;
        this.changeDetector.markForCheck();
      })
    ).subscribe({
      next: () => {
        const requestedUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        const destination = getSafeAssistantReturnUrl(requestedUrl);
        void this.router.navigateByUrl(destination);
      },
      error: (error: unknown) => {
        this.errorMessage = error instanceof HttpErrorResponse && error.status === 401
          ? 'Username or password is incorrect.'
          : error instanceof HttpErrorResponse && error.status === 400
            ? 'Please check your sign-in details and try again.'
            : 'We could not reach the sign-in service. Please try again.';
        this.changeDetector.markForCheck();
      }
    });
  }
}