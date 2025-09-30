import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
  ValidatorFn,
  AbstractControl,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';

@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css',
})
export class ResetPassword {
  constructor(private _auth: AuthService, private _router: Router) {}

  msg = '';
  email = '';

  // Fixed validator function
  passwordMatchValidator: ValidatorFn = (
    control: AbstractControl
  ): { [key: string]: boolean } | null => {
    const newPassword = control.get('newPassword')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (newPassword !== confirmPassword) {
      return { mismatch: true };
    }
    return null;
  };

  // Fixed form group initialization
  resetPasswordForm = new FormGroup(
    {
      code: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(6),
        Validators.pattern('^[0-9]*$'),
      ]),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: new FormControl('', [Validators.required]),
    },
    { validators: this.passwordMatchValidator }
  );

  ngOnInit() {
    // Get email from localStorage
    const storedEmail = localStorage.getItem('resetEmail');
    this.email = storedEmail || '';

    // If no email found, redirect to forgot password
    if (!this.email) {
      this._router.navigate(['/forgot-password']);
    }
  }

  onSubmit() {
    if (this.resetPasswordForm.valid) {
      const code = this.resetPasswordForm.get('code')?.value;
      const newPassword = this.resetPasswordForm.get('newPassword')?.value;

      if (!code || !newPassword || !this.email) return;

      this._auth.resetPassword(this.email, code, newPassword, newPassword).subscribe({
        next: (data) => {
          console.log('Password reset successful:', data);
          // Clear stored email
          localStorage.removeItem('resetEmail');
          // Navigate to login page
          this._router.navigate(['/login'], {
            queryParams: { passwordReset: true },
          });
        },
        error: (err) => {
          this.msg = err.error?.message || 'Failed to reset password';
        },
      });
    }
  }
}
