import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../Core/Services/auth-service';

@Component({
  selector: 'app-verify-code',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './verify-code.html',
  styleUrl: './verify-code.css',
})
export class VerifyCode {
  constructor(
    private _auth: AuthService,
    private _router: Router,
    private _route: ActivatedRoute
  ) {}

  msg = '';
  purpose = '';
  email = '';

  verifyCodeForm = new FormGroup({
    code: new FormControl('', [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(6),
      Validators.pattern('^[0-9]*$'),
    ]),
  });

  ngOnInit() {
    // Get purpose from query params
    this._route.queryParams.subscribe((params) => {
      this.purpose = params['purpose'] || 'EmailVerification';

      // Get email from localStorage
      const storedEmail = localStorage.getItem('resetEmail');
      this.email = storedEmail || '';

      // If no email found, redirect to appropriate page
      if (!this.email) {
        if (this.purpose === 'PasswordReset') {
          this._router.navigate(['/forgot-password']);
        } else {
          this._router.navigate(['/register']);
        }
      }
    });
  }

  onSubmit() {
    if (this.verifyCodeForm.valid) {
      const code = this.verifyCodeForm.get('code')?.value;
      if (!code || !this.email) return;

      this._auth.verifyCode(this.email, code, this.purpose as any).subscribe({
        next: (data) => {
          console.log('Code verified:', data);

          if (this.purpose === 'PasswordReset') {
            // Navigate to reset password page
            this._router.navigate(['/reset-password']);
          } else {
            // For email verification, navigate to login
            this._router.navigate(['/login']);
          }
        },
        error: (err) => {
          this.msg = err.error?.message || 'Invalid verification code';
        },
      });
    }
  }
}
