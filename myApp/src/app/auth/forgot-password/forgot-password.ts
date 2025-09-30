import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  constructor(private _auth: AuthService, private _router: Router) {}

  msg = '';
  emailSent = false;

  forgotPasswordForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
  });

  onSubmit() {
    if (this.forgotPasswordForm.valid) {
      const emailValue = this.forgotPasswordForm.get('email')?.value;
      if (!emailValue) return;

      this._auth.forgotPassword(emailValue).subscribe({
        next: (data: any) => {
          console.log('Password reset code sent:', data);
          this.emailSent = true;
          // Store email in localStorage for next steps
          localStorage.setItem('resetEmail', emailValue);
          // Navigate to verify code page
          this._router.navigate(['/verify-code'], {
            queryParams: { purpose: 'PasswordReset' },
          });
        },
        error: (err) => {
          this.msg = err.error?.message || 'Failed to send reset code';
        },
      });
    }
  }
}
